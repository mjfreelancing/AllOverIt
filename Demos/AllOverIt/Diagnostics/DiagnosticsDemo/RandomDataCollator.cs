using AllOverIt.Assertion;
using AllOverIt.Diagnostics.Breadcrumbs;
using System.Collections;

namespace DiagnosticsDemo
{
    // Provides a custom enumerator so consecutive items with the same Tag have their metadata concatenated
    internal sealed class RandomDataCollator : IEnumerable<BreadcrumbData>
    {
        private sealed class RandomDataEnumerator : IEnumerator<BreadcrumbData>
        {
            private readonly IEnumerator<BreadcrumbData> _breadcrumbEnumerator;
            private readonly Func<object, object, object> _collator;

            private BreadcrumbData _current;
            private BreadcrumbData _next;

            private object _collatedData;

            object IEnumerator.Current => _current;

            public BreadcrumbData Current => _current;
            public BreadcrumbData Next => _next;

            public RandomDataEnumerator(IBreadcrumbs breadcrumbs, Func<object, object, object> collator)
            {
                _breadcrumbEnumerator = breadcrumbs.GetEnumerator();
                _collator = collator;
            }

            bool IEnumerator.MoveNext()
            {
                var canMove = _breadcrumbEnumerator.MoveNext();

                CollateRandomData();

                return canMove;
            }

            void IEnumerator.Reset()
            {
                _breadcrumbEnumerator.Reset();

                CollateRandomData();
            }

            void IDisposable.Dispose()
            {
            }

            private void CollateRandomData()
            {
                _current = _breadcrumbEnumerator.Current;
                _collatedData = null;

                var current = _current;
                var currentTag = current?.Tag;

                while (current?.Tag is not null && current.Tag == currentTag)
                {
                    _collatedData = _collator.Invoke(_collatedData, current.Metadata);

                    _breadcrumbEnumerator.MoveNext();

                    current = _breadcrumbEnumerator.Current;
                }

                if (!ReferenceEquals(current, _current))
                {
                    _current = _current with { Metadata = _collatedData };
                }

                _next = _breadcrumbEnumerator.MoveNext()
                    ? _breadcrumbEnumerator.Current
                    : null;
            }
        }

        private readonly IBreadcrumbs _breadcrumbs;
        private readonly Func<object, object, object> _collator;

        // The func inputs are 'currently collated elements', 'next element to be collated',
        // and returns the resultant collated data.
        public RandomDataCollator(IBreadcrumbs breadcrumbs, Func<object, object, object> collator)
        {
            _breadcrumbs = breadcrumbs.WhenNotNull();
            _collator = collator.WhenNotNull();
        }

        IEnumerator<BreadcrumbData> IEnumerable<BreadcrumbData>.GetEnumerator()
        {
            return new RandomDataEnumerator(_breadcrumbs, _collator);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<BreadcrumbData>) this).GetEnumerator();
        }
    }
}