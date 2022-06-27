using AllOverIt.GenericHost;
using AllOverIt.Pagination.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaginationConsole;
using System.Threading.Tasks;

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
        // Using a static object as it simplifies the demo, especially when wanting to create migrations for different database providers.
        DemoStartupOptions.RecreateData = false;
        DemoStartupOptions.Use = DatabaseChoice.PostgreSql;
        DemoStartupOptions.RunPerformanceOnly = false;

        return GenericHost
            .CreateConsoleHostBuilder<App>(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddQueryPaginator();
                services.AddDbContextFactory<BloggingContext>();
            });
    }
}
