using AllOverIt.Assertion;
using AllOverIt.Pagination.TokenEncoding;

namespace AllOverIt.Pagination
{
    /// <summary>Creates a <see cref="IQueryPaginator{TEntity}"/> instances.</summary>
    public sealed class QueryPaginatorFactory : IQueryPaginatorFactory
    {
        private readonly IContinuationTokenEncoderFactory _continuationTokenEncoderFactory;

        /// <summary>Constructor.</summary>
        /// <param name="continuationTokenEncoderFactory">A factory to create a continuation token encoder..</param>
        public QueryPaginatorFactory(IContinuationTokenEncoderFactory continuationTokenEncoderFactory)
        {
            _continuationTokenEncoderFactory = continuationTokenEncoderFactory.WhenNotNull();
        }

        /// <inheritdoc />
        public IQueryPaginator<TEntity> CreatePaginator<TEntity>(IQueryable<TEntity> query, QueryPaginatorConfiguration configuration)
            where TEntity : class
        {
            _ = query.WhenNotNull();
            _ = configuration.WhenNotNull();

            return new QueryPaginator<TEntity>(query, configuration, _continuationTokenEncoderFactory);
        }
    }
}
