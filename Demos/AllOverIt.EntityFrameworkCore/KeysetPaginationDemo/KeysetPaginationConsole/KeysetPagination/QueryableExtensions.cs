using AllOverIt.Serialization.NewtonsoftJson;
using System.Linq;

namespace KeysetPaginationConsole.KeysetPagination
{
    public static class QueryableExtensions
    {
        public static PaginationQueryBuilder<TEntity> KeysetPaginate<TEntity>(this IQueryable<TEntity> query, int pageSize,
            PaginationDirection direction = PaginationDirection.Forward) where TEntity : class
        {
            // TODO: To be abstracted via a factory
            var serializer = new NewtonsoftJsonSerializer();

            return new PaginationQueryBuilder<TEntity>(serializer, query, direction, pageSize);
        }
    }
}
