using AllOverIt.GenericHost;
using AllOverIt.Pagination;
using AllOverIt.Pagination.Extensions;
using AllOverIt.Serialization.NewtonsoftJson;
using KeysetPaginationConsole;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        DatabaseProvider.RecreateData = false;
        DatabaseProvider.Use = DatabaseChoice.Sqlite;

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
                services.AddQueryPaginator(provider =>
                {
                    return new QueryPaginatorConfig
                    {                       
                        Serializer = new NewtonsoftJsonSerializer()
                    };
                });

                services.AddDbContextFactory<BloggingContext>();
            });
    }
}
