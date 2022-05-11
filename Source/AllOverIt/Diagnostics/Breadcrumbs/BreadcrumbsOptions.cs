namespace AllOverIt.Diagnostics.Breadcrumbs
{
    /// <summary>Provides options that control how breadcrumb items are inserted and cached by <see cref="Breadcrumbs"/>.</summary>
    public sealed class BreadcrumbsOptions
    {
        /// <summary>Indicates if the internal buffer should be thread safe for insert operations.</summary>
        public bool ThreadSafe { get; init; }

        /// <summary>Specifies the maximum number of breadcrumbs to be stored. When the limit is exceeded, older
        /// items will be removed. Any value less than 1 is treated as unlimited.</summary>
        public int MaxCapacity { get; init; } = -1;
    }
}
