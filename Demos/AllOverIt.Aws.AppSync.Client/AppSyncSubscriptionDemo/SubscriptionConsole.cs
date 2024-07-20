using AllOverIt.Assertion;
using AllOverIt.GenericHost;
using Microsoft.Extensions.Logging;

namespace AppSyncSubscriptionDemo
{
    public sealed class SubscriptionConsole : ConsoleAppBase
    {
        private readonly IWorkerReady _workerReady;
        private readonly ILogger<SubscriptionConsole> _logger;

        public SubscriptionConsole(IWorkerReady workerReady, ILogger<SubscriptionConsole> logger)
        {
            _workerReady = workerReady.WhenNotNull();
            _logger = logger.WhenNotNull();
        }

        public override async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await _workerReady.Wait().ConfigureAwait(false);

            Console.WriteLine();
            Console.WriteLine("All Over It (the background worker will continue until a key is pressed).");
            Console.WriteLine();
            Console.ReadKey();

            ExitCode = 0;
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