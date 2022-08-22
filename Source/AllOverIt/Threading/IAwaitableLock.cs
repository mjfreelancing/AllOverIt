using System;
using System.Threading.Tasks;
using System.Threading;

namespace AllOverIt.Threading
{
    /// <summary>Represents a lock that limits the number of threads that can access a resource within an asynchronous context.</summary>
    public interface IAwaitableLock : IDisposable
    {
        /// <summary>Asynchronously waits indefinitely to obtain the lock.</summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="Task"/> that will complete when the lock has been obtained.</returns>
        Task EnterLockAsync(CancellationToken cancellationToken);

        /// <summary>Tries to asynchronously wait until the lock is obtained.</summary>
        /// <param name="milliseconds">The timeout period in milliseconds to wait. Use a value of -1 to wait indefinitely.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task that will complete with a result of true if the current thread successfully 
        /// obtained the lock, otherwise with a result of false.</returns>
        Task<bool> TryEnterLockAsync(int milliseconds, CancellationToken cancellationToken);

        /// <summary>Tries to asynchronously wait until the lock is obtained.</summary>
        /// <param name="timeSpan">The <see cref="TimeSpan"/> to wait. Use a value of -1 to wait indefinitely.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task that will complete with a result of true if the current thread successfully
        ///  obtained the lock, otherwise with a result of false.</returns>
        Task<bool> TryEnterLockAsync(TimeSpan timeSpan, CancellationToken cancellationToken);

        /// <summary>Releases the lock held by the current thread. This must only be called if the thread previously
        /// obtained the lock.</summary>
        void ExitLock();
    }
}