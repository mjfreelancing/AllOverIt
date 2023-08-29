using AllOverIt.Assertion;
using AllOverIt.Threading;
using AllOverIt.Threading.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AllOverIt.Diagnostics.Breadcrumbs
{
    /// <inheritdoc cref="IBreadcrumbs" />
    public sealed class Breadcrumbs : IBreadcrumbs
    {
        private interface IEnumerableWrapper
        {
            void Add(BreadcrumbData breadcrumb);
            void Clear();
            IEnumerator<BreadcrumbData> GetEnumerator();
        }

        private sealed class SingleThreadListWrapper : IEnumerableWrapper
        {
            private readonly List<BreadcrumbData> _breadcrumbs = new();
            private readonly int _maxCapactiy;

            public SingleThreadListWrapper(BreadcrumbsOptions options)
            {
                _maxCapactiy = options.MaxCapacity;
            }

            public void Add(BreadcrumbData breadcrumb)
            {
                _breadcrumbs.Add(breadcrumb);

                if (_maxCapactiy > 0 && _breadcrumbs.Count > _maxCapactiy)
                {
                    _breadcrumbs.RemoveRange(0, _breadcrumbs.Count - _maxCapactiy);
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
            private readonly SortedList<long, BreadcrumbData> _breadcrumbs;
            private readonly object _syncRoot;
            private readonly int _maxCapactiy;

            public MultiThreadListWrapper(BreadcrumbsOptions options)
            {
                _maxCapactiy = options.MaxCapacity;
                _breadcrumbs = new SortedList<long, BreadcrumbData>();
                _syncRoot = ((ICollection)_breadcrumbs).SyncRoot;
            }

            public void Add(BreadcrumbData breadcrumb)
            {
                lock (_syncRoot)
                {
                    _breadcrumbs.Add(breadcrumb.Counter, breadcrumb);

                    if (_maxCapactiy > 0)
                    {
                        while (_breadcrumbs.Count > _maxCapactiy)
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
                lock (_syncRoot)
                {
                    using (var iterator = _breadcrumbs.GetEnumerator())
                    {
                        while (iterator.MoveNext())
                        {
                            yield return iterator.Current.Value;
                        }
                    }                   
                }
            }
        }

        private readonly IEnumerableWrapper _breadcrumbs;
        private readonly IReadWriteLock _readWriteLock;
        private bool _enabled;
        private DateTime _startTimestamp;

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

        /// <summary>Constructor.</summary>
        /// <param name="options">Provides options that control how breadcrumb items are inserted and cached.</param>
        public Breadcrumbs(BreadcrumbsOptions options = default)
        {
            Options = options ?? new BreadcrumbsOptions();

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
