namespace AllOverIt.Diagnostics.Breadcrumbs
{
    /// <summary>A collection of breadcrumb messages and metadata.</summary>
    public interface IBreadcrumbs : IEnumerable<BreadcrumbData>
    {
        /// <summary>Controls whether or not breadcrumbs are collected.</summary>
        bool Enabled { get; set; }

        /// <summary>The timestamp when breadcrumb collection begins.</summary>
        DateTime StartTimestamp { get; }

        /// <summary>Adds a new breadcrumb data item.</summary>
        /// <param name="breadcrumb">The breadcrumb data item.</param>
        void Add(BreadcrumbData breadcrumb);

        /// <summary>Clears the current collection of breadcrumbs.</summary>
        void Clear();

        /// <summary>Clears the current collection of breadcrumbs and resets <see cref="StartTimestamp"/>.</summary>
        void Reset();
    }
}
