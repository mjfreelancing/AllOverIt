using AllOverIt.Assertion;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.DependencyInjection.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>Registers a <see cref="INamedServiceResolver"/> and a <see cref="INamedServiceResolver{TService}"/> as a Singleton with
        /// the provided <paramref name="serviceCollection"/>. The <see cref="INamedServiceResolver"/> can be used to resolve any service
        /// type by name, whereas the <see cref="INamedServiceResolver{TService}"/> can only resolve named services implementing
        /// <typeparamref name="TService"/>.</summary>
        /// <typeparam name="TService">The service type that can later be resolved.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>An <see cref="INamedServiceBuilder{TService}"/> that can be used to register the implementation types by name.</returns>
        public static INamedServiceBuilder<TService> AddNamedServices<TService>(this IServiceCollection serviceCollection) where TService : class
        {
            _ = serviceCollection.WhenNotNull(nameof(serviceCollection));

            // The builder provides a mechanism for the caller to register named services in the IServiceCollection and an INamedDependencyResolver
            var builder = new NamedServiceBuilder<TService>(serviceCollection);

            // Register the resolver that the caller will ultimately use for resolving a previously registered service
            serviceCollection.AddSingleton<INamedServiceResolver<TService>>(provider =>
            {
                var resolver = builder._resolver;

                // We can now inject the provider so the resolver can resolve from the IServiceProvider
                resolver._provider = provider;

                return resolver;
            });

            serviceCollection.AddSingleton<INamedServiceResolver>(provider => new NamedServiceResolver(provider));

            return builder;
        }
    }
}