using AllOverIt.Assertion;
using AllOverIt.Async;
using AllOverIt.Aws.AppSync.Client;
using AllOverIt.Aws.AppSync.Client.Authorization;
using AllOverIt.Aws.AppSync.Client.Configuration;
using AllOverIt.Aws.AppSync.Client.Exceptions;
using AllOverIt.Aws.AppSync.Client.Request;
using AllOverIt.Aws.AppSync.Client.Response;
using AllOverIt.Aws.AppSync.Client.Subscription;
using AllOverIt.Extensions;
using AllOverIt.GenericHost;
using AllOverIt.Serialization.NewtonsoftJson;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
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
        private readonly AppSyncSubscriptionClient _subscriptionClient;
        private readonly IWorkerReady _workerReady;
        private readonly ILogger<SubscriptionWorker> _logger;
        private CompositeAsyncDisposable _compositeSubscriptions = new();

        public SubscriptionWorker(IHostApplicationLifetime applicationLifetime, AppSyncSubscriptionClient subscriptionClient,
            IWorkerReady workerReady, ILogger<SubscriptionWorker> logger)
            : base(applicationLifetime)
        {
            _subscriptionClient = subscriptionClient.WhenNotNull(nameof(subscriptionClient));
            _workerReady = workerReady.WhenNotNull(nameof(workerReady));
            _logger = logger.WhenNotNull(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // Write the current connection status to the console
            _subscriptionClient.ConnectionState
                .Subscribe(state =>
                {
                    LogMessage($"Connection State: {state}");
                });

            // Write all errors to the console
            _subscriptionClient.GraphqlErrors
                .Subscribe(response =>
                {
                    // Not displaying the error.Type, which can be:
                    // "error" - such as when a query is provided instead of a subscription (will have error code)

                    var message = string.Join(", ", response.Error.Payload.Errors.Select(GetErrorMessage));
                    LogMessage($"{response.Id}: {message}");
                });

            // Write all exceptions to the console
            _subscriptionClient.Exceptions
                .Subscribe(exception =>
                {
                    switch (exception)
                    {
                        // "connection_error"  - such as when the sub-protocol is not defined on the web socket (will have error type)
                        case WebSocketConnectionException connectionException:
                        {
                            var message = string.Join(", ", connectionException.Errors.Select(GetErrorMessage));
                            LogMessage($"{message}");
                            break;
                        }

                        // WebSocketConnectionTimeoutException:
                        // SubscribeTimeoutException
                        // UnsubscribeTimeoutException
                        case TimeoutExceptionBase timeoutException:
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
                GetSubscription1(_subscriptionClient),
                GetSubscription2(_subscriptionClient),
                GetSubscription3(_subscriptionClient));

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
                    LogMessage($" - Subscription '{subscription.Key}'");

                    var exceptions = subscription.SelectMany(item => item.Exceptions);

                    foreach (var exception in exceptions)
                    {
                        LogMessage($"  - {exception.Message}");
                    }
                }
            }

            // then dispose of them
            Console.WriteLine();
            LogMessage("Disposing of ALL subscriptions...");
            Console.WriteLine();

            // safe to do even if the subscription failed
            await subscription1.DisposeAsync();
            await subscription2.DisposeAsync();
            await subscription3.DisposeAsync();

            // at this point the WebSocket will have been closed by the client as all subscriptions are now disposed of
            Console.WriteLine();

            // and subscribe again, sequentially, to check everything re-connects as expected
            LogMessage("Registering subscriptions again, sequentially...");
            Console.WriteLine();

            subscription1 = await GetSubscription1(_subscriptionClient);
            Console.WriteLine();

            subscription2 = await GetSubscription2(_subscriptionClient);
            Console.WriteLine();
            
            subscription3 = await GetSubscription3(_subscriptionClient);
            Console.WriteLine();

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

            // Testing closing the connection without unsubscribing, then dispose of one subscription,
            // then reconnect, the re-subscribe the closed subscription (will get a new Id). After all
            // this, all 3 subscriptions should still be working.
            //
            LogMessage("Closing the connection (keeping any existing subscriptions)");
            await _subscriptionClient.DisconnectAsync();

            // will only remove from the internal registered list because there's no active connection
            LogMessage($"Disposing of {nameof(subscription2)}");
            await subscription2.DisposeAsync();

            LogMessage("Re-opening the connection, will re-subscribe the existing subscriptions with AppSync");
            var isAlive = await _subscriptionClient.ConnectAsync();

            LogMessage(isAlive
                ? "The connection has been re-established"
                : "The connection has not been re-established");

            // If the connection is not available this will retry to establish the connection
            LogMessage($"Re-create a new subscription for {nameof(subscription2)}");
            subscription2 = await GetSubscription2(_subscriptionClient);

            // ready to start receiving messages
            if (subscription1.Success || subscription2.Success || subscription3.Success)
            {
                LogMessage("Ready to start receiving subscription responses");
                Console.WriteLine();
            }

            // the user can now press a key to terminate (via the main console)
            _workerReady.SetCompleted();

            SendMutations(cancellationToken);

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
        private static async Task<IAppSyncSubscriptionRegistration> GetSubscription1(AppSyncSubscriptionClient client)
        {
            // try this for an unsupported operation error
            // var badQuery = "query MyQuery { defaultLanguage { code name } }";

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
        private static async Task<IAppSyncSubscriptionRegistration> GetSubscription2(AppSyncSubscriptionClient client)
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
        private static async Task<IAppSyncSubscriptionRegistration> GetSubscription3(AppSyncSubscriptionClient client)
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

        private static async Task<IAppSyncSubscriptionRegistration> GetSubscription(AppSyncSubscriptionClient client, string name, string query, object variables = null)
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

        private static void SendMutations(CancellationToken cancellationToken)
        {
            var options = new GraphqlClientConfiguration
            {
                EndPoint = "https://pbwlv45sfbfzzd22wqmlrahw5y.appsync-api.ap-southeast-2.amazonaws.com/graphql",
                Serializer = new NewtonsoftJsonSerializer(),
                DefaultAuthorization = new AppSyncApiKeyAuthorization("da2-gcb75twfwjep5ols2qwyefjlki")
            };

            var client = new AppSyncClient(options);

            var mutation = new GraphqlQuery
            {
                //Query = "query MyQuery { defaultLanguage { code name } }"

                Query =
                    @"mutation MyMutation($code: ID!, $name: String!) {
                        addLanguage(language: {code: $code, name: $name}) {
                          code
                          name
                        }
                     }"
            };

            var counter = 0;
            var codes = new[] {"LNG1", "LNG2", "LNG3"};

            RepeatingTask.Start(async () =>
            {
                mutation.Variables = new
                {
                    Code = codes[counter++ % 3],
                    Name = $"{Guid.NewGuid()}"
                };

                // Queries are identical; just call SendQueryAsync()
                // SendQueryAsync() and SendMutationAsync() are identical - the two methods exist for readability
                var response = await client
                    .SendMutationAsync<AddLanguageResponse>(mutation, cancellationToken)
                    .ConfigureAwait(false);

                LogMessage($"Sent mutation: {options.Serializer.SerializeObject(mutation.Variables)}");
                LogMessage($"Response: {options.Serializer.SerializeObject(response.Data)}");
            }, 3000, cancellationToken);
        }
    }
}