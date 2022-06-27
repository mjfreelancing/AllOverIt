using System.Linq;

namespace AllOverIt.Pagination
{
    public sealed class QueryPaginatorFactory : IQueryPaginatorFactory
    {
        public IQueryPaginator<TEntity> CreatePaginator<TEntity>(IQueryable<TEntity> query, int pageSize,
            PaginationDirection paginationDirection = PaginationDirection.Forward) where TEntity : class
        {
            return new QueryPaginator<TEntity>(query, pageSize, paginationDirection);
        }
    }
}
