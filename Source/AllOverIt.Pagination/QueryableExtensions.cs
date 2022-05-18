using AllOverIt.Pagination;
using AllOverIt.Serialization.Abstractions;
using System.Linq;

namespace AllOverIt.Pagination
{
    public static class QueryableExtensions
    {
        public static PaginationQueryBuilder<TEntity> KeysetPaginate<TEntity>(this IQueryable<TEntity> query, IJsonSerializer serializer, int pageSize,
            PaginationDirection direction = PaginationDirection.Forward) where TEntity : class
        {
            return new PaginationQueryBuilder<TEntity>(serializer, query, direction, pageSize);
        }
    }
}
