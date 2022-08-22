using AllOverIt.Patterns.ResourceInitialization;
using System;

namespace AllOverIt.Threading.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IReadWriteLock"/> types.</summary>
    public static class ReadWriteLockExtensions
    {
        /// <summary>Gets a read lock that will auto-release when disposed.</summary>
        /// <param name="lock">The lock to obtain a read lock.</param>
        /// <param name="upgradeable">Indicates if the read lock can be upgraded to a write lock.</param>
        /// <returns>A disposable that will release the lock when disposed.</returns>
        public static IDisposable GetReadLock(this IReadWriteLock @lock, bool upgradeable)
        {
            return new Raii(() => @lock.EnterReadLock(upgradeable), @lock.ExitReadLock);
        }

        /// <summary>Gets a write lock that will auto-release when disposed.</summary>
        /// <param name="lock">The lock to obtain a write lock.</param>
        /// <returns>A disposable that will release the lock when disposed.</returns>
        public static IDisposable GetWriteLock(this IReadWriteLock @lock)
        {
            return new Raii(@lock.EnterWriteLock, @lock.ExitWriteLock);
        }
    }
}