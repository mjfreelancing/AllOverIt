using AllOverIt.Assertion;
using AllOverIt.Extensions;
using System.Reflection;

namespace AllOverIt.Pagination
{
    internal abstract class ColumnDefinition<TEntity> : IColumnDefinition where TEntity : class
    {
        public PropertyInfo Property { get; }
        public bool IsAscending { get; }

        public ColumnDefinition(PropertyInfo property, bool isAscending)
        {
            Property = property.WhenNotNull();
            IsAscending = isAscending;
        }

        public abstract IOrderedQueryable<TEntity> ApplyColumnOrderTo(IQueryable<TEntity> queryable, PaginationDirection paginationDirection);
        public abstract IOrderedQueryable<TEntity> ThenApplyColumnOrderTo(IOrderedQueryable<TEntity> queryable, PaginationDirection paginationDirection);
    }

    internal class ColumnDefinition<TEntity, TProp> : ColumnDefinition<TEntity> where TEntity : class
    {
        public ColumnDefinition(PropertyInfo property, bool isAscending)
            : base(property, isAscending)
        {
        }

        public override IOrderedQueryable<TEntity> ApplyColumnOrderTo(IQueryable<TEntity> queryable, PaginationDirection paginationDirection)
        {
            return OrderBy(queryable, paginationDirection);
        }

        public override IOrderedQueryable<TEntity> ThenApplyColumnOrderTo(IOrderedQueryable<TEntity> queryable, PaginationDirection paginationDirection)
        {
            return ThenOrderBy(queryable, paginationDirection);
        }

        private IOrderedQueryable<TEntity> OrderBy(IQueryable<TEntity> queryable, PaginationDirection paginationDirection)
        {
            _ = queryable.WhenNotNull();
            _ = Property.WhenNotNull();

            var accessExpression = Property.CreateMemberAccessLambda<TEntity, TProp>("entity");

            return OrderAsAscending(paginationDirection)
                ? queryable.OrderBy(accessExpression)
                : queryable.OrderByDescending(accessExpression);
        }

        private IOrderedQueryable<TEntity> ThenOrderBy(IOrderedQueryable<TEntity> queryable, PaginationDirection paginationDirection)
        {
            _ = queryable.WhenNotNull();
            _ = Property.WhenNotNull();

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
