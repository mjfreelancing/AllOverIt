namespace AllOverIt.Diagnostics.Breadcrumbs
{
    /// <summary>Provides options that control how breadcrumb items are inserted and cached by <see cref="Breadcrumbs"/>.</summary>
    public sealed class BreadcrumbsOptions
    {
        /// <summary>Indicates if breadcrumbs will be initialized in an Enabled state. Default is true.</summary>
        public bool StartEnabled { get; init; } = true;

        /// <summary>Indicates if the internal buffer should be thread safe for insert operations. Default is false.</summary>
        public bool ThreadSafe { get; init; }

        /// <summary>Specifies the maximum number of breadcrumbs to be stored. When the limit is exceeded, older
        /// items will be removed. Any value less than 1 is treated as unlimited. Default is -1.</summary>
        public int MaxCapacity { get; init; } = -1;
    }
}
