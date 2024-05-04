using System.Collections;

namespace AllOverIt.Diagnostics.Breadcrumbs
{
    /// <summary>Provides an instance of <see cref="IBreadcrumbs"/> without behavior.</summary>
    public sealed class NullBreadcrumbs : IBreadcrumbs
    {
        /// <summary>Returns a static instance of <see cref="NullBreadcrumbs"/>.</summary>
        public static readonly IBreadcrumbs Instance = new NullBreadcrumbs();

        /// <summary>Does nothing. Implemented as a null-object.</summary>
        public bool Enabled { get; set; }

        /// <summary>Does nothing. Implemented as a null-object.</summary>
        public DateTime StartTimestamp { get; }

        /// <summary>Does nothing. Implemented as a null-object.</summary>
        public void Add(BreadcrumbData breadcrumb)
        {
        }

        /// <summary>Does nothing. Implemented as a null-object.</summary>
        public void Clear()
        {
        }

        /// <summary>Does nothing. Implemented as a null-object.</summary>
        public void Reset()
        {
        }

        /// <summary>Does nothing. Implemented as a null-object.</summary>
        public IEnumerator<BreadcrumbData> GetEnumerator()
        {
            return Enumerable.Empty<BreadcrumbData>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
