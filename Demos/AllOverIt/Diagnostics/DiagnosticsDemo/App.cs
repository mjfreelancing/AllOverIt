using AllOverIt.Assertion;
using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.GenericHost;
using Microsoft.Extensions.Logging;

namespace DiagnosticsDemo
{
    public sealed class App : ConsoleAppBase
    {
        private readonly IBreadcrumbs _breadcrumbs;
        private readonly ILogger<App> _logger;

        public App(IBreadcrumbs breadcrumbs, ILogger<App> logger)
        {
            _breadcrumbs = breadcrumbs.WhenNotNull(nameof(breadcrumbs));
            _logger = logger.WhenNotNull(nameof(logger));
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting...");

            Console.WriteLine();
            Console.WriteLine("All Over It (the background worker will finish shortly, or when a key is pressed).");
            Console.WriteLine();
            Console.ReadKey();

            ExitCode = 0;
        }

        public override void OnStopping()
        {
            _logger.LogInformation("App is stopping");

            var allBreadcrumbs = _breadcrumbs.ToList();

            Console.WriteLine();
            _logger.LogInformation("Breadcrumbs captured...");

            foreach (var breadcrumb in allBreadcrumbs)
            {
                if (breadcrumb.Metadata != null)
                {
                    _logger.LogInformation($"{breadcrumb.Message} : {(DateTime) breadcrumb.Metadata}");
                }
                else
                {
                    _logger.LogInformation(breadcrumb.Message);
                }

                Console.WriteLine();
            }
        }

        public override void OnStopped()
        {
            _logger.LogInformation("App is stopped");
        }
    }
}