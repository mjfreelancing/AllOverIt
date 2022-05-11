using System.Collections.Generic;

namespace AllOverIt.Diagnostics.Breadcrumbs
{
    /// <summary>Represents a collection of breadcrumb messages and metadata.</summary>
    public interface IBreadcrumbs : IEnumerable<BreadcrumbData>
    {
        /// <summary>Adds a new breadcrumb data item.</summary>
        /// <param name="breadcrumb">The breadcrumb data item.</param>
        /// <returns>The same breadcrumb instance to allow for a fluent syntax.</returns>
        IBreadcrumbs Add(BreadcrumbData breadcrumb);
    }
}
