namespace AllOverIt.Diagnostics.Breadcrumbs
{
    /// <summary>Contains breadcrumb information at a point in time.</summary>
    public sealed class BreadcrumbData
    {
        private static long _counter;
        internal long Counter { get; }

        /// <summary>The breadcrumb timestamp, as UTC.</summary>
        public DateTime TimestampUtc => Timestamp.ToUniversalTime();

        /// <summary>The breadcrumb timestamp.</summary>
        public DateTime Timestamp { get; } = DateTime.Now;

        /// <summary>The name of the calling method (if provided).</summary>
        public string? CallerName { get; init; }

        /// <summary>The file path of the calling method.</summary>
        public string? FilePath { get; init; }

        /// <summary>The line number of the calling method file path.</summary>
        public int LineNumber { get; init; }

        /// <summary>The breadcrumb message.</summary>
        public string? Message { get; init; }

        /// <summary>The breadcrumb metadata.</summary>
        public object? Metadata { get; init; }

        /// <summary>Constructor.</summary>
        public BreadcrumbData()
        {
            Counter = Interlocked.Increment(ref _counter);
        }
    }
}
