using AllOverIt.Assertion;
using Microsoft.Extensions.Hosting;

namespace AllOverIt.GenericHost
{
    public abstract class ConsoleWorker : BackgroundService
    {
        protected ConsoleWorker(IHostApplicationLifetime applicationLifetime)
        {
            _ = applicationLifetime.WhenNotNull(nameof(applicationLifetime));

            applicationLifetime.ApplicationStopping.Register(OnStopping);
            applicationLifetime.ApplicationStopped.Register(OnStopped);
        }

        protected virtual void OnStopping()
        {
        }

        protected virtual void OnStopped()
        {
        }
    }
}