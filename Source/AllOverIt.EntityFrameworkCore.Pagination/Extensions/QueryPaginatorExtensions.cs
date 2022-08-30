using AllOverIt.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.EntityFrameworkCore.Pagination.Extensions
{
    /// <summary>Provides extension methods for <see cref="IQueryPaginator{TEntity}"/>.</summary>
    public static class QueryPaginatorExtensions
    {
        /// <summary>Indicates if there's any data prior to the provided entity reference, based on the current query definition.</summary>
        /// <typeparam name="TEntity">The entity type the query is based on.</typeparam>
        /// <param name="paginator">The query paginator instance.</param>
        /// <param name="reference">The entity reference that all subsequent data must be prior to.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The current paginator instance to support a fluent syntax.</returns>
        public static Task<bool> HasPreviousPageAsync<TEntity>(this IQueryPaginator<TEntity> paginator, TEntity reference,
            CancellationToken cancellationToken = default) where TEntity : class
        {
            return paginator.HasPreviousPageAsync(reference, (queryable, predicate, token) =>
            {
                return queryable.AnyAsync(predicate, token);
            }, cancellationToken);
        }

        /// <summary>Indicates if there's any data after the provided entity reference, based on the current query definition.</summary>
        /// <typeparam name="TEntity">The entity type the query is based on.</typeparam>
        /// <param name="paginator">The query paginator instance.</param>
        /// <param name="reference">The entity reference that all subsequent data must follow.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The current paginator instance to support a fluent syntax.</returns>
        public static Task<bool> HasNextPageAsync<TEntity>(this IQueryPaginator<TEntity> paginator, TEntity reference,
            CancellationToken cancellationToken = default) where TEntity : class
        {
            return paginator.HasNextPageAsync(reference, (queryable, predicate, token) =>
            {
                return queryable.AnyAsync(predicate, token);
            }, cancellationToken);
        }
    }
}