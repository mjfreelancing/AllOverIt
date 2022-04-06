using System;
using System.Threading.Tasks;
using AllOverIt.DependencyInjection.Extensions;
using AllOverIt.GenericHost;
using ExternalDependencies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AutoRegistration
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).RunConsoleAsync(options => options.SuppressStatusMessages = true);
        }

        // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-5.0
        // If the app uses Entity Framework Core, don't change the name or signature of the CreateHostBuilder method.
        // The Entity Framework Core tools expect to find a CreateHostBuilder method that configures the host without
        // running the app. For more information, see https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dbcontext-creation
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            // The main console app is implemented in the App class. Here we are configuring the
            // auto-registration of classes based on 'marker' interfaces.

            return GenericHost
                .CreateConsoleHostBuilder<App>(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging(configure => configure.AddConsole());

                    services.AutoRegisterSingleton(new ExternalDependenciesRegistrar(Console.WriteLine), new[] { typeof(RepositoryBase) }, true);

                    // Note: There are overloads that take a generic for the 'Registrar' (requires a default ctor)

                    // ILocalRepository => LocalRepository
                    services.AutoRegisterSingleton(new DemoAppRegistrar(Console.WriteLine), typeof(IRepository));

                    // IExternalInternalRepository => ExternalInternalRepository
                    // IExternalPublicRepository => ExternalPublicRepository
                    services.AutoRegisterSingleton(new ExternalDependenciesRegistrar(Console.WriteLine), typeof(IRepository));

                    // IAppProvider => PublicSealedAppProvider
                    // IAppProvider => InternalSealedAppProvider
                    services.AutoRegisterSingleton(
                        new ExternalDependenciesRegistrar(Console.WriteLine),
                        new[] { typeof(IAppProvider) },
                        (provider, implementationType) =>
                        {
                            // This callback provides the arguments to be passed to the object's constructor.
                            var logger = provider.GetService<ILogger<Program>>();
                            return new object[] {DateTime.Now, logger};
                        },
                        true);

                    Console.WriteLine();
                });
        }
    }
}
