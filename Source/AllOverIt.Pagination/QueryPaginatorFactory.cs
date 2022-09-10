using AllOverIt.Assertion;
using System.Linq;

namespace AllOverIt.Pagination
{
    /// <summary>Creates a <see cref="IQueryPaginator{TEntity}"/> instances.</summary>
    public sealed class QueryPaginatorFactory : IQueryPaginatorFactory
    {
        private readonly IContinuationTokenEncoderFactory _continuationTokenEncoderFactory;

        public QueryPaginatorFactory(IContinuationTokenEncoderFactory continuationTokenEncoderFactory)
        {
            _continuationTokenEncoderFactory = continuationTokenEncoderFactory.WhenNotNull(nameof(continuationTokenEncoderFactory));
        }

        /// <inheritdoc />
        public IQueryPaginator<TEntity> CreatePaginator<TEntity>(IQueryable<TEntity> query, QueryPaginatorConfiguration configuration)
            where TEntity : class
        {
            _ = query.WhenNotNull(nameof(query));
            _ = configuration.WhenNotNull(nameof(configuration));

            return new QueryPaginator<TEntity>(query, configuration, _continuationTokenEncoderFactory);
        }
    }
}
