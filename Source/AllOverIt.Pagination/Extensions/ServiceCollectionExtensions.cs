using AllOverIt.Pagination.TokenEncoding;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.Pagination.Extensions
{
    /// <summary>Provides extension methods for <see cref="IServiceCollection"/>.</summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers query pagination interfaces as singletons. The registered interfaces include <see cref="IQueryPaginatorFactory"/>,
        /// <see cref="IContinuationTokenEncoderFactory"/>, <see cref="IContinuationTokenSerializerFactory"/>, and <see cref="IContinuationTokenValidator"/>.</summary>
        /// <param name="serviceCollection">The service collection to register the interfaces with.</param>
        /// <returns>The service collection instance to allow for a fluent syntax.</returns>
        public static IServiceCollection AddQueryPagination(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IContinuationTokenValidator, ContinuationTokenValidator>();
            serviceCollection.AddSingleton<IContinuationTokenSerializerFactory, ContinuationTokenSerializerFactory>();
            serviceCollection.AddSingleton<IContinuationTokenEncoderFactory, ContinuationTokenEncoderFactory>();
            serviceCollection.AddSingleton<IQueryPaginatorFactory, QueryPaginatorFactory>();

            return serviceCollection;
        }
    }
}
