using AllOverIt.Assertion;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AllOverIt.Diagnostics.Breadcrumbs
{
    /// <summary>Stores a collection of breadcrumb messages and metadata.</summary>
    public sealed class Breadcrumbs : IBreadcrumbs
    {
        private interface IEnumerableWrapper
        {
            void Add(BreadcrumbData breadcrumb);
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
                _syncRoot = ((ICollection) _breadcrumbs).SyncRoot;
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

            public IEnumerator<BreadcrumbData> GetEnumerator()
            {
                lock (_syncRoot)
                {
                    var iterator = _breadcrumbs.GetEnumerator();

                    while (iterator.MoveNext())
                    {
                        yield return iterator.Current.Value;
                    }
                }
            }
        }

        private readonly IEnumerableWrapper _breadcrumbs;

        /// <summary>Provides options that control how breadcrumb items are inserted and cached.</summary>
        public BreadcrumbsOptions Options { get; }

        /// <summary>The timestamp when breadcrumb collection begins.</summary>
        public DateTime StartTimestamp { get; } = DateTime.Now;

        /// <summary>Constructor.</summary>
        /// <param name="options">Provides options that control how breadcrumb items are inserted and cached.</param>
        public Breadcrumbs(BreadcrumbsOptions options = default)
        {
            Options = options ?? new BreadcrumbsOptions();

            _breadcrumbs = Options.ThreadSafe
                ? new MultiThreadListWrapper(Options)
                : new SingleThreadListWrapper(Options);
        }

        /// <inheritdoc />
        public IEnumerator<BreadcrumbData> GetEnumerator()
        {
            return _breadcrumbs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public IBreadcrumbs Add(BreadcrumbData breadcrumb)
        {
            _ = breadcrumb.WhenNotNull(nameof(breadcrumb));

            _breadcrumbs.Add(breadcrumb);

            return this;
        }
    }
}
