﻿using AllOverIt.Aws.AppSync.Client;
using AllOverIt.Aws.AppSync.Client.Authorization;
using AllOverIt.Aws.AppSync.Client.Configuration;
using AllOverIt.GenericHost;
using AllOverIt.Serialization.NewtonsoftJson;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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

                    // registers SubscriptionClientConfiguration and populates properties via IOptions<AppSyncOptions>
                    services.AddSingleton(provider =>
                    {
                        var options = provider.GetRequiredService<IOptions<AppSyncOptions>>().Value;
                        
                        return new SubscriptionClientConfiguration
                        {
                            Host = options.Host,

                            // RealTimeUrl will be derived from Host when not provided by replacing 'appsync-api' with 'appsync-realtime-api'
                            RealTimeUrl = options.RealTimeUrl,

                            DefaultAuthorization = new AppSyncApiKeyAuthorization(options.ApiKey),

                            // take your pick between Newtonsoft and System.Text
                            Serializer = new NewtonsoftJsonSerializer()
                            //Serializer = new SystemTextJsonSerializer()
                        };
                    });

                    // AppSyncSubscriptionClient has several constructors so register a factory method to construct it
                    // using a SubscriptionClientConfiguration
                    services.AddSingleton(provider =>
                    {
                        var configuration = provider.GetRequiredService<SubscriptionClientConfiguration>();
                        return new AppSyncSubscriptionClient(configuration);
                    });

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
    }
}
