using AllOverIt.GenericHost;
using AllOverIt.Pagination;
using AllOverIt.Pagination.Extensions;
using AllOverIt.Serialization.NewtonsoftJson;
using MemoryPaginationDemo;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        await GenericHost
            .CreateConsoleHostBuilder<App>(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddQueryPaginator(provider =>
                {
                    return new QueryPaginatorOptions
                    {
                        // The pageSize can be provided here as the default page size and not given to the BuildQuery() method.
                        // The BuildQuery() method can be provided an override.
                        // DefaultPageSize = 25,

                        // This is the default so can be left off
                        Direction = PaginationDirection.Forward,

                        Serializer = new NewtonsoftJsonSerializer()
                    };
                });
            })
            .RunConsoleAsync(options => options.SuppressStatusMessages = true);
    }
}
