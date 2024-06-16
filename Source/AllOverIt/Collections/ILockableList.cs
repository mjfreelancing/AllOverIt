namespace AllOverIt.Pipes.Named.Server
{
    /// <summary>Represents a lockable list of objects that can be access by index. The list methods are individually
    /// locked and the list can be globally locked for read or write access. A global read lock can be optionally defined
    /// as upgradeable to a write lock.</summary>
    /// <typeparam name="TType">The object type help by the list.</typeparam>
    public interface ILockableList<TType> : IList<TType>, IDisposable
    {
        /// <summary>Gets a read lock that will auto-release when disposed. The lock supports recursion, but it is not <c>async</c> aware.</summary>
        /// <param name="upgradeable">Indicates if the read lock can be upgraded to a write lock.</param>
        /// <returns>A disposable that will release the lock when disposed.</returns>
        IDisposable GetReadLock(bool upgradeable);

        /// <summary>Gets a write lock that will auto-release when disposed. The lock supports recursion, but it is not <c>async</c> aware.</summary>
        /// <returns>A disposable that will release the lock when disposed.</returns>
        IDisposable GetWriteLock();
    }
}