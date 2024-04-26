using AllOverIt.Aws.AppSync.Client.Authorization;
using AllOverIt.Aws.AppSync.Client.Configuration;
using AllOverIt.Aws.AppSync.Client.Extensions;
using AllOverIt.GenericHost;
using AllOverIt.Serialization.Json.Abstractions;
using AllOverIt.Serialization.Json.Newtonsoft;
using AllOverIt.Serialization.Json.SystemText;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AppSyncSubscriptionDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args)

                // The main console app is implemented in the SubscriptionConsole class. Here we are injecting
                // an additional worker background service just to demonstrate it is possible.
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IWorkerReady, WorkerReady>();

                    // Take your pick
                    // This is only for use within SubscriptionWorker - the clients create their own instance.
                    // Also mixing and matching. Using SystemTextJsonSerializer here whereas the clients are using NewtonsoftJsonSerializer
                    services.AddSingleton<IJsonSerializer, SystemTextJsonSerializer>();
                    //services.AddSingleton<IJsonSerializer, NewtonsoftJsonSerializer>();

                    // Only use one of the these:
                    //   If you use AddAppSyncClient() then SubscriptionWorker requires IAppSyncClient
                    //   If you use AddAppSyncNamedClient() then SubscriptionWorker requires IAppSyncNamedClientProvider
                    //AddAppSyncClient(services);
                    AddNamedAppSyncClient(services);

                    AddSubscriptionClient(services);

                    // This performs the graphql subscription and logging of errors and responses received
                    services.AddHostedService<SubscriptionWorker>();
                })
                .RunConsoleAsync(options => options.SuppressStatusMessages = true);
        }

        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-5.0
        // If the app uses Entity Framework Core, don't change the name or signature of the CreateHostBuilder method.
        // The Entity Framework Core tools expect to find a CreateHostBuilder method that configures the host without
        // running the app. For more information, see https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dbcontext-creation
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return GenericHost
                .CreateConsoleHostBuilder<SubscriptionConsole>(args)
                .ConfigureHostConfiguration(configBuilder => configBuilder.AddUserSecrets<AppSyncOptions>())
                .ConfigureServices((hostContext, services) =>
                {
                    // AppSyncOptions is loaded from user secrets
                    services
                        .AddOptions()
                        .Configure<AppSyncOptions>(hostContext.Configuration.GetSection(nameof(AppSyncOptions)));

                    // Alternative - allows for validation at startup
                    //  .AddOptions<AppSyncOptions>()
                    //  .Bind(hostContext.Configuration.GetSection(nameof(AppSyncOptions)))
                    //  .ValidateUsingFluentValidation() or Validate() etc
                    //  .ValidateOnStart();

                    // If not using the generic version of CreateConsoleHostBuilder(), then this must be manually called
                    // .AddSingleton<IConsoleApp, SubscriptionConsole>();
                });
        }

        private static void AddAppSyncClient(IServiceCollection services)
        {
            services.AddAppSyncClient(provider =>
            {
                var options = provider.GetRequiredService<IOptions<AppSyncOptions>>().Value;
                var serializer = CreateJsonJsonSerializer();

                return new AppSyncClientConfiguration
                {
                    EndPoint = $"https://{options.Host}/graphql",

                    DefaultAuthorization = new AppSyncApiKeyAuthorization(options.ApiKey),
                    Serializer = serializer
                };
            });
        }

        private static void AddNamedAppSyncClient(IServiceCollection services)
        {
            services.AddAppSyncNamedClient((provider, name) =>
            {
                // demo code just to show that the configuration is named based (change the name used in SubscriptionWorker and this call will throw)
                if (name == "Public")
                {
                    var options = provider.GetRequiredService<IOptions<AppSyncOptions>>().Value;
                    var serializer = CreateJsonJsonSerializer();

                    return new AppSyncClientConfiguration
                    {
                        EndPoint = $"https://{options.Host}/graphql",

                        DefaultAuthorization = new AppSyncApiKeyAuthorization(options.ApiKey),
                        Serializer = serializer
                    };
                }

                throw new InvalidOperationException($"Unknown client name '{name}'");
            });
        }

        private static void AddSubscriptionClient(IServiceCollection services)
        {
            // registers SubscriptionClientConfiguration and populates properties via IOptions<AppSyncOptions>
            services.AddAppSyncSubscriptionClient(provider =>
            {
                var options = provider.GetRequiredService<IOptions<AppSyncOptions>>().Value;
                var serializer = CreateJsonJsonSerializer();

                return new SubscriptionClientConfiguration
                {
                    Host = options.Host,

                    // RealtimeUrl will be derived from Host when not provided by replacing 'appsync-api' with 'appsync-realtime-api'
                    RealtimeUrl = options.RealtimeUrl,

                    DefaultAuthorization = new AppSyncApiKeyAuthorization(options.ApiKey),
                    Serializer = serializer,

                    // Just to show this can be changed
                    ConnectionOptions = new SubscriptionClientConnectionOptions
                    {
                        ConnectionTimeout = TimeSpan.FromSeconds(10),
                        SubscriptionTimeout = TimeSpan.FromSeconds(10)
                    }
                };
            });
        }

        // Created per-client in this demo. Could also create a single configured instance for all clients.
        private static NewtonsoftJsonSerializer CreateJsonJsonSerializer()
        {
            // take your pick...
            var serializer = new NewtonsoftJsonSerializer();
            //var serializer = new SystemTextJsonSerializer();

            serializer.Configure(new JsonSerializerConfiguration
            {
                SupportEnrichedEnums = true
            });

            return serializer;
        }
    }
}
