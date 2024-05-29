using AllOverIt.Assertion;
using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.Diagnostics.Breadcrumbs.Extensions;
using AllOverIt.Extensions;
using AllOverIt.GenericHost;
using DiagnosticsDemo.Extensions;
using Microsoft.Extensions.Logging;

namespace DiagnosticsDemo
{
    public sealed class App : ConsoleAppBase
    {
        private const string _randomTag = "random";

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

            // Add some random, but related content - the demo uses a custom data collator to group consecutive items with the same Tag
            for (var i = 0; i < 10; i++)
            {
                var data = Enumerable
                    .Range(0, Random.Shared.Next(20) + 5)
                    .SelectToArray(_ => Random.Shared.Next(100));

                _breadcrumbs.Add("Random Data", data, _randomTag);
            }

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

            foreach (var breadcrumb in _breadcrumbs.WithRandomDataCollated())
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