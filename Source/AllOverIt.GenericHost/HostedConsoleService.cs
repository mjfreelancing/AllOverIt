using AllOverIt.Assertion;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AllOverIt.GenericHost
{
    internal sealed class HostedConsoleService : IHostedService
    {
        private readonly IConsoleApp _consoleApp;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ILogger _logger;
        private readonly TimeSpan _shutdownWaitTimeout;

        private int? _exitCode;
        private Task? _consoleAppTask;

        // HostBuilder registers the IOptions<> open generic, so IOptions<ConsoleHostOptions>
        // resolves even if a consumer does not register it.
        public HostedConsoleService(IConsoleApp consoleApp, IHostApplicationLifetime applicationLifetime,
            IOptions<ConsoleHostOptions> hostOptions, ILogger<HostedConsoleService> logger)
        {
            _applicationLifetime = applicationLifetime.WhenNotNull();
            _consoleApp = consoleApp.WhenNotNull();

            var options = hostOptions.WhenNotNull().Value;
            _shutdownWaitTimeout = options.ShutdownWaitTimeout;

            _logger = logger.WhenNotNull();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
#pragma warning disable CA1873 // Avoid potentially expensive logging
            _logger.LogInformation("Starting with arguments: {CommandLineArgs}", string.Join(" ", Environment.GetCommandLineArgs()));
#pragma warning restore CA1873 // Avoid potentially expensive logging

            _applicationLifetime.ApplicationStarted.Register(() => OnStarted(cancellationToken));
            _applicationLifetime.ApplicationStopping.Register(OnStopping);
            _applicationLifetime.ApplicationStopped.Register(OnStopped);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Wait (bounded) for the console app to finish unwinding so its exit code is captured before
            // being read. Without this, a Ctrl+C/SIGTERM mid-run makes StopAsync read a null exit code and
            // default Environment.ExitCode to DefaultExitCode (0), discarding the app's own exit code.
            if (_consoleAppTask is not null)
            {
                try
                {
                    await _consoleAppTask
                        .WaitAsync(_shutdownWaitTimeout, CancellationToken.None)    // Deliberate default cancellation token
                        .ConfigureAwait(false);
                }
                catch (TimeoutException)
                {
                    // The app did not finish unwinding within the configured timeout - fall through to
                    // the exit-code fallback below (which distinguishes user cancellation).
                }
                catch (OperationCanceledException)
                {
                    // The app's task was cancelled - fall through to the exit-code fallback below.
                }
            }

#pragma warning disable CA1873 // Avoid potentially expensive logging
            _logger.LogInformation("Exiting with return code: {ExitCode}", _exitCode);
#pragma warning restore CA1873 // Avoid potentially expensive logging

            // When the app did not set an exit code, report a deliberate non-success code if the user
            // cancelled (rather than the success default), otherwise fall back to the default exit code.
            Environment.ExitCode = _exitCode
                ?? (_consoleApp.UserCancelled
                    ? _consoleApp.CancelledExitCode
                    : _consoleApp.DefaultExitCode);
        }

        private void OnStarted(CancellationToken cancellationToken)
        {
            // The token passed to a hosted service is the host's startup token: Host.StartAsync links it
            // against ApplicationStopping but disposes that linked source as soon as ApplicationStarted fires,
            // severing the registration. By the time the console app runs the token is frozen and would never
            // cancel on Ctrl+C/SIGTERM. Link against ApplicationStopping ourselves and hold the source for the
            // whole command, so the token handed to StartAsync genuinely cancels on shutdown. Without this, an
            // app that trusts the StartAsync token never observes shutdown and the bounded wait in StopAsync
            // has to time out (e.g. a Ctrl+C during a blocking wait delays exit by the full timeout).
            var shutdownTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken, _applicationLifetime.ApplicationStopping);

            var shutdownToken = shutdownTokenSource.Token;

            _consoleAppTask = Task.Run(async () =>
            {
                try
                {
                    await _consoleApp.StartAsync(shutdownToken);
                    _exitCode = _consoleApp.ExitCode;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception!");
                    _exitCode = _consoleApp.UnhandedErrorExitCode;
                }
                finally
                {
                    shutdownTokenSource.Dispose();

                    // Stop the application once the work is done
                    _applicationLifetime.StopApplication();
                }
            }, shutdownToken);
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