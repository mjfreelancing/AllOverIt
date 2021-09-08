﻿using AllOverIt.Aws.AppSync.Client.Exceptions;
using AllOverIt.Aws.AppSync.Client.Subscription;
using AllOverIt.Extensions;
using AllOverIt.GenericHost;
using AllOverIt.Helpers;
using AllOverIt.Serialization.Newtonsoft;
using AllOverIt.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AppSyncSubscription
{
    /*
     
    Using user secrets - the secrets.json file will look like:

    {
        "appSyncOptions": {
            "apiHost": " example123.appsync-api.ap-southeast-2.amazonaws.com",
            "apiKey": "graphql_api_key"
        }
    }

    */

    public sealed class SubscriptionWorker : ConsoleWorker
    {
        private readonly AppSyncOptions _apiOptions;
        private readonly IWorkerReady _workerReady;
        private readonly ILogger<SubscriptionWorker> _logger;
        private CompositeAsyncDisposable _compositeSubscriptions = new();

        public SubscriptionWorker(IHostApplicationLifetime applicationLifetime, IOptions<AppSyncOptions> options, IWorkerReady workerReady,
            ILogger<SubscriptionWorker> logger)
            : base(applicationLifetime)
        {
            _apiOptions = options.Value.WhenNotNull(nameof(options));
            _workerReady = workerReady.WhenNotNull(nameof(workerReady));
            _logger = logger.WhenNotNull(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var client = new AppSyncSubscriptionClient(
                _apiOptions.ApiHost,
                new ApiKeyAuthorization(_apiOptions.ApiKey),
                new NewtonsoftJsonSerializer());

            // Write the current connection status to the console
            client.ConnectionState
                .Subscribe(state =>
                {
                    LogMessage($"Connection State: {state}");
                });

            // Write all errors to the console
            client.GraphqlErrors
                .Subscribe(response =>
                {
                    // Not displaying the error.Type, which can be:
                    // "error" - such as when a query is provided instead of a subscription (will have error code)

                    var message = string.Join(", ", response.Error.Payload.Errors.Select(GetErrorMessage));
                    LogMessage($"{response.Id}: {message}");
                });

            // Write all exceptions to the console
            client.Exceptions
                .Subscribe(exception =>
                {
                    switch (exception)
                    {
                        // "connection_error"  - such as when the sub-protocol is not defined on the web socket (will have error type)
                        case GraphqlConnectionException connectionException:
                        {
                            var message = string.Join(", ", connectionException.Errors.Select(GetErrorMessage));
                            LogMessage($"{message}");
                            break;
                        }

                        // GraphqlConnectionTimeoutException:
                        // GraphqlSubscribeTimeoutException
                        // GraphqlUnsubscribeTimeoutException
                        case GraphqlTimeoutExceptionBase timeoutException:
                            LogMessage($"{timeoutException.Message}");
                            break;

                        default:
                            // ? WebSocketConnectionLostException
                            LogMessage($"{exception.Message}");
                            break;
                    }
                });

            // Subscribe to a mutation using two different queries - at the same time to test connection locking
            // Exceptions are raised on the exception observable as well as being populated in the subscription result.

            // first, subscribe them all at the same time
            var (subscription1, subscription2, subscription3) = await TaskHelper.WhenAll(
                GetSubscription1(client),
                GetSubscription2(client),
                GetSubscription3(client));

            // collate all exceptions raised during the subscription process
            var subscriptionErrors = new[] {subscription1, subscription2, subscription3}
                .Where(item => item.Exceptions != null)
                .Select(item => item)
                .GroupBy(item => item.Id)
                .AsReadOnlyCollection();

            if (subscriptionErrors.Any())
            {
                LogMessage("Subscription errors received:");

                foreach (var subscription in subscriptionErrors)
                {
                    var subscriptionId = subscription.Key;

                    LogMessage(subscriptionId.IsNullOrEmpty()
                        ? " - Subscription failure with no connection"
                        : $" - Subscription '{subscription.Key}'");

                    var exceptions = subscription.SelectMany(item => item.Exceptions);

                    foreach (var exception in exceptions)
                    {
                        LogMessage($"  - {exception.Message}");
                    }
                }
            }

            // then dispose of them
            Console.WriteLine();
            LogMessage("Disposing of subscriptions...");
            Console.WriteLine();

            // safe to do even if the subscription failed
            await subscription1.DisposeAsync();
            await subscription2.DisposeAsync();
            await subscription3.DisposeAsync();

            // and subscribe again, sequentially, to check everything re-connects as expected
            Console.WriteLine();
            LogMessage("Registering subscriptions again, sequentially...");
            Console.WriteLine();

            subscription1 = await GetSubscription1(client);
            subscription2 = await GetSubscription2(client);
            subscription3 = await GetSubscription3(client);

            // Track all valid subscriptions that we need to wait for when shutting down
            // Example: If one subscription is an invalid query then it will be returned as null
            if (subscription1.Success)
            {
                _compositeSubscriptions.Add(subscription1);
            }

            if (subscription2.Success)
            {
                _compositeSubscriptions.Add(subscription2);
            }

            if (subscription3.Success)
            {
                _compositeSubscriptions.Add(subscription3);
            }

            if (subscription1.Success || subscription2.Success || subscription3.Success)
            {
                LogMessage("One or more subscriptions are now ready");
                Console.WriteLine();
            }
 
            // the user can now press a key to terminate (via the main console)
            _workerReady.SetCompleted();

            // non - blocking wait => will complete when the user presses a key in the main console (cancellationToken is signaled)
            await Task.Run(() =>
            {
                cancellationToken.WaitHandle.WaitOne();
            }, cancellationToken);
        }

        protected override void OnStopping()
        {
            _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - The background worker is stopping");

            _compositeSubscriptions.Dispose();
            _compositeSubscriptions = null;
        }

        protected override void OnStopped()
        {
            _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - The background worker is done");

            // shutdown is not graceful after this returns
        }

        // Explicitly subscribes to the addLanguage("LNG1") mutation
        private static async Task<SubscriptionId> GetSubscription1(AppSyncSubscriptionClient client)
        {
            // try this for an unsupported operation error
            var badQuery = "query MyQuery { defaultLanguage { code name } }";

            var goodQuery = @"subscription MySubscription1 {
                                addedLanguage(code: ""LNG1"") {
                                  code
                                  name
                                }
                              }";

            var subscription = await GetSubscription(client, "Subscription1", goodQuery);

            if (subscription.Success)
            {
                LogMessage(" => Listening ONLY for the code 'LNG1'");
            }

            return subscription;
        }

        // Subscribes to ALL addLanguage() mutations
        private static async Task<SubscriptionId> GetSubscription2(AppSyncSubscriptionClient client)
        {
            var subscription = await GetSubscription(
                client,
                "Subscription2",
                @"subscription Subscription2 {
                    addedLanguage {
                      code
                      name
                    }
                  }");

            if (subscription.Success)
            {
                LogMessage(" => Listening for ALL codes");
            }

            return subscription;
        }

        // Explicitly subscribes to the addLanguage("LNG1") mutation using a variable
        private static async Task<SubscriptionId> GetSubscription3(AppSyncSubscriptionClient client)
        {
            var langCode = "LNG3";

            var subscription = await GetSubscription(
                client,
                "Subscription3",
                @"subscription Subscription3($code: ID!) {
                    addedLanguage(code: $code) {
                      code
                      name
                    }
                  }",
                new { code = langCode });

            if (subscription.Success)
            {
                LogMessage($" => Listening ONLY for the code '{langCode}' using a variable");
            }

            return subscription;
        }

        private static async Task<SubscriptionId> GetSubscription(AppSyncSubscriptionClient client, string name, string query, object variables = null)
        {
            var subscriptionQuery = new SubscriptionQuery
            {
                Query = query,
                Variables = variables
            };

            LogMessage($"Adding subscription {name}, Id = {subscriptionQuery.Id}");

            var subscription = await client.SubscribeAsync<AddedLanguageResponse>(
                subscriptionQuery,
                response =>
                {
                    var type = response.Errors.IsNullOrEmpty()
                        ? "Data"
                        : "Errors";

                    var message = response.Errors.IsNullOrEmpty()
                        ? (object) response.Data
                        : response.Errors;

                    LogMessage($"{name}: {type}{Environment.NewLine}" +
                               $"{JsonConvert.SerializeObject(message, new JsonSerializerSettings { Formatting = Formatting.Indented })}");

                    Console.WriteLine();
                });

            string GetFailureMessage()
            {
                var errors = subscription.Exceptions != null
                    ? subscription.Exceptions.Select(item => item.Message)
                    : subscription.GraphqlErrors.Select(item => item.Message);

                return string.Join(", ", errors);
            }

            LogMessage(subscription.Success
                ? $"{name} is registered (Id: {subscription.Id})"
                : $"{name} FAILED: {GetFailureMessage()}");

            return subscription;
        }

        private static string GetErrorMessage(GraphqlErrorDetail detail)
        {
            if (detail.ErrorCode.HasValue)
            {
                return $"({detail.ErrorCode}): {detail.Message}";
            }

            if (!detail.ErrorType.IsNullOrEmpty())
            {
                return $"({detail.ErrorType}): {detail.Message}";
            }

            return detail.Message;
        }

        private static void LogMessage(string message)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }
    }
}