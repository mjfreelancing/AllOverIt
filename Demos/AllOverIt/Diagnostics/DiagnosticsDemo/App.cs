using AllOverIt.Assertion;
using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.Extensions;
using AllOverIt.GenericHost;
using DiagnosticsDemo.Extensions;
using Microsoft.Extensions.Logging;

namespace DiagnosticsDemo
{
    public sealed class App : ConsoleAppBase
    {
        public static readonly string RandomTag = "random";

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

            // Add some random int[] data - the demo uses a custom data collator to group consecutive items with the same Tag
            _breadcrumbs.AddIntArrayData(false);

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

            Console.WriteLine();
            _logger.LogInformation("Breadcrumbs captured...");

            foreach (var breadcrumb in _breadcrumbs.WithRandomDataCollated(RandomTag))
            {
                var timeOffset = (breadcrumb.Timestamp - _breadcrumbs.StartTimestamp).TotalMilliseconds;

                if (breadcrumb.Metadata is null)
                {
                    _logger.LogInformation("({TimeOffset}ms) {Message}", timeOffset, breadcrumb.Message);
                }
                else
                {
                    _logger.LogInformation("({TimeOffset}ms) {Message} : {Data}", timeOffset, breadcrumb.Message, breadcrumb.Metadata);
                }

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