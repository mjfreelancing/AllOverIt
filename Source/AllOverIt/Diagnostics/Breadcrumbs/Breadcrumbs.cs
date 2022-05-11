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
        public void Add(string message)
        {
            _ = message.WhenNotNullOrEmpty(nameof(message));

            Add(message, null);
        }

        /// <inheritdoc />
        public void Add(string message, object metadata)
        {
            _ = message.WhenNotNullOrEmpty(nameof(message));

            var breadcrumb = new BreadcrumbData
            {

                Timestamp = DateTime.Now,
                Message = message,
                Metadata = metadata
            };

            _breadcrumbs.Add(breadcrumb);
        }
    }
}
