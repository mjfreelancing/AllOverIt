using Microsoft.Extensions.DependencyInjection;
using System;

namespace AllOverIt.Pagination.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQueryPaginator(this IServiceCollection serviceCollection, Func<IServiceProvider, QueryPaginatorOptions> optionsResolver)
        {
            serviceCollection.AddSingleton<IQueryPaginatorFactory>(provider =>
            {
                var options = optionsResolver.Invoke(provider);
                return new QueryPaginatorFactory(options);
            });

            return serviceCollection;
        }
    }
}
