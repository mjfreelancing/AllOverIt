using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.Threading
{
    /// <summary>Implements a read/write lock that performs no locking actions (null object pattern).</summary>
    /// <remarks>Use this class where a non-blocking locking policy is required for use within a single thread.</remarks>
    [ExcludeFromCodeCoverage]
    public sealed class NoLock : IReadWriteLock
    {
        /// <summary>Performs no action.</summary>
        /// <param name="upgradeable">Ignored.</param>
        public void EnterReadLock(bool upgradeable)
        {
        }

        /// <summary>Performs no action.</summary>
        /// <param name="upgradeable">Ignored.</param>
        /// <param name="millisecondsTimeout">Ignored.</param>
        /// <returns>Always returns true.</returns>
        public bool TryEnterReadLock(bool upgradeable, int millisecondsTimeout)
        {
            return true;
        }

        /// <summary>Performs no action.</summary>
        public void ExitReadLock()
        {
        }

        /// <summary>Performs no action.</summary>
        public void EnterWriteLock()
        {
        }

        /// <summary>Performs no action.</summary>
        /// <param name="millisecondsTimeout">Ignored.</param>
        public bool TryEnterWriteLock(int millisecondsTimeout)
        {
            return true;
        }

        /// <summary>Performs no action.</summary>
        public void ExitWriteLock()
        {
        }

        /// <summary>Performs no action.</summary>
        public void Dispose()
        {
        }
    }
}
