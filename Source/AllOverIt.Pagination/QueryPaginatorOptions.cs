using AllOverIt.Serialization.Abstractions;

namespace AllOverIt.Pagination
{
    public sealed class QueryPaginatorOptions
    {
        public IJsonSerializer Serializer { get; init; }
    }
}
