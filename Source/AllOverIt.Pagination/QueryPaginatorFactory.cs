using AllOverIt.Assertion;
using System.Linq;

namespace AllOverIt.Pagination
{
    public sealed class QueryPaginatorFactory : IQueryPaginatorFactory
    {
        private readonly QueryPaginatorConfig _config;

        public QueryPaginatorFactory(QueryPaginatorConfig config)
        {
            _config = config.WhenNotNull(nameof(config));
        }

        public IQueryPaginator<TEntity> CreatePaginator<TEntity>(IQueryable<TEntity> query, int pageSize,
            PaginationDirection paginationDirection = PaginationDirection.Forward) where TEntity : class
        {
            return new QueryPaginator<TEntity>(query, _config, pageSize, paginationDirection);
        }
    }
}
