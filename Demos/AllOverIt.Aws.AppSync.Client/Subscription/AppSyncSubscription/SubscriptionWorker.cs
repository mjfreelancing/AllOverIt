using AllOverIt.Aws.AppSync.Client.Exceptions;
using AllOverIt.Aws.AppSync.Client.Subscription;
using AllOverIt.Extensions;
using AllOverIt.GenericHost;
using AllOverIt.Helpers;
using AllOverIt.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reactive.Concurrency;
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
        private readonly ILogger<SubscriptionWorker> _logger;
        private CompositeAsyncDisposable _compositeSubscriptions = new();

        public SubscriptionWorker(IHostApplicationLifetime applicationLifetime, IOptions<AppSyncOptions> options, ILogger<SubscriptionWorker> logger)
            : base(applicationLifetime)
        {
            _logger = logger.WhenNotNull(nameof(logger));
            _apiOptions = options.Value.WhenNotNull(nameof(options));
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var client = new AppSyncSubscriptionClient(
                _apiOptions.ApiHost,
                new ApiKeyAuthorization(_apiOptions.ApiKey),
                new AppSyncClientNewtonsoftJsonSerializer());

            // Write the current connection status to the console
            client.ConnectionState
                .ObserveOn(Scheduler.CurrentThread)
                .Subscribe(state =>
                {
                    Console.WriteLine($"Connection State: {state}");
                });

            // Write all errors to the console
            client.GraphqlErrors
                .ObserveOn(Scheduler.CurrentThread)
                .Subscribe(error =>
                {
                    // Not displaying the error.Type, which can be:
                    // "error" - such as when a query is provided instead of a subscription (will have error code)

                    var message = string.Join(", ", error.Payload.Errors.Select(GetErrorMessage));
                    Console.WriteLine(message);
                });

            // Write all exceptions to the console
            client.Exceptions
                .ObserveOn(Scheduler.CurrentThread)
                .Subscribe(exception =>
                {
                    // "connection_error"  - such as when the sub-protocol is not defined on the web socket (will have error type)
                    if (exception is GraphqlConnectionException connectionException)
                    {
                        var message = string.Join(", ", connectionException.Errors.Select(GetErrorMessage));
                        Console.WriteLine(message);
                    }


                    Console.WriteLine(exception.Message);
                });

            // Subscribe to a mutation using two different queries - at the same time to test connection locking
            var (subscription1, subscription2) = await TaskHelper.WhenAll(
                GetSubscription1(client),
                GetSubscription2(client));

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
                Console.WriteLine("Subscriptions are now ready");
                Console.WriteLine();
            }

            // This task will complete when all subscriptions are cleaned up when _compositeSubscriptions is disposed via OnStopping()
            var subscriptionDisposalTask = _compositeSubscriptions.GetDisposalCompletion();

            // non - blocking wait => will complete when the user presses a key in the main console (cancellationToken is signaled)
            var waitForCancelTask = Task.Run(() =>
            {
                cancellationToken.WaitHandle.WaitOne();
            }, cancellationToken);

            await Task.WhenAll(subscriptionDisposalTask, waitForCancelTask);
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("The background worker is stopping");

            _compositeSubscriptions.Dispose();
            _compositeSubscriptions = null;
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("The background worker is done");
        }

        private static Task<IAsyncDisposable> GetSubscription1(AppSyncSubscriptionClient client)
        {
            // try this for an unsupported operation error
            // "query MyQuery { defaultLanguage { code name } }"

            return GetSubscription(client, "Subscription1", @"subscription MySubscription1 {addedLanguage(code: ""LNG"") {code name}}");
        }

        private static Task<IAsyncDisposable> GetSubscription2(AppSyncSubscriptionClient client)
        {
            return GetSubscription(client, "Subscription2", @"subscription MySubscription2 {addedLanguage {code name}}");
        }

        private static async Task<IAsyncDisposable> GetSubscription(AppSyncSubscriptionClient client, string name, string query)
        {
            Console.WriteLine($"Adding subscription {name}");

            var subscriptionQuery = new SubscriptionQuery
            {
                Query = query
            };

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
                ? $"{name} is registered"
                : $"{name} failed to register");

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