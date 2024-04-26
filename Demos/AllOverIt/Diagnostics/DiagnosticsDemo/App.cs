using AllOverIt.Assertion;
using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.Extensions;
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

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting...");

            Console.WriteLine();
            Console.WriteLine("All Over It (the background worker will finish shortly, or when a key is pressed).");
            Console.WriteLine();
            Console.ReadKey();

            ExitCode = 0;

            return Task.CompletedTask;
        }

        public override void OnStopping()
        {
            _logger.LogInformation("App is stopping");

            var allBreadcrumbs = _breadcrumbs.ToList();

            Console.WriteLine();
            _logger.LogInformation("Breadcrumbs captured...");

            foreach (var breadcrumb in allBreadcrumbs)
            {
                var timeOffset = (breadcrumb.Timestamp - _breadcrumbs.StartTimestamp).TotalMilliseconds;

                _logger.LogInformation(breadcrumb.Metadata != null
                    ? $"({timeOffset}ms) {breadcrumb.Message} : {(DateTime) breadcrumb.Metadata}"
                    : $"({timeOffset}ms) {breadcrumb.Message}");

                if (breadcrumb.CallerName.IsNotNullOrEmpty())
                {
                    _logger.LogInformation($"Called from {breadcrumb.CallerName} at {breadcrumb.FilePath}:{breadcrumb.LineNumber}");
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