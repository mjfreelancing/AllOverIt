using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.GenericHost
{
    public interface IGenericApp
    {
        /// <summary>The entry point to the console application after the application host has fully started.</summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>Triggered when the application host is performing a graceful shutdown. Shutdown will block until
        /// this event completes.</summary>
        public void OnStopping();

        /// <summary>Triggered when the application host is performing a graceful shutdown. Shutdown will block until
        /// this event completes.</summary>
        public void OnStopped();
    }
}

