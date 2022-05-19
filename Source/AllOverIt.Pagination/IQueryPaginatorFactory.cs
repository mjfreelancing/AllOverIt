﻿using System.Linq;

namespace AllOverIt.Pagination
{
    public interface IQueryPaginatorFactory
    {
        IQueryPaginator<TEntity> CreatePaginator<TEntity>(IQueryable<TEntity> query) where TEntity : class;
    }
}
