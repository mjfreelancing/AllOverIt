using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace AllOverIt.Threading
{
    // Internally, this class uses a default constructed ReaderWriterLockSlim object to perform all locking operations.
    // Refer to http://msdn.microsoft.com/en-us/library/system.threading.readerwriterlockslim(v=vs.110).aspx for more
    // information on the semantics of this lock type.

    /// <summary>Represents a lock that is used to manage access to a resource, allowing multiple threads for reading or
    /// exclusive access for writing. This cannot be used with async methods.</summary>
    [ExcludeFromCodeCoverage]
    public sealed class ReadWriteLock : IReadWriteLock
    {
        // Many threads can enter the read lock simultaneously.
        // Only one thread can enter an upgradeable lock but other threads can still enter a read lock.
        // Only one thread can enter a write lock and meanwhile no other thread can enter any other lock type.
        private ReaderWriterLockSlim _slimLock;

        /// <summary>Constructor.</summary>
        /// <remarks>Defaults to a non-recursive lock policy.</remarks>
        public ReadWriteLock()
            : this(LockRecursionPolicy.NoRecursion)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="recursionPolicy">Determines if a lock can be entered multiple times by the same thread.</param>
        public ReadWriteLock(LockRecursionPolicy recursionPolicy)
        {
            _slimLock = new ReaderWriterLockSlim(recursionPolicy);
        }

        /// <inheritdoc />
        public void EnterReadLock(bool upgradeable)
        {
            if (upgradeable)
            {
                _slimLock.EnterUpgradeableReadLock();
            }
            else
            {
                _slimLock.EnterReadLock();
            }
        }

        /// <inheritdoc />
        public bool TryEnterReadLock(bool upgradeable, int millisecondsTimeout)
        {
            return upgradeable
                ? _slimLock.TryEnterUpgradeableReadLock(millisecondsTimeout)
                : _slimLock.TryEnterReadLock(millisecondsTimeout);
        }

        /// <inheritdoc />
        public bool TryEnterReadLock(bool upgradeable, TimeSpan timeout)
        {
            return upgradeable
                ? _slimLock.TryEnterUpgradeableReadLock(timeout)
                : _slimLock.TryEnterReadLock(timeout);
        }

        /// <inheritdoc />
        public bool TryEnterUpgradeableReadLock(int millisecondsTimeout)
        {
            return _slimLock.TryEnterUpgradeableReadLock(millisecondsTimeout);
        }

        /// <inheritdoc />
        public bool TryEnterUpgradeableReadLock(TimeSpan timeout)
        {
            return _slimLock.TryEnterUpgradeableReadLock(timeout);
        }

        /// <inheritdoc />
        public void ExitReadLock()
        {
            if (_slimLock.IsUpgradeableReadLockHeld)
            {
                _slimLock.ExitUpgradeableReadLock();
            }
            else
            {
                _slimLock.ExitReadLock();
            }
        }

        /// <inheritdoc />
        public void EnterWriteLock()
        {
            _slimLock.EnterWriteLock();
        }

        /// <inheritdoc />
        public bool TryEnterWriteLock(int millisecondsTimeout)
        {
            return _slimLock.TryEnterWriteLock(millisecondsTimeout);
        }

        /// <inheritdoc />
        public bool TryEnterWriteLock(TimeSpan timeout)
        {
            return _slimLock.TryEnterWriteLock(timeout);
        }

        /// <inheritdoc />
        public void ExitWriteLock()
        {
            _slimLock.ExitWriteLock();
        }

        /// <summary>Disposes of the internal lock.</summary>
        public void Dispose()
        {
            _slimLock?.Dispose();
            _slimLock = null;
        }
    }
}