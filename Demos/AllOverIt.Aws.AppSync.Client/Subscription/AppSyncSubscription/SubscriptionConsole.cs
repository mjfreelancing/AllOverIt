using AllOverIt.GenericHost;
using AllOverIt.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppSyncSubscription
{
    public sealed class SubscriptionConsole : ConsoleAppBase
    {
        private readonly ILogger<SubscriptionConsole> _logger;

        public SubscriptionConsole(ILogger<SubscriptionConsole> logger)
        {
            _logger = logger.WhenNotNull(nameof(logger));
        }

        public override Task StartAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine();
            Console.WriteLine("All Over It (the background worker will continue until a key is pressed).");
            Console.WriteLine();
            Console.ReadKey();

            ExitCode = 0;

            return Task.FromResult(true);
        }

        public override void OnStopping()
        {
            _logger.LogInformation("SubscriptionConsole is stopping");
        }

        public override void OnStopped()
        {
            _logger.LogInformation("SubscriptionConsole is stopped");
        }
    }
}