using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace AllOverIt.GenericHost
{
    /// <summary>Provides static methods to create host builders.</summary>
    public sealed class GenericHost
    {
        /// <summary>Create a host builder for a hosted console application.</summary>
        /// <param name="args">The application's command line arguments.</param>
        /// <returns>A <see cref="IHostBuilder"/> instance.</returns>
        /// <remarks>This overload requires a call to <see cref="IHostBuilder.ConfigureServices"/> to register a Singleton <see cref="IConsoleApp"/> instance.</remarks>
        public static IHostBuilder CreateConsoleHostBuilder(string[] args = default)
        {
            // HostedConsoleService injects the implementation as an IConsoleApp
            return CreateHostedConsoleServiceBuilder(args);
        }

        /// <summary>Create a host builder for a hosted console application.</summary>
        /// <typeparam name="TConsoleApp">An <see cref="IConsoleApp"/> derived class that will be registered as a Singleton for injection into the hosted service.</typeparam>
        /// <param name="args">The application's command line arguments.</param>
        /// <returns>A <see cref="IHostBuilder"/> instance.</returns>
        public static IHostBuilder CreateConsoleHostBuilder<TConsoleApp>(string[] args = default) where TConsoleApp : class, IConsoleApp
        {
            // HostedConsoleService injects the implementation as an IConsoleApp
            return CreateHostedConsoleServiceBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IConsoleApp, TConsoleApp>();
                });
        }

        /// <summary>Initializes a new instance of the <see cref="HostBuilder" /> class with pre-configured defaults
        /// and registers the specified hosted service type.</summary>
        /// <typeparam name="THostedService">The hosted service type implementing <see cref="IHostedService"/>.</typeparam>
        /// <param name="args">The application's command line arguments.</param>
        /// <returns>The initialized <see cref="HostBuilder" /> instance.</returns>
        public static IHostBuilder CreateHostBuilder<THostedService>(string[] args)
            where THostedService : class, IHostedService
        {
            /*
                Default behaviour of CreateDefaultBuilder:
                  * Sets the content root to the path returned by GetCurrentDirectory.
                  * Loads host configuration from:
                  * Environment variables prefixed with DOTNET_.
                  * Command-line arguments.
                  * Loads app configuration from:
                      appsettings.json.
                      appsettings.{Environment}.json.
                      User secrets when the app runs in the Development environment.
                      Environment variables.
                      Command-line arguments.
                  * Adds the following logging providers:
                      Console
                      Debug
                      EventSource
                      EventLog (only when running on Windows)
                  * Enables scope validation and dependency validation when the environment is Development.
                 
                 https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host
             */

            return Host
                .CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHostedService<THostedService>();
                });
        }

        private static IHostBuilder CreateHostedConsoleServiceBuilder(string[] args)
        {
            return CreateHostBuilder<HostedConsoleService>(args)
                .ConfigureHostConfiguration(configBuilder =>
                {
                    configBuilder.SetBasePath(Directory.GetCurrentDirectory());
                });
        }
    }
}