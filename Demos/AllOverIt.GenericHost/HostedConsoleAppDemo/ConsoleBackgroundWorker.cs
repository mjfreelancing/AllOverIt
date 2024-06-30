using AllOverIt.Assertion;
using AllOverIt.GenericHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HostedConsoleAppDemo
{
    public sealed class ConsoleBackgroundWorker : BackgroundWorker
    {
        private readonly ILogger<ConsoleBackgroundWorker> _logger;

        public ConsoleBackgroundWorker(IHostApplicationLifetime applicationLifetime, ILogger<ConsoleBackgroundWorker> logger)
            : base(applicationLifetime)
        {
            _logger = logger.WhenNotNull(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Waiting for everything to start");

            // ExecuteAsync() starts before OnStarted() fires, so this shows how to wait. The result indicates if the app has started
            // or not (gone into the Stopping state).
            var started = await WaitForStartup();

            if (!started)
            {
                _logger.LogInformation("Failed to start");
                return;
            }

            _logger.LogInformation("Now running");

            // if anything needs to run after the while block then the code would need to catch and ignore
            // TaskCanceledException exceptions. This is not required when using IHostApplicationLifetime
            // as shown in this class (the exception is caught elsewhere and the notifications are still made).

            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Background Worker: {DateTimeOffset}", DateTimeOffset.Now);

                await Task.Delay(1000, cancellationToken);
            }
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