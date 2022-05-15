using AllOverIt.Serialization.NewtonsoftJson;
using System.Linq;

namespace KeysetPaginationConsole.KeysetPagination
{
    public static class QueryableExtensions
    {
        public static PaginationQueryBuilder<TEntity> KeysetPaginate<TEntity>(this IQueryable<TEntity> query, int pageSize, string continuationToken = default)
            where TEntity : class
        {
            return KeysetPaginate(query, PaginationDirection.Forward, pageSize, continuationToken);
        }

        public static PaginationQueryBuilder<TEntity> KeysetPaginate<TEntity>(this IQueryable<TEntity> query, PaginationDirection direction,
            int pageSize, string continuationToken = default) where TEntity : class
        {
            // TODO: To be abstracted
            var serializer = new NewtonsoftJsonSerializer();

            return new PaginationQueryBuilder<TEntity>(serializer, query, direction, pageSize, continuationToken);
        }
    }
}
