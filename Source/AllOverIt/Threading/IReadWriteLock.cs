﻿using System;

namespace AllOverIt.Threading
{
    /// <summary>An interface representing a lock that is used to manage access to a resource, allowing multiple threads
    /// for reading or exclusive access for writing.</summary>
    public interface IReadWriteLock : IDisposable
    {
        /// <summary>Blocks the calling thread until a read lock is acquired.</summary>
        /// <param name="upgradeable">Indicates if the read lock can be upgraded to a write lock.</param>
        void EnterReadLock(bool upgradeable);

        /// <summary>Blocks the calling thread while trying to acquire a read lock within the specified timeout period.</summary>
        /// <param name="upgradeable">Indicates if the read lock can be upgraded to a write lock.</param>
        /// <param name="millisecondsTimeout">The timeout period to block the calling thread while trying to acquire the read lock.
        /// A value of 0 indicates the method should return if the lock cannot be immediately acquired.
        /// A value of -1 will result in the calling thread being blocked indefinitely, until the lock is acquired.</param>
        /// <returns>True if the lock was acquired, otherwise false.</returns>
        bool TryEnterReadLock(bool upgradeable, int millisecondsTimeout);

        /// <summary>Exists a previously acquired read lock.</summary>
        void ExitReadLock();

        /// <summary>Blocks the calling thread until a write lock is acquired.</summary>
        void EnterWriteLock();

        /// <summary>Blocks the calling thread while trying to acquire a write lock within the specified timeout period.</summary>
        /// <param name="millisecondsTimeout">The timeout period to block the calling thread while trying to acquire the write lock.
        /// A value of 0 indicates the method should return if the lock cannot be immediately acquired.
        /// A value of -1 will result in the calling thread being blocked indefinitely, until the lock is acquired.</param>
        /// <returns>True if the lock was acquired, otherwise false.</returns>
        bool TryEnterWriteLock(int millisecondsTimeout);

        /// <summary>Exists a previously acquired write lock.</summary>
        void ExitWriteLock();
    }
}