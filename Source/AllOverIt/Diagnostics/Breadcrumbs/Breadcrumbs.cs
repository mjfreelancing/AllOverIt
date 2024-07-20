using AllOverIt.Assertion;
using AllOverIt.Threading;
using AllOverIt.Threading.Extensions;
using System.Collections;

namespace AllOverIt.Diagnostics.Breadcrumbs
{
    /// <inheritdoc cref="IBreadcrumbs" />
    public sealed class Breadcrumbs : IBreadcrumbs
    {
        private interface IEnumerableWrapper
        {
            int Count { get; }
            void Add(BreadcrumbData breadcrumb);
            void Clear();
            IEnumerator<BreadcrumbData> GetEnumerator();
        }

        private sealed class SingleThreadListWrapper : IEnumerableWrapper
        {
            private readonly List<BreadcrumbData> _breadcrumbs = [];
            private readonly int _maxCapacity;

            public int Count => _breadcrumbs.Count;

            public SingleThreadListWrapper(BreadcrumbsOptions options)
            {
                _maxCapacity = options.MaxCapacity;
            }

            public void Add(BreadcrumbData breadcrumb)
            {
                _breadcrumbs.Add(breadcrumb);

                if (_maxCapacity > 0 && _breadcrumbs.Count > _maxCapacity)
                {
                    _breadcrumbs.RemoveRange(0, _breadcrumbs.Count - _maxCapacity);
                }
            }

            public void Clear()
            {
                _breadcrumbs.Clear();
            }

            public IEnumerator<BreadcrumbData> GetEnumerator()
            {
                return _breadcrumbs.GetEnumerator();
            }
        }

        private sealed class MultiThreadListWrapper : IEnumerableWrapper
        {
            private readonly SortedList<long, BreadcrumbData> _breadcrumbs = [];
            private readonly object _syncRoot;
            private readonly int _maxCapacity;

            public int Count
            {
                get
                {
                    lock (_syncRoot)
                    {
                        return _breadcrumbs.Count;
                    }
                }
            }

            public MultiThreadListWrapper(BreadcrumbsOptions options)
            {
                _maxCapacity = options.MaxCapacity;
                _syncRoot = ((ICollection) _breadcrumbs).SyncRoot;
            }

            public void Add(BreadcrumbData breadcrumb)
            {
                lock (_syncRoot)
                {
                    _breadcrumbs.Add(breadcrumb.Counter, breadcrumb);

                    if (_maxCapacity > 0)
                    {
                        while (_breadcrumbs.Count > _maxCapacity)
                        {
                            _breadcrumbs.RemoveAt(0);
                        }
                    }
                }
            }

            public void Clear()
            {
                lock (_syncRoot)
                {
                    _breadcrumbs.Clear();
                }
            }

            public IEnumerator<BreadcrumbData> GetEnumerator()
            {
                List<BreadcrumbData> clone = [];

                // Can't yield within a lock, so taking a snapshot and returning it's enumerator
                lock (_syncRoot)
                {
                    clone.AddRange(_breadcrumbs.Values);
                }

                return clone.GetEnumerator();
            }
        }

        private readonly IEnumerableWrapper _breadcrumbs;
        private readonly IReadWriteLock _readWriteLock;
        private bool _enabled;
        private DateTime _startTimestamp;

        /// <inheritdoc />
        public int Count => _breadcrumbs.Count;

        /// <inheritdoc />
        public bool Enabled
        {
            get
            {
                using (_readWriteLock.GetReadLock(false))
                {
                    return _enabled;
                }
            }

            set
            {
                using (_readWriteLock.GetWriteLock())
                {
                    _enabled = value;
                }
            }
        }

        /// <inheritdoc />
        public BreadcrumbsOptions Options { get; }

        /// <inheritdoc />
        public DateTime StartTimestamp
        {
            get
            {
                using (_readWriteLock.GetReadLock(false))
                {
                    return _startTimestamp;
                }
            }

            private set
            {
                using (_readWriteLock.GetWriteLock())
                {
                    _startTimestamp = value;
                }
            }
        }

        /// <summary>Constructor. Applies a default constructed <see cref="BreadcrumbsOptions"/>.</summary>
        public Breadcrumbs()
            : this(new BreadcrumbsOptions())
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="options">Provides options that control how breadcrumb items are inserted and cached.</param>
        public Breadcrumbs(BreadcrumbsOptions options)
        {
            Options = options.WhenNotNull(nameof(options));

            _breadcrumbs = Options.ThreadSafe
                ? new MultiThreadListWrapper(Options)
                : new SingleThreadListWrapper(Options);

            _readWriteLock = Options.ThreadSafe
                ? new ReadWriteLock()
                : new NoLock();

            Enabled = Options.StartEnabled;
            StartTimestamp = DateTime.Now;
        }

        /// <inheritdoc />
        public IEnumerator<BreadcrumbData> GetEnumerator()
        {
            return _breadcrumbs.GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(BreadcrumbData breadcrumb)
        {
            _ = breadcrumb.WhenNotNull(nameof(breadcrumb));

            if (Enabled)
            {
                _breadcrumbs.Add(breadcrumb);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            _breadcrumbs.Clear();
        }

        /// <inheritdoc />
        public void Reset()
        {
            Clear();
            StartTimestamp = DateTime.Now;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
