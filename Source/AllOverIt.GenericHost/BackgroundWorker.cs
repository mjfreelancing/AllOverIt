using System.Threading.Tasks;
using AllOverIt.Assertion;
using Microsoft.Extensions.Hosting;

namespace AllOverIt.GenericHost
{
    /// <summary>A base class for implementing a long running hosted service that is notified when the application is started, stopping and stopped.</summary>
    public abstract class BackgroundWorker : BackgroundService
    {
        private readonly Task<bool> _startupCompletionTask;

        /// <summary>Constructor.</summary>
        /// <param name="applicationLifetime">Provides notification of lifetime events.</param>
        protected BackgroundWorker(IHostApplicationLifetime applicationLifetime)
        {
            _ = applicationLifetime.WhenNotNull(nameof(applicationLifetime));

            applicationLifetime.ApplicationStarted.Register(OnStarted);
            applicationLifetime.ApplicationStopping.Register(OnStopping);
            applicationLifetime.ApplicationStopped.Register(OnStopped);

            _startupCompletionTask = CreateStartedCompletionTask(applicationLifetime);
        }

        /// <summary>This method is called when the application's lifetime 'Started' event is triggered.</summary>
        /// <remarks>Note, this is called after the <see cref="BackgroundService.ExecuteAsync"/> method is called.</remarks>
        protected virtual void OnStarted()
        {
        }

        /// <summary>This method is called when the application's lifetime 'Stopping' event is triggered.</summary>
        protected virtual void OnStopping()
        {
        }

        /// <summary>This method is called when the application's lifetime 'Stopped' event is triggered.</summary>
        protected virtual void OnStopped()
        {
        }

        /// <summary>Waits for the application to start, or enter the 'Stopping' state if the application fails to start.</summary>
        /// <returns>True if the application has completely started or False if the application is stopping after failing to start.</returns>
        /// <remarks>Calling this method at the start of the overriden <see cref="BackgroundService.ExecuteAsync"/> method is useful
        /// to ensure the application has completely started before continuing.</remarks>
        protected async Task<bool> WaitForStartup()
        {
            return await _startupCompletionTask;
        }

        private static Task<bool> CreateStartedCompletionTask(IHostApplicationLifetime applicationLifetime)
        {
            var started = new TaskCompletionSource<bool>();
            var cancelled = new TaskCompletionSource<bool>();

            applicationLifetime.ApplicationStarted.Register(() => started.SetResult(true));
            applicationLifetime.ApplicationStopping.Register(() => cancelled.SetResult(false));

            return Task.WhenAny(started.Task, cancelled.Task).Unwrap();
        }
    }
}