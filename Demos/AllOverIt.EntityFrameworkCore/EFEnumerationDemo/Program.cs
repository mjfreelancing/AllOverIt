using AllOverIt.GenericHost;
using EFEnumerationDemo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
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
        return GenericHost
            .CreateConsoleHostBuilder<App>(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContextFactory<BloggingContext>();
            });
    }
}
