namespace AllOverIt.Pagination
{
    /// <summary>Defined a factory that creates a <see cref="IQueryPaginator{TEntity}"/> instances.</summary>
    public interface IQueryPaginatorFactory
    {
        /// <summary>Creates a <see cref="IQueryPaginator{TEntity}"/> instance using the provided base query and paginator configuration.</summary>
        /// <typeparam name="TEntity">The entity type the query is based on.</typeparam>
        /// <param name="query">The base query to apply pagination to.</param>
        /// <param name="configuration">Provides paginator options that define how the paginated query will be generated.</param>
        /// <returns>A new <see cref="IQueryPaginator{TEntity}"/> instance.</returns>
        IQueryPaginator<TEntity> CreatePaginator<TEntity>(IQueryable<TEntity> query, QueryPaginatorConfiguration configuration) where TEntity : class;
    }
}
