using AllOverIt.Assertion;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AllOverIt.GenericHost
{
    internal sealed class HostedConsoleService : IHostedService
    {
        private readonly IConsoleApp _consoleApp;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ILogger _logger;

        private int? _exitCode;

        public HostedConsoleService(IConsoleApp consoleApp, IHostApplicationLifetime applicationLifetime, ILogger<HostedConsoleService> logger)
        {
            _applicationLifetime = applicationLifetime.WhenNotNull(nameof(applicationLifetime));
            _consoleApp = consoleApp.WhenNotNull(nameof(consoleApp));
            _logger = logger.WhenNotNull(nameof(logger));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting with arguments: {string.Join(" ", Environment.GetCommandLineArgs())}");

            _applicationLifetime.ApplicationStarted.Register(() => OnStarted(cancellationToken));
            _applicationLifetime.ApplicationStopping.Register(OnStopping);
            _applicationLifetime.ApplicationStopped.Register(OnStopped);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Exiting with return code: {_exitCode}");

            // Exit code may be null if the user cancelled via Ctrl+C/SIGTERM
            Environment.ExitCode = _exitCode.GetValueOrDefault(_consoleApp.DefaultExitCode);
            return Task.CompletedTask;
        }

        private void OnStarted(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                try
                {
                    await _consoleApp.StartAsync(cancellationToken);
                    _exitCode = _consoleApp.ExitCode;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception!");
                    _exitCode = _consoleApp.UnhandedErrorExitCode;
                }
                finally
                {
                    // Stop the application once the work is done
                    _applicationLifetime.StopApplication();
                }
            }, cancellationToken);
        }

        private void OnStopping()
        {
            _consoleApp.OnStopping();
        }

        private void OnStopped()
        {
            _consoleApp.OnStopped();
        }
    }
}