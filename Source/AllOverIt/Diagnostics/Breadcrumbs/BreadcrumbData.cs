using System;

namespace AllOverIt.Diagnostics.Breadcrumbs
{
    /// <summary>Contains breadcrumb information at a point in time.</summary>
    public sealed class BreadcrumbData
    {
        /// <summary>The breadcrumb timestamp, as UTC.</summary>
        public DateTime TimestampUtc => Timestamp.ToUniversalTime();

        /// <summary>The breadcrumb timestamp.</summary>
        public DateTime Timestamp { get; init; }

        /// <summary>The breadcrumb message.</summary>
        public string Message { get; init; }

        /// <summary>The breadcrumb metadata.</summary>
        public object Metadata { get; init; }
    }
}
