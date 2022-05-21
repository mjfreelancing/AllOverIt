using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;

namespace AllOverIt.Pagination
{
    public interface IQueryPaginator<TEntity> where TEntity : class
    {
        IQueryable<TEntity> BuildPageQuery(string continuationToken = default);

        Task<bool> HasPreviousPageAsync(TEntity reference, Func<IQueryable<TEntity>, Expression<Func<TEntity, bool>>, CancellationToken, Task<bool>> anyResolver,
            CancellationToken cancellationToken);

        Task<bool> HasNextPageAsync(TEntity reference, Func<IQueryable<TEntity>, Expression<Func<TEntity, bool>>, CancellationToken, Task<bool>> anyResolver,
            CancellationToken cancellationToken);


        // Provides a query that can be executed to retrieve rows from the provided reference without
        // regard to any page size. Can also be used with Any() (or AnyAsync in EF) to determine if
        // there's additional data in either direction.
        IQueryable<TEntity> BuildPreviousPageQuery(TEntity reference);      // relative to the pagination direction
        IQueryable<TEntity> BuildNextPageQuery(TEntity reference);       // relative to the pagination direction


        IQueryPaginator<TEntity> ColumnAscending<TProperty>(Expression<Func<TEntity, TProperty>> expression);
        IQueryPaginator<TEntity> ColumnDescending<TProperty>(Expression<Func<TEntity, TProperty>> expression);


        string CreateContinuationToken(ContinuationDirection direction, IReadOnlyCollection<TEntity> references);
        string CreateContinuationToken(ContinuationDirection direction, TEntity reference);
        string CreateFirstPageContinuationToken();
        string CreateLastPageContinuationToken();
    }
}
