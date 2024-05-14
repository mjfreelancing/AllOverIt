using AllOverIt.GenericHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HostedConsoleAppDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await GenericHost
                .CreateConsoleHostBuilder<App>(args)

                // The main console app is implemented in the App class. Here we are injecting
                // an additional worker background service just to demonstrate it is possible.
                .ConfigureServices(services =>
                {
                    services.AddHostedService<ConsoleBackgroundWorker>();
                })

                .RunConsoleAsync(options => options.SuppressStatusMessages = true);
        }
    }
}
