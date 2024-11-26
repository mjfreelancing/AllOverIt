namespace AllOverIt.Diagnostics.Breadcrumbs
{
    /// <summary>Contains breadcrumb information at a point in time.</summary>
    public sealed class BreadcrumbData
    {
        private static long _counter;

        internal object? _metadata;
        internal string[]? _tags = [];

        internal long Counter { get; }

        /// <summary>The breadcrumb timestamp, as UTC.</summary>
        public DateTime TimestampUtc => Timestamp.ToUniversalTime();

        /// <summary>The breadcrumb timestamp.</summary>
        public DateTime Timestamp { get; } = DateTime.Now;

        /// <summary>The name of the calling method (if provided).</summary>
        public string? CallerName { get; internal set; }

        /// <summary>The file path of the calling method.</summary>
        public string? FilePath { get; internal set; }

        /// <summary>The line number of the calling method file path.</summary>
        public int LineNumber { get; internal set; }

        /// <summary>The breadcrumb message.</summary>
        public string? Message { get; init; }

        /// <summary>The breadcrumb metadata.</summary>
        public object? Metadata
        {
            get => _metadata;
            init => _metadata = value;
        }

        /// <summary>Optional tags associated with the data that can be used to later collate related content.</summary>
        public string[]? Tags
        {
            get => _tags;
            init => _tags = value;
        }

        /// <summary>Constructor.</summary>
        public BreadcrumbData()
        {
            Counter = Interlocked.Increment(ref _counter);
        }
    }
}
