using AllOverIt.Aws.AppSync.Client.Exceptions;
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
                    Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Connection State: {state}");
                });

            // Write all errors to the console
            client.GraphqlErrors
                .Subscribe(response =>
                {
                    // Not displaying the error.Type, which can be:
                    // "error" - such as when a query is provided instead of a subscription (will have error code)

                    var message = string.Join(", ", response.Error.Payload.Errors.Select(GetErrorMessage));
                    Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {response.Id}: {message}");
                });

            // Write all exceptions to the console
            client.Exceptions
                .Subscribe(exception =>
                {
                    // "connection_error"  - such as when the sub-protocol is not defined on the web socket (will have error type)
                    if (exception is GraphqlConnectionException connectionException)
                    {
                        var message = string.Join(", ", connectionException.Errors.Select(GetErrorMessage));
                        Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
                    }

                    // WebSocketConnectionLostException

                    Console.WriteLine(exception.Message);
                });

            // Subscribe to a mutation using two different queries - at the same time to test connection locking
            // A null subscription is returned if there was a problem with the connection or subscription request.
            // The error / exception observables will have reported the problem.

            // first, subscribe them both at the same time
            var (subscription1, subscription2) = await TaskHelper.WhenAll(
                GetSubscription1(client),
                GetSubscription2(client));

            // then dispose of them
            Console.WriteLine();
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Disposing of subscriptions...");
            Console.WriteLine();
            if (subscription1 != null)
            {
                await subscription1.DisposeAsync();
            }
            if (subscription2 != null)
            {
                await subscription2.DisposeAsync();
            }

            // and subscribe again, sequentially, to check everything re-connects as expected
            Console.WriteLine();
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Registering subscriptions again, sequentially...");
            Console.WriteLine();
            subscription1 = await GetSubscription1(client);
            subscription2 = await GetSubscription2(client);

            // Track all valid subscriptions that we need to wait for when shutting down
            // Example: If one subscription is an invalid query then it will be returned as null
            if (subscription1 != null)
            {
                _compositeSubscriptions.Add(subscription1);
            }

            if (subscription2 != null)
            {
                _compositeSubscriptions.Add(subscription2);
            }

            if (subscription1 != null && subscription2 != null)
            {
                Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Subscriptions are now ready");
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

        private static Task<IAsyncDisposable> GetSubscription1(AppSyncSubscriptionClient client)
        {
            // try this for an unsupported operation error
            var badQuery = "query MyQuery { defaultLanguage { code name } }";
            //var goodQuery = @"subscription MySubscription1 {addedLanguage(code: ""LNG"") {code name}}";
            var goodQuery = @"subscription MySubscription1 {addedLanguage(language: {code: ""LNG"", name: ""Language Name""}) {code name}}";

            return GetSubscription(client, "Subscription1", goodQuery);
        }

        private static Task<IAsyncDisposable> GetSubscription2(AppSyncSubscriptionClient client)
        {
            return GetSubscription(client, "Subscription2", @"subscription MySubscription2 {addedLanguage {code name}}");
        }

        private static async Task<IAsyncDisposable> GetSubscription(AppSyncSubscriptionClient client, string name, string query)
        {
            var subscriptionQuery = new SubscriptionQuery
            {
                Query = query
            };

            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Adding subscription {name}, Id = {subscriptionQuery.Id}");

            var subscription = await client.SubscribeAsync<AddedLanguageResponse>(
                subscriptionQuery,
                response =>
                {
                    var message = response.Errors.IsNullOrEmpty()
                        ? (object) response.Data
                        : response.Errors;

                    Console.WriteLine($"{name}: {JsonConvert.SerializeObject(message, new JsonSerializerSettings { Formatting = Formatting.Indented })}");
                    Console.WriteLine();
                });

            Console.WriteLine(subscription != null
                ? $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {name} is registered (Id: {subscription.Id})"
                : $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {name} failed to register");

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
    }
}