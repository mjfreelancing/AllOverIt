using AllOverIt.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace KeysetPaginationConsole
{
    public static class QueryPaginatorExtensions
    {
        public static Task<bool> HasPreviousPageAsync<TEntity>(this IQueryPaginator<TEntity> paginator, TEntity reference,
            CancellationToken cancellation = default) where TEntity : class
        {
            return paginator.HasPreviousPageAsync(reference, (queryable, predicate, token) =>
            {
                return queryable.AnyAsync(predicate, token);
            }, cancellation);
        }

        public static Task<bool> HasNextPageAsync<TEntity>(this IQueryPaginator<TEntity> paginator, TEntity reference,
            CancellationToken cancellation = default) where TEntity : class
        {
            return paginator.HasNextPageAsync(reference, (queryable, predicate, token) =>
            {
                return queryable.AnyAsync(predicate, token);
            }, cancellation);
        }
    }
}