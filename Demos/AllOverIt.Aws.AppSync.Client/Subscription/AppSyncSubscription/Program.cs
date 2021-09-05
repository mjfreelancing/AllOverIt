﻿using AllOverIt.GenericHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                    // SubscriptionWorker subscribes to a graphql
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
                    services.Configure<AppSyncOptions>(hostContext.Configuration.GetSection(nameof(AppSyncOptions)))
                        .AddOptions()
                        .AddScoped<IConsoleApp, SubscriptionConsole>();
                });
        }
    }
}
