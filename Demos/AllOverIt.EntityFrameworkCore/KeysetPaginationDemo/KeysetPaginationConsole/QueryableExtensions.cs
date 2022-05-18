using AllOverIt.Pagination;
using AllOverIt.Serialization.NewtonsoftJson;
using System.Linq;

namespace KeysetPaginationConsole
{
    public static class QueryableExtensions
    {
        public static PaginationQueryBuilder<TEntity> KeysetPaginate<TEntity>(this IQueryable<TEntity> query, int pageSize,
            PaginationDirection direction = PaginationDirection.Forward) where TEntity : class
        {
            // TODO: To be abstracted via a factory
            var serializer = new NewtonsoftJsonSerializer();

            return query.KeysetPaginate<TEntity>(serializer, pageSize, direction);
        }
    }
}
