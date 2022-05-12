using AllOverIt.Assertion;
using AllOverIt.Diagnostics.Breadcrumbs;
using AllOverIt.Diagnostics.Breadcrumbs.Extensions;
using AllOverIt.Extensions;
using AllOverIt.GenericHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DiagnosticsDemo
{
    public sealed class ConsoleBackgroundWorker : BackgroundWorker
    {
        private readonly ILogger<ConsoleBackgroundWorker> _logger;
        private readonly IBreadcrumbs _breadcrumbs;

        public ConsoleBackgroundWorker(IBreadcrumbs breadcrumbs, IHostApplicationLifetime applicationLifetime, ILogger<ConsoleBackgroundWorker> logger)
            : base(applicationLifetime)
        {
            _breadcrumbs = breadcrumbs.WhenNotNull(nameof(breadcrumbs));
            _logger = logger.WhenNotNull(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _breadcrumbs.Add("Waiting for everything to start");

            _logger.LogInformation("Waiting for everything to start");

            // ExecuteAsync() starts before OnStarted() fires, so this shows how to wait. The result indicates if the app has started
            // or not (gone into the Stopping state).
            var started = await WaitForStartup();

            if (!started)
            {
                _logger.LogInformation("Failed to start");
                return;
            }

            _breadcrumbs.Add("Now running");

            _logger.LogInformation("Now running");




            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);

            var tasks = Enumerable
                .Range(1, 10)
                .SelectAsReadOnlyCollection(item =>
                {
                    return Task.Run(async () =>
                    {
                        var threadId = Task.CurrentId;

                        while (!linkedTokenSource.IsCancellationRequested)
                        {
                            _breadcrumbs.Add($"Thread {threadId} is running", DateTime.Now);

                            await Task.Delay(500);
                        }
                    }, linkedTokenSource.Token);
                });

            await Task.WhenAll(tasks);
        
            var allBreadcrumbs = _breadcrumbs.ToList();

            Console.WriteLine("Breadcrumbs...");

            foreach (var breadcrumb in allBreadcrumbs)
            {
                if (breadcrumb.Metadata != null)
                {
                    Console.WriteLine($"{breadcrumb.Message} : {(DateTime) breadcrumb.Metadata}");
                }
                else
                {
                    Console.WriteLine(breadcrumb.Message);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Background worker is done. Press a key to end the process.");
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("The background worker has started");
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("The background worker is stopping");
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("The background worker is done");
        }
    }
}