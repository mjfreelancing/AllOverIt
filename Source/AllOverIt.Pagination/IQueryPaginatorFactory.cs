using System.Linq;

namespace AllOverIt.Pagination
{
    public interface IQueryPaginatorFactory
    {
        IQueryPaginator<TEntity> CreatePaginator<TEntity>(IQueryable<TEntity> query, PaginationDirection direction = PaginationDirection.Forward,
            int? defaultPageSize = default) where TEntity : class;
    }
}
