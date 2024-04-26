using AllOverIt.GenericHost;
using AllOverIt.Validation.Options.Extensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OptionsValidationDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await GenericHost
                .CreateConsoleHostBuilder<ConsoleApp>(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IValidator<AppOptions>, AppOptionsValidator>();

                    services
                        .AddOptions<AppOptions>()
                        .Bind(hostContext.Configuration.GetSection(AppOptions.SectionName))
                        .UseFluentValidation()      // Use FluentValidation to provide the rule validation
                        .ValidateOnStart();         // Validate immediately - not later when the app attempts to read the options

                    // If not using the generic version of CreateConsoleHostBuilder(), then this must be manually called
                    // .AddSingleton<IConsoleApp, ConsoleApp>();
                })
                .RunConsoleAsync(options => options.SuppressStatusMessages = true);
        }
    }
}