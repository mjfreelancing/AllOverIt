using AllOverIt.Assertion;
using System.Linq;

namespace AllOverIt.Pagination
{
    public sealed class QueryPaginatorFactory : IQueryPaginatorFactory
    {
        private readonly QueryPaginatorOptions _options;

        public QueryPaginatorFactory(QueryPaginatorOptions options)
        {
            _options = options.WhenNotNull(nameof(options));
        }

        public IQueryPaginator<TEntity> CreatePaginator<TEntity>(IQueryable<TEntity> query, 
            PaginationDirection direction = PaginationDirection.Forward, int? defaultPageSize = default)
            where TEntity : class
        {
            return new QueryPaginator<TEntity>(_options.Serializer, query, direction, defaultPageSize);
        }
    }
}
