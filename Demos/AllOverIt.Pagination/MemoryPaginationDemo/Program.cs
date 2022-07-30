using AllOverIt.GenericHost;
using AllOverIt.Pagination.Extensions;
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
                services.AddQueryPaginatorFactory();
            })
            .RunConsoleAsync(options => options.SuppressStatusMessages = true);
    }
}
