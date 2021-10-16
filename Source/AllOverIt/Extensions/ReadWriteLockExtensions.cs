using AllOverIt.Patterns.ResourceInitialization;
using AllOverIt.Threading;
using System;
using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ReadWriteLockExtensions
    {
        public static IDisposable GetReadLock(this IReadWriteLock @lock, bool upgradeable)
        {
            return new Raii(() => @lock.EnterReadLock(upgradeable), @lock.ExitReadLock);
        }

        public static IDisposable GetWriteLock(this IReadWriteLock @lock)
        {
            return new Raii(@lock.EnterWriteLock, @lock.ExitWriteLock);
        }
    }
}