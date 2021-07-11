using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.Threading
{
    // Implements a read/write lock that performs no locking actions (null object pattern).
    // Use this class where a non-blocking locking policy is required for use within a single thread.
    [ExcludeFromCodeCoverage]
    public sealed class NoLock : IReadWriteLock
    {
        public void EnterReadLock(bool upgradeable)
        {
        }

        public bool TryEnterReadLock(bool upgradeable, int millisecondsTimeout)
        {
            return true;
        }

        public void ExitReadLock()
        {
        }

        public void EnterWriteLock()
        {
        }

        public bool TryEnterWriteLock(int millisecondsTimeout)
        {
            return true;
        }

        public void ExitWriteLock()
        {
        }

        public void Dispose()
        {
        }
    }
}
