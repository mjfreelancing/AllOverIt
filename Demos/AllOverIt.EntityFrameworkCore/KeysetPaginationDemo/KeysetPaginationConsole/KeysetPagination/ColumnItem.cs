using AllOverIt.Extensions;
using System.Linq;
using System.Reflection;

namespace KeysetPaginationConsole.KeysetPagination
{
    public interface IColumnItem
    {
        PropertyInfo Property { get; }
    }

    internal abstract class ColumnItem<TEntity> : IColumnItem where TEntity : class
    {
        public PropertyInfo Property { get; init; }
        public bool IsAscending { get; init; }

        public abstract IOrderedQueryable<TEntity> ApplyOrderBy(IQueryable<TEntity> query, PaginationDirection direction);
        public abstract IOrderedQueryable<TEntity> ApplyThenOrderBy(IOrderedQueryable<TEntity> query, PaginationDirection direction);
    }

    internal class ColumnItem<TEntity, TProp> : ColumnItem<TEntity> where TEntity : class
    {
        public override IOrderedQueryable<TEntity> ApplyOrderBy(IQueryable<TEntity> query, PaginationDirection direction)
        {
            return OrderBy(query, direction);
        }

        public override IOrderedQueryable<TEntity> ApplyThenOrderBy(IOrderedQueryable<TEntity> query, PaginationDirection direction)
        {
            return ThenOrderBy(query, direction);
        }

        private IOrderedQueryable<TEntity> OrderBy(IQueryable<TEntity> query, PaginationDirection direction)
        {
            var accessExpression = Property.CreateMemberAccessLambda<TEntity, TProp>("entity");

            var isAscending = direction == PaginationDirection.Forward
                ? IsAscending
                : !IsAscending;

            return isAscending
                ? query.OrderBy(accessExpression)
                : query.OrderByDescending(accessExpression);
        }

        private IOrderedQueryable<TEntity> ThenOrderBy(IOrderedQueryable<TEntity> query, PaginationDirection direction)
        {
            var accessExpression = Property.CreateMemberAccessLambda<TEntity, TProp>("entity");

            var isAscending = direction == PaginationDirection.Forward
                ? IsAscending
                : !IsAscending;

            return isAscending
                ? query.ThenBy(accessExpression)
                : query.ThenByDescending(accessExpression);
        }
    }
}
