using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.GenericHost;
using DiagnosticsDemo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

await GenericHost
    .CreateConsoleHostBuilder<App>(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<IBreadcrumbs>(_ =>
        {
            var options = new BreadcrumbsOptions
            {
                ThreadSafe = true
            };

            return new Breadcrumbs(options);
        });

        services.AddHostedService<ConsoleBackgroundWorker>();
        services.AddLogging(configure => configure.AddConsole());
    })
    .RunConsoleAsync(options => options.SuppressStatusMessages = true);

