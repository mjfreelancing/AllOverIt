using System.Collections.Generic;

namespace AllOverIt.Diagnostics.Breadcrumbs
{
    /// <summary>Represents a collection of breadcrumb messages and metadata.</summary>
    public interface IBreadcrumbs : IEnumerable<BreadcrumbData>
    {
        /// <summary>Adds a message to the collection of breadcrumbs.</summary>
        /// <param name="message">The message to be added.</param>
        void Add(string message);

        /// <summary>Adds a message and related metadata to the collection of breadcrumbs.</summary>
        /// <param name="message">The message to be added.</param>
        /// <param name="metadata">Metadata associated with the message.</param>
        void Add(string message, object metadata);
    }
}
