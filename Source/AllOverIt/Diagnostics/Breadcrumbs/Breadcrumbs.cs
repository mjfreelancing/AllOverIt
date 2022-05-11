using AllOverIt.Assertion;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AllOverIt.Diagnostics.Breadcrumbs
{
    /// <summary>Stores a collection of breadcrumb messages and metadata.</summary>
    public sealed class Breadcrumbs : IBreadcrumbs
    {
        private readonly IList<BreadcrumbData> _breadcrumbs = new List<BreadcrumbData>();

        /// <summary>The timestamp when breadcrumb collection begins.</summary>
        public DateTime StartTimestamp { get; } = DateTime.Now;

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
