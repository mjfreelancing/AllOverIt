using AllOverIt.Aws.AppSync.Client.Authorization;
using AllOverIt.Aws.AppSync.Client.Configuration;
using AllOverIt.Aws.AppSync.Client.Extensions;
using AllOverIt.GenericHost;
using AllOverIt.Serialization.Abstractions;
using AllOverIt.Serialization.NewtonsoftJson;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace AppSyncSubscription
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

                    // take your pick...
                    services.AddSingleton<IJsonSerializer, NewtonsoftJsonSerializer>();
                    //services.AddSingleton<IJsonSerializer, SystemTextJsonSerializer>();

                    // Only use one of the these:
                    //   If you use AddAppSyncClient() then SubscriptionWorker requires IAppSyncClient
                    //   If you use AddNamedAppSyncClient() then SubscriptionWorker requires INamedAppSyncClientProvider
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
                .CreateConsoleHostBuilder(args)
                .ConfigureHostConfiguration(configBuilder => configBuilder.AddUserSecrets<AppSyncOptions>())
                .ConfigureServices((hostContext, services) =>
                {
                    // AppSyncOptions is loaded from user secrets
                    services
                        .AddOptions()
                        .Configure<AppSyncOptions>(hostContext.Configuration.GetSection(nameof(AppSyncOptions)))
                        .AddScoped<IConsoleApp, SubscriptionConsole>();
                });
        }

        private static void AddAppSyncClient(IServiceCollection services)
        {
            services.AddAppSyncClient(provider => 
            {
                var options = provider.GetRequiredService<IOptions<AppSyncOptions>>().Value;
                var serializer = provider.GetRequiredService<IJsonSerializer>();

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
            services.AddNamedAppSyncClient((provider, name) =>
            {
                // demo code just to show that the configuration is named based (change the name used in SubscriptionWorker and this call will throw)
                if( name == "Public")
                {
                    var options = provider.GetRequiredService<IOptions<AppSyncOptions>>().Value;
                    var serializer = provider.GetRequiredService<IJsonSerializer>();

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
                var serializer = provider.GetRequiredService<IJsonSerializer>();

                return new SubscriptionClientConfiguration
                {
                    Host = options.Host,

                    // RealTimeUrl will be derived from Host when not provided by replacing 'appsync-api' with 'appsync-realtime-api'
                    RealTimeUrl = options.RealTimeUrl,

                    DefaultAuthorization = new AppSyncApiKeyAuthorization(options.ApiKey),
                    Serializer = serializer
                };
            });
        }
    }
}
