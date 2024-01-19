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

        /// <summary>Executes a paginated query and returns the results along with information pertaining to the previous and next pages of data.</summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="queryPaginator">The paginator containing the configured query to execute.</param>
        /// <param name="continuationToken">The continuation token that describes how to obtain the next (or previous) page of data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A page of results along with information about the previous and next page of data available.</returns>
        public static async Task<PageResult<TResult>> GetPageResultsAsync<TResult>(this IQueryPaginator<TResult> queryPaginator, string continuationToken,
            CancellationToken cancellationToken) where TResult : class
        {
            var totalCount = await queryPaginator.BaseQuery.CountAsync(cancellationToken).ConfigureAwait(false);
            var pageQuery = queryPaginator.GetPageQuery(continuationToken);
            var pageResults = await pageQuery.ToListAsync(cancellationToken).ConfigureAwait(false);
            var hasResults = pageResults.Count != 0;

            string previousToken = default;
            string nextToken = default;

            if (hasResults)
            {
                var (first, last) = queryPaginator.GetQueryDirection() == PaginationDirection.Forward
                    ? (pageResults[0], pageResults[^1])
                    : (pageResults[^1], pageResults[0]);

                var hasPreviousPage = await queryPaginator
                    .HasPreviousPageAsync(first, cancellationToken)
                    .ConfigureAwait(false);

                if (hasPreviousPage)
                {
                    previousToken = queryPaginator.TokenEncoder.EncodePreviousPage(pageResults);
                }

                var hasNextPage = await queryPaginator
                    .HasNextPageAsync(last, cancellationToken)
                    .ConfigureAwait(false);

                if (hasNextPage)
                {
                    nextToken = queryPaginator.TokenEncoder.EncodeNextPage(pageResults);
                }
            }

            return new PageResult<TResult>
            {
                Results = pageResults,
                TotalCount = totalCount,
                CurrentToken = continuationToken,
                PreviousToken = previousToken,
                NextToken = nextToken
            };
        }
    }
}