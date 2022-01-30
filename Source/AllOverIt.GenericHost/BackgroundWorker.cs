using AllOverIt.Assertion;
using Microsoft.Extensions.Hosting;

namespace AllOverIt.GenericHost
{
    /// <summary>A base class for implementing a long running hosted service that is notified when the application is stopping and stopped.</summary>
    public abstract class BackgroundWorker : BackgroundService
    {
        /// <summary>Constructor.</summary>
        /// <param name="applicationLifetime">Provides notification of lifetime events.</param>
        protected BackgroundWorker(IHostApplicationLifetime applicationLifetime)
        {
            _ = applicationLifetime.WhenNotNull(nameof(applicationLifetime));

            applicationLifetime.ApplicationStarted.Register(OnStarted);
            applicationLifetime.ApplicationStopping.Register(OnStopping);
            applicationLifetime.ApplicationStopped.Register(OnStopped);
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
    }
}