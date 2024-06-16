using AllOverIt.Assertion;
using AllOverIt.Threading;
using AllOverIt.Threading.Extensions;
using System.Collections;

namespace AllOverIt.Collections
{
    /// <summary>A general-purpose list, or list decorator, that applies a lock around each method call as well as
    /// providing the ability to be globally locked for read or write access. A global read lock can be optionally
    /// defined as upgradeable to a write lock.</summary>
    /// <typeparam name="TType">The object type help by the list.</typeparam>
    /// <remarks>This lockable list is not as comprehensive as the .NET provided concurrent collections. Refer to
    /// https://github.com/dotnet/runtime/issues/41740 for a discussion on potential issues, such as race conditions.</remarks>
    public class LockableList<TType> : ILockableList<TType>
    {
        private sealed class LockedListEnumerator : IEnumerator<TType>
        {
            private readonly IEnumerator<TType> _enumerator;
            private readonly IReadWriteLock _lock;

            TType IEnumerator<TType>.Current => _enumerator.Current;
            object IEnumerator.Current => ((IEnumerator) _enumerator).Current;

            public LockedListEnumerator(IList<TType> list, IReadWriteLock @lock)
            {
                @lock.EnterReadLock(false);

                _enumerator = list.GetEnumerator();
                _lock = @lock;
            }

            bool IEnumerator.MoveNext() => _enumerator.MoveNext();
            void IEnumerator.Reset() => _enumerator.Reset();

            void IDisposable.Dispose()
            {
                _lock.ExitReadLock();
            }
        }

        private bool _disposed;
        private readonly IReadWriteLock _lock;
        internal readonly IList<TType> _list;

        /// <inheritdoc />
        public TType this[int index]
        {
            get
            {
                using (_lock.GetReadLock(false))
                {
                    return _list[index];
                }
            }

            set
            {
                using (_lock.GetWriteLock())
                {
                    _list[index] = value;
                }
            }
        }

        /// <inheritdoc />
        public int Count
        {
            get
            {
                using (_lock.GetReadLock(false))
                {
                    return _list.Count;
                }
            }
        }

        /// <inheritdoc />
        public bool IsReadOnly
        {
            get
            {
                using (_lock.GetReadLock(false))
                {
                    return _list.IsReadOnly;
                }
            }
        }

        /// <inheritdoc />
        public LockableList(bool supportsLockRecursion)
            : this([], CreateReadWriteLock(supportsLockRecursion))
        {
        }

        /// <inheritdoc />
        public LockableList(IList<TType> list, bool supportsLockRecursion)
            : this(list, CreateReadWriteLock(supportsLockRecursion))
        {
        }

        internal LockableList(IList<TType> list, IReadWriteLock @lock)
        {
            _list = list.WhenNotNull(nameof(list));
            _lock = @lock;
        }

        /// <inheritdoc />
        public void Add(TType item)
        {
            using (_lock.GetWriteLock())
            {
                _list.Add(item);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            using (_lock.GetWriteLock())
            {
                _list.Clear();
            }
        }

        /// <inheritdoc />
        public bool Contains(TType item)
        {
            using (_lock.GetReadLock(false))
            {
                return _list.Contains(item);
            }
        }

        /// <inheritdoc />
        public void CopyTo(TType[] array, int arrayIndex)
        {
            using (_lock.GetReadLock(false))
            {
                _list.CopyTo(array, arrayIndex);
            }
        }

        /// <inheritdoc />
        public IEnumerator<TType> GetEnumerator()
        {
            return new LockedListEnumerator(_list, _lock);
        }

        /// <inheritdoc />
        public int IndexOf(TType item)
        {
            using (_lock.GetReadLock(false))
            {
                return _list.IndexOf(item);
            }
        }

        /// <inheritdoc />
        public void Insert(int index, TType item)
        {
            using (_lock.GetWriteLock())
            {
                _list.Insert(index, item);
            }
        }

        /// <inheritdoc />
        public bool Remove(TType item)
        {
            using (_lock.GetWriteLock())
            {
                return _list.Remove(item);
            }
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            using (_lock.GetWriteLock())
            {
                _list.RemoveAt(index);
            }
        }

        /// <inheritdoc />
        public IDisposable GetReadLock(bool upgradeable)
        {
            return _lock.GetReadLock(upgradeable);
        }

        /// <inheritdoc />
        public IDisposable GetWriteLock()
        {
            return _lock.GetWriteLock();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _lock.Dispose();
                }

                _disposed = true;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static ReadWriteLock CreateReadWriteLock(bool supportsLockRecursion)
        {
            return new ReadWriteLock(supportsLockRecursion ? LockRecursionPolicy.SupportsRecursion : LockRecursionPolicy.NoRecursion);
        }
    }
}