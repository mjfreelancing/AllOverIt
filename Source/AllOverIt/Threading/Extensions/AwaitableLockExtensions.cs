using System;
using System.Threading;
using System.Threading.Tasks;
using AllOverIt.Patterns.ResourceInitialization;

namespace AllOverIt.Threading.Extensions
{
    // TODO: Add tests

    /// <summary>Provides a variety of extension methods for <see cref="IAwaitableLock"/> types.</summary>
    public static class AwaitableLockExtensions
    {
        /// <summary>Asynchronously waits to obtain a lock that is automatically released when disposed.</summary>
        /// <param name="lock">The lock to wait for.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="Task"/> that returns a disposable when the lock is obtained. The lock is released
        /// when the disposable is disposed.</returns>
        public static async Task<IDisposable> GetLockAsync(this IAwaitableLock @lock, CancellationToken cancellationToken = default)
        {
            await @lock.LockAsync(cancellationToken).ConfigureAwait(false);

            return new Raii(() => { }, () => @lock.Release());
        }
    }
}