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
        IQueryPaginator<TEntity> ColumnAscending<TProperty>(Expression<Func<TEntity, TProperty>> expression);
        IQueryPaginator<TEntity> ColumnDescending<TProperty>(Expression<Func<TEntity, TProperty>> expression);

        IQueryable<TEntity> BuildPageQuery(string continuationToken = default);     // Using the relative direction defined by the token
        IQueryable<TEntity> BuildPreviousPageQuery(TEntity reference);              // relative to the pagination direction
        IQueryable<TEntity> BuildNextPageQuery(TEntity reference);                  // relative to the pagination direction

        bool HasPreviousPage(TEntity reference);

        Task<bool> HasPreviousPageAsync(TEntity reference, Func<IQueryable<TEntity>, Expression<Func<TEntity, bool>>, CancellationToken, Task<bool>> anyResolver,
            CancellationToken cancellationToken);

        bool HasNextPage(TEntity reference);

        Task<bool> HasNextPageAsync(TEntity reference, Func<IQueryable<TEntity>, Expression<Func<TEntity, bool>>, CancellationToken, Task<bool>> anyResolver,
            CancellationToken cancellationToken);

        string CreateContinuationToken(ContinuationDirection direction, IReadOnlyCollection<TEntity> references);
        string CreateContinuationToken(ContinuationDirection direction, TEntity reference);
        string CreateFirstPageContinuationToken();
        string CreateLastPageContinuationToken();
    }
}
