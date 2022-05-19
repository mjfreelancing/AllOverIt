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
                        Serializer = new NewtonsoftJsonSerializer()
                    };
                });
            })
            .RunConsoleAsync(options => options.SuppressStatusMessages = true);
    }
}
