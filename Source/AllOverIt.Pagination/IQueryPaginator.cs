using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pagination
{
    /// <summary>Defines a keyset paginated query builder that supports ordering multiple columns suitable for forward and backward navigation.</summary>
    /// <typeparam name="TEntity">The entity type that the generated query is based on.</typeparam>
    public interface IQueryPaginator<TEntity> where TEntity : class
    {
        /// <summary>A token generator that simplifies requesting next and previous page queries.</summary>
        IContinuationTokenEncoder TokenEncoder { get; }

        /// <summary>Appends a new ascending order-by column. When ordering by multiple columns it is important that the last column
        /// is unique across all pages (such as the IDENTITY column), even if that column is not returned in the results.</summary>
        /// <typeparam name="TProperty">The entity's property type (the column).</typeparam>
        /// <param name="expression">An expression indicating the property to be added.</param>
        /// <returns>The current paginator instance to support a fluent syntax.</returns>
        IQueryPaginator<TEntity> ColumnAscending<TProperty>(Expression<Func<TEntity, TProperty>> expression);

        /// <summary>Appends a new descending order-by column. When ordering by multiple columns it is important that the last column
        /// is unique across all pages (such as the IDENTITY column), even if that column is not returned in the results.</summary>
        /// <typeparam name="TProperty">The entity's property type (the column).</typeparam>
        /// <param name="expression">An expression indicating the property to be added.</param>
        /// <returns>The current paginator instance to support a fluent syntax.</returns>
        IQueryPaginator<TEntity> ColumnDescending<TProperty>(Expression<Func<TEntity, TProperty>> expression);

        /// <summary>Creates a query based on the paginator's column definition (ascending and descending columns) along with the content
        /// of the continuation token. The token includes information about the previously queried data and the relative direction to
        /// navigate.</summary>
        /// <param name="continuationToken">The continuation token. The token will only be valid for queries that are constructed equally
        /// across consecutive calls. The content of the token is only used to determine where the required page starts.</param>
        /// <returns>The current paginator instance to support a fluent syntax.</returns>
        IQueryable<TEntity> GetPageQuery(string continuationToken = default);

        /// <summary>Creates a query that returns data that is previous to (relative to the pagination direction) the provided entity reference.</summary>
        /// <param name="reference">The entity reference that all subsequent data must be prior to.</param>
        /// <returns>The current paginator instance to support a fluent syntax.</returns>
        IQueryable<TEntity> GetPreviousPageQuery(TEntity reference);

        /// <summary>Creates a query that returns data that occurs after (relative to the pagination direction) the provided entity reference.</summary>
        /// <param name="reference">The entity reference that all subsequent data must follow.</param>
        /// <returns>The current paginator instance to support a fluent syntax.</returns>
        IQueryable<TEntity> GetNextPageQuery(TEntity reference);

        /// <summary>Indicates if there's any data prior to the provided entity reference, based on the current query definition.</summary>
        /// <param name="reference">The entity reference that all subsequent data must be prior to.</param>
        /// <returns>The current paginator instance to support a fluent syntax.</returns>
        bool HasPreviousPage(TEntity reference);

        /// <summary>Indicates if there's any data prior to the provided entity reference, based on the current query definition.</summary>
        /// <param name="reference">The entity reference that all subsequent data must be prior to.</param>
        /// <param name="anyResolver">A resolver that determines if there's any data before the provided entity reference.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The current paginator instance to support a fluent syntax.</returns>
        Task<bool> HasPreviousPageAsync(TEntity reference, Func<IQueryable<TEntity>, Expression<Func<TEntity, bool>>, CancellationToken, Task<bool>> anyResolver,
            CancellationToken cancellationToken);

        /// <summary>Indicates if there's any data after the provided entity reference, based on the current query definition.</summary>
        /// <param name="reference">The entity reference that all subsequent data must follow.</param>
        /// <returns>The current paginator instance to support a fluent syntax.</returns>
        bool HasNextPage(TEntity reference);

        /// <summary>Indicates if there's any data after the provided entity reference, based on the current query definition.</summary>
        /// <param name="reference">The entity reference that all subsequent data must follow.</param>
        /// <param name="anyResolver">A resolver that determines if there's any data after the provided entity reference.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The current paginator instance to support a fluent syntax.</returns>
        Task<bool> HasNextPageAsync(TEntity reference, Func<IQueryable<TEntity>, Expression<Func<TEntity, bool>>, CancellationToken, Task<bool>> anyResolver,
            CancellationToken cancellationToken);
    }
}
