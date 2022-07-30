namespace AllOverIt.Pagination
{
    /// <summary>Provides options to control how paginated queries are generated.</summary>
    public sealed class QueryPaginatorConfiguration
    {
        /// <summary>The maximum number of records to be returned with each page.</summary>
        public int PageSize { get; init; }

        /// <summary>The direction to navigate when requesting the 'next' page.</summary>
        public PaginationDirection PaginationDirection { get; init; } = PaginationDirection.Forward;

        /// <summary>Indicates if arguments included in the query should be parameterized or not. This
        /// can be set to false for memory based pagination but should be set to true for Entity Framework
        /// based queries as it will help with performance as well as avoid SQL Injection attacks.</summary>
        public bool UseParameterizedQueries { get; init; } = true;

        /// <summary>Indicates if the continuation token should be compressed. This is not enabled by default.</summary>
        public bool UseCompression { get; init; }
    }
}
