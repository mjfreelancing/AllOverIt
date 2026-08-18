using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.GenericHost;
using Microsoft.Extensions.Logging;

namespace HostedConsoleAppDemo
{
    public sealed class App : ConsoleAppBase
    {
        private readonly ILogger<App> _logger;

        public App(ILogger<App> logger)
        {
            _logger = logger.WhenNotNull();
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartAsync");

            // providing an initial delay so the background worker can show it is alive
            if (!await WaitCompletedAsync(cancellationToken))
            {
                return;
            }

            Console.WriteLine();
            Console.WriteLine("ENVIRONMENT VARIABLES");
            Console.WriteLine("=====================");

            var variables = Environment.GetEnvironmentVariables().ToSerializedDictionary();

            foreach (var (key, value) in variables)
            {
                Console.WriteLine($"{key} = {value}");
            }

            Console.WriteLine();

            // providing another delay so the background worker can show it is still alive
            if (!await WaitCompletedAsync(cancellationToken))
            {
                return;
            }

            ExitCode = 0;

            Console.WriteLine();
            Console.WriteLine("All Over It (the background worker will continue until a key is pressed).");
            Console.WriteLine();

            // Wait for a key press, but stop immediately on shutdown (e.g. Ctrl+C) instead of blocking in a
            // synchronous Console.ReadKey() that ignores the cancellation token - otherwise the host's bounded
            // shutdown wait would time out before the app can exit.
            var keyPressTask = Task.Run(() => Console.ReadKey());
            var shutdownTask = Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);

            await Task.WhenAny(keyPressTask, shutdownTask);
        }

        public override void OnStopping()
        {
            _logger.LogInformation("=> App is stopping");
        }

        public override void OnStopped()
        {
            if (UserCancelled)
            {
                _logger.LogWarning("The application was cancelled via CTRL+C");
            }

            _logger.LogInformation("=> App is stopped");
        }

        private async Task<bool> WaitCompletedAsync(CancellationToken cancellationToken)
        {
            if (UserCancelled)
            {
                return false;
            }

            try
            {
                await Task.Delay(3000, cancellationToken);
                return true;
            }
            catch (OperationCanceledException)
            {
                ExitCode = -2;
                return false;
            }
        }
    }
}