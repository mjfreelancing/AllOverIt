using AllOverIt.GenericHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HostedConsoleAppDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args)

                // The main console app is implemented in the App class. Here we are injecting
                // an additional worker background service just to demonstrate it is possible.
                .ConfigureServices(services =>
                {
                    services.AddHostedService<ConsoleBackgroundWorker>();
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
                .CreateConsoleHostBuilder<App>(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging(configure => configure.AddConsole());

                    // if 'App' requires anything to be injected, set it up here
                });
        }
    }
}
