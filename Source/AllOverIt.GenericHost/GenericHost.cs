using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace AllOverIt.GenericHost
{
    public sealed class GenericHost
    {
        public static IHostBuilder CreateConsoleHostBuilder(string[] args)
        {
            // HostedConsoleService injects the implementation as an IConsoleApp
            return CreateHostBuilder<HostedConsoleService>(args)
                .ConfigureHostConfiguration(configBuilder =>
                {
                    configBuilder.SetBasePath(Directory.GetCurrentDirectory());
                });
        }

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
    }
}