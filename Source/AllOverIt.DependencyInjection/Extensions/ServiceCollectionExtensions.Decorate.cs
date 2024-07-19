using AllOverIt.Aspects;
using AllOverIt.Assertion;
using AllOverIt.DependencyInjection.Exceptions;
using AllOverIt.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.DependencyInjection.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>Decorates all registered <typeparamref name="TServiceType"/> types with <typeparamref name="TDecoratorType"/> types.
        /// Decoration is only applied to services already registered at the time of making the call.</summary>
        /// <typeparam name="TServiceType">The service type to be decorated.</typeparam>
        /// <typeparam name="TDecoratorType">The type decorating the service type. This type's constructor is expected to accept an argument
        /// of type <typeparamref name="TServiceType"/> in addition to any other dependencies it requires.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection Decorate<TServiceType, TDecoratorType>(this IServiceCollection serviceCollection)
            where TDecoratorType : TServiceType
        {
            _ = serviceCollection.WhenNotNull(nameof(serviceCollection));

            ReplaceServiceDescriptor<TServiceType>(serviceCollection, descriptor => Decorate(descriptor, typeof(TDecoratorType)));

            return serviceCollection;
        }

        /// <summary>Decorates all registered <typeparamref name="TServiceType"/> types with <typeparamref name="TInterceptor"/> types. Decoration is only applied
        /// to services already registered at the time of making the call.</summary>
        /// <typeparam name="TServiceType">The service type to be decorated.</typeparam>
        /// <typeparam name="TInterceptor">The interceptpr type decorating the service type. The interceptor must have a default constructor and 
        /// it cannot be a sealed class.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="configure">An optional action that can be used to configure the interceptor instance decorating the <typeparamref name="TServiceType"/>.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection DecorateWithInterceptor<TServiceType, TInterceptor>(this IServiceCollection serviceCollection, Action<IServiceProvider, TInterceptor>? configure = default)
            where TInterceptor : InterceptorBase<TServiceType>
        {
            _ = serviceCollection.WhenNotNull(nameof(serviceCollection));

            ReplaceServiceDescriptor<TServiceType>(serviceCollection, descriptor => DecorateWithInterceptor<TServiceType, TInterceptor>(descriptor, configure));

            return serviceCollection;
        }

        private static void ReplaceServiceDescriptor<TServiceType>(IServiceCollection serviceCollection, Func<ServiceDescriptor, ServiceDescriptor> descriptorResolver)
        {
            var descriptors = GetServiceDescriptors<TServiceType>(serviceCollection);

            foreach (var descriptor in descriptors)
            {
                var index = serviceCollection.IndexOf(descriptor);
                serviceCollection[index] = descriptorResolver.Invoke(descriptor);
            }
        }

        private static ServiceDescriptor Decorate(ServiceDescriptor descriptor, Type decoratorType)
        {
            return ServiceDescriptor.Describe(
                descriptor.ServiceType,
                provider =>
                {
                    var instance = GetInstance(provider, descriptor);
                    return ActivatorUtilities.CreateInstance(provider, decoratorType, instance);
                },
                descriptor.Lifetime);
        }

        private static ServiceDescriptor DecorateWithInterceptor<TServiceType, TInterceptor>(ServiceDescriptor descriptor, Action<IServiceProvider, TInterceptor>? configure)
            where TInterceptor : InterceptorBase<TServiceType>
        {
            return ServiceDescriptor.Describe(
                descriptor.ServiceType,
                provider =>
                {
                    var instance = (TServiceType) GetInstance(provider, descriptor);
                    return InterceptorFactory.CreateInterceptor(instance, provider, configure)!;
                },
                descriptor.Lifetime);
        }

        private static IReadOnlyCollection<ServiceDescriptor> GetServiceDescriptors<TServiceType>(IServiceCollection services)
        {
            var serviceType = typeof(TServiceType);
            var descriptors = services.Where(service => service.ServiceType == serviceType).AsReadOnlyCollection();

            if (descriptors.Count == 0)
            {
                throw new DependencyRegistrationException($"No registered services found for the type '{serviceType.GetFriendlyName()}'.");
            }

            return descriptors;
        }

        private static object GetInstance(IServiceProvider provider, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationInstance is not null)
            {
                return descriptor.ImplementationInstance;
            }

            return descriptor.ImplementationType is not null
                ? ActivatorUtilities.GetServiceOrCreateInstance(provider, descriptor.ImplementationType)
                : descriptor.ImplementationFactory!.Invoke(provider);
        }
    }
}