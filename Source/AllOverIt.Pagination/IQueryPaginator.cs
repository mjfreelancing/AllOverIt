﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AllOverIt.Pagination
{
    public interface IQueryPaginator<TEntity> where TEntity : class
    {
        IQueryable<TEntity> BuildPageQuery(string continuationToken = default, int? pageSize = default);

        // Provides a query that can be executed to retrieve rows from the provided reference without
        // regard to any page size. Can also be used with Any() (or AnyAsync in EF) to determine if
        // there's additional data in either direction.
        IQueryable<TEntity> BuildBackwardQuery(TEntity reference);
        IQueryable<TEntity> BuildForwardQuery(TEntity reference);

        IQueryPaginator<TEntity> ColumnAscending<TProperty>(Expression<Func<TEntity, TProperty>> expression);
        IQueryPaginator<TEntity> ColumnDescending<TProperty>(Expression<Func<TEntity, TProperty>> expression);

        string CreateContinuationToken(ContinuationDirection direction, IReadOnlyCollection<TEntity> references);
        string CreateContinuationToken(ContinuationDirection direction, TEntity reference);
    }
}
