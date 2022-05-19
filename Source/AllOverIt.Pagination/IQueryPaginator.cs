using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AllOverIt.Pagination
{
    public interface IQueryPaginator<TEntity> where TEntity : class
    {
        IQueryable<TEntity> BuildQuery(string continuationToken = default, int? pageSize = default);

        IQueryPaginator<TEntity> ColumnAscending<TProperty>(Expression<Func<TEntity, TProperty>> expression);
        IQueryPaginator<TEntity> ColumnDescending<TProperty>(Expression<Func<TEntity, TProperty>> expression);

        string CreateContinuationToken(ContinuationDirection direction, IReadOnlyCollection<TEntity> references);
        string CreateContinuationToken(ContinuationDirection direction, TEntity reference);
    }
}
