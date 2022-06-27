namespace AllOverIt.Pagination
{
    public sealed class QueryPaginatorConfiguration
    {
        public int PageSize { get; init; }
        public PaginationDirection PaginationDirection { get; init; } = PaginationDirection.Forward;

        // Set to False for memory based pagination. Set to True for EF based queries.
        public bool UseParameterizedQueries { get; init; } = true;
    }
}
