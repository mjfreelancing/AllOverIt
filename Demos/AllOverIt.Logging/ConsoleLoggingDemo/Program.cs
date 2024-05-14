using AllOverIt.GenericHost;
using AllOverIt.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsoleLoggingDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Using alternative message templates
            LogCallOptions.UseCallPrefix("CallSite: ");                     // Default is "Call: "
            LogCallOptions.UseMethodNameProperty("CallSite");               // Default is "MethodName"
            LogCallOptions.UseArgumentsPrefix("Args: ");                    // Default is "Arguments = "
            LogCallOptions.UseArgumentsDestructureProperty("Args");         // Default is "Arguments" (used as @Arguments in the template)
            LogCallOptions.UseExceptionPrefix("Exception: ");               // Default is "Error: "
            LogCallOptions.UseExceptionMessageProperty("Exception");        // Default is "ErrorMessage"
            LogCallOptions.IncludeCallerNamespace(false);                   // Default is true

            await GenericHost
                .CreateConsoleHostBuilder<App>(args)
                .ConfigureServices((hostBuilder, services) =>
                {
                    services.AddSingleton<IAppService, AppService>();
                    services.AddSingleton<IAppRepository, AppRepository>();
                })
                .RunConsoleAsync(options => options.SuppressStatusMessages = true);
        }
    }
}