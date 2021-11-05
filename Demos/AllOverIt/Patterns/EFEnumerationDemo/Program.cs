using AllOverIt.GenericHost;
using EFEnumerationDemo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        await GenericHost
            .CreateConsoleHostBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext<BloggingContext>();
                services.AddScoped<IConsoleApp, App>();
            })
            .RunConsoleAsync(options => options.SuppressStatusMessages = true);
    }
}
