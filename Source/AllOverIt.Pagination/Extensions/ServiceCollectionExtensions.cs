using Microsoft.Extensions.DependencyInjection;
using System;

namespace AllOverIt.Pagination.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQueryPaginator(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IQueryPaginatorFactory, QueryPaginatorFactory>();

            return serviceCollection;
        }
    }
}
