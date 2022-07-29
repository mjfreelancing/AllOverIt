using AllOverIt.Assertion;
using System.Linq;

namespace AllOverIt.Pagination
{
    public sealed class QueryPaginatorFactory : IQueryPaginatorFactory
    {
        public IQueryPaginator<TEntity> CreatePaginator<TEntity>(IQueryable<TEntity> query, QueryPaginatorConfiguration configuration)
            where TEntity : class
        {
            _ = query.WhenNotNull(nameof(query));
            _ = configuration.WhenNotNull(nameof(configuration));

            return new QueryPaginator<TEntity>(query, configuration);
        }
    }
}
