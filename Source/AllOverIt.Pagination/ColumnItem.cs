﻿using AllOverIt.Extensions;
using System.Linq;
using System.Reflection;

namespace AllOverIt.Pagination
{
    internal abstract class ColumnItem<TEntity> : IColumnItem where TEntity : class
    {
        public PropertyInfo Property { get; init; }
        public bool IsAscending { get; init; }

        public abstract IOrderedQueryable<TEntity> ApplyOrderBy(IQueryable<TEntity> queryable, PaginationDirection paginationDirection);
        public abstract IOrderedQueryable<TEntity> ApplyThenOrderBy(IOrderedQueryable<TEntity> queryable, PaginationDirection paginationDirection);
    }

    internal class ColumnItem<TEntity, TProp> : ColumnItem<TEntity> where TEntity : class
    {
        public override IOrderedQueryable<TEntity> ApplyOrderBy(IQueryable<TEntity> queryable, PaginationDirection paginationDirection)
        {
            return OrderBy(queryable, paginationDirection);
        }

        public override IOrderedQueryable<TEntity> ApplyThenOrderBy(IOrderedQueryable<TEntity> queryable, PaginationDirection paginationDirection)
        {
            return ThenOrderBy(queryable, paginationDirection);
        }

        private IOrderedQueryable<TEntity> OrderBy(IQueryable<TEntity> queryable, PaginationDirection paginationDirection)
        {
            var accessExpression = Property.CreateMemberAccessLambda<TEntity, TProp>("entity");

            return OrderAsAscending(paginationDirection)
                ? queryable.OrderBy(accessExpression)
                : queryable.OrderByDescending(accessExpression);
        }

        private IOrderedQueryable<TEntity> ThenOrderBy(IOrderedQueryable<TEntity> queryable, PaginationDirection paginationDirection)
        {
            var accessExpression = Property.CreateMemberAccessLambda<TEntity, TProp>("entity");

            return OrderAsAscending(paginationDirection)
                ? queryable.ThenBy(accessExpression)
                : queryable.ThenByDescending(accessExpression);
        }

        private bool OrderAsAscending(PaginationDirection paginationDirection)
        {
            return paginationDirection == PaginationDirection.Forward
                ? IsAscending
                : !IsAscending;
        }
    }
}
