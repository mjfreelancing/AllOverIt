using AllOverIt.DependencyInjection.Extensions;
using AllOverIt.GenericHost;
using ExternalDependencies;
using Microsoft.Extensions.Hosting;

namespace AutoRegistrationDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await GenericHost
                .CreateConsoleHostBuilder<App>(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AutoRegisterSingleton<ExternalRegistrar, IRepository>()    // Will find the internal NameRepository (in the external assembly)
                        .Decorate<IRepository, DecoratedRepository>();              // Will replace the IRepository registration with a decorated implementation

                    Console.WriteLine();
                })
                .RunConsoleAsync(options => options.SuppressStatusMessages = true);
        }
    }
}
