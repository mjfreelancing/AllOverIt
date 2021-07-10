using AllOverIt.GenericHost;
using AllOverIt.Helpers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HostedConsoleApp
{
    public sealed class BackgroundWorker : ConsoleWorker
    {
        private readonly ILogger<BackgroundWorker> _logger;

        public BackgroundWorker(IHostApplicationLifetime applicationLifetime, ILogger<BackgroundWorker> logger)
            : base(applicationLifetime)
        {
            _logger = logger.WhenNotNull(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // if anything needs to run after the while block then the code would need to catch and ignore
            // TaskCanceledException exceptions. This is not required when using IHostApplicationLifetime
            // as shown in this class (the exception is caught elsewhere and the notifications are still made).

            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Background Worker: {DateTimeOffset.Now}");
                await Task.Delay(1000, cancellationToken);
            }
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