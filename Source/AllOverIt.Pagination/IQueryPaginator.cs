using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Pagination
{
    public interface IQueryPaginator<TEntity> where TEntity : class
    {
        IContinuationTokenEncoder ContinuationTokenEncoder { get; }

        IQueryPaginator<TEntity> ColumnAscending<TProperty>(Expression<Func<TEntity, TProperty>> expression);
        IQueryPaginator<TEntity> ColumnDescending<TProperty>(Expression<Func<TEntity, TProperty>> expression);

        IQueryable<TEntity> GetPageQuery(string continuationToken = default);     // Using the relative direction defined by the token
        IQueryable<TEntity> GetPreviousPageQuery(TEntity reference);              // relative to the pagination direction
        IQueryable<TEntity> GetNextPageQuery(TEntity reference);                  // relative to the pagination direction

        bool HasPreviousPage(TEntity reference);

        Task<bool> HasPreviousPageAsync(TEntity reference, Func<IQueryable<TEntity>, Expression<Func<TEntity, bool>>, CancellationToken, Task<bool>> anyResolver,
            CancellationToken cancellationToken);

        bool HasNextPage(TEntity reference);

        Task<bool> HasNextPageAsync(TEntity reference, Func<IQueryable<TEntity>, Expression<Func<TEntity, bool>>, CancellationToken, Task<bool>> anyResolver,
            CancellationToken cancellationToken);
    }
}
