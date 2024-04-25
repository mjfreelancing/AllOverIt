using System;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Threading
{
    /// <inheritdoc cref="IAwaitableLock" />
    public sealed class AwaitableLock : IAwaitableLock
    {
        private bool _disposed;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        /// <inheritdoc />
        public Task EnterLockAsync(CancellationToken cancellationToken)
        {
            return _semaphore.WaitAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> TryEnterLockAsync(int milliseconds, CancellationToken cancellationToken)
        {
            return _semaphore.WaitAsync(milliseconds, cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> TryEnterLockAsync(TimeSpan timeSpan, CancellationToken cancellationToken)
        {
            return _semaphore.WaitAsync(timeSpan, cancellationToken);
        }

        /// <inheritdoc />
        public void ExitLock()
        {
            try
            {
                _semaphore.Release();
            }
            catch (SemaphoreFullException exception)
            {
                throw new SynchronizationLockException("The lock is not currently held.", exception);
            }
        }

        /// <summary>Disposes of the managed resources.</summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _semaphore.Dispose();
                _disposed = true;           // avoiding making _disposed nullable
            }
        }
    }
}