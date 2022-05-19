using AllOverIt.Serialization.Abstractions;

namespace AllOverIt.Pagination
{
    public sealed class QueryPaginatorOptions
    {
        public PaginationDirection Direction { get; init; } = PaginationDirection.Forward;
        public int? DefaultPageSize { get; init; }
        public IJsonSerializer Serializer { get; init; }
    }
}
