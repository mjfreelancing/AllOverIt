using System.Linq;

namespace AllOverIt.Pagination
{
    public interface IQueryPaginatorFactory
    {
        IQueryPaginator<TEntity> CreatePaginator<TEntity>(IQueryable<TEntity> query, int pageSize,
            PaginationDirection paginationDirection = PaginationDirection.Forward) where TEntity : class;
    }
}
