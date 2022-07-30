using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.Pagination.Extensions
{
    /// <summary>Provides extension methods for <see cref="IServiceCollection"/>.</summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>Adds a query paginator as a singleton.</summary>
        /// <param name="serviceCollection">The service collection to register the <see cref="IQueryPaginatorFactory"/> with.</param>
        /// <returns>The service collection instance to allow for a fluent syntax.</returns>
        public static IServiceCollection AddQueryPaginatorFactory(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IQueryPaginatorFactory, QueryPaginatorFactory>();

            return serviceCollection;
        }
    }
}
