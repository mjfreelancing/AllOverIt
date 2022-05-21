using AllOverIt.Serialization.Abstractions;

namespace AllOverIt.Pagination
{
    public sealed class QueryPaginatorConfig
    {
        public IJsonSerializer Serializer { get; init; }
    }
}
