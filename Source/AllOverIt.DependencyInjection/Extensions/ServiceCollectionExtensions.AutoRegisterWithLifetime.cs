using AllOverIt.Assertion;
using AllOverIt.DependencyInjection.Exceptions;
using AllOverIt.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.DependencyInjection.Extensions
{
    // Helper methods for use by AutoRegisterScoped(), AutoRegisterSingleton(), and AutoRegisterTransient()
    public static partial class ServiceCollectionExtensions
    {
        private static readonly Dictionary<ServiceLifetime, Action<IServiceCollection, Type, Type>> ServiceCreatorsByType =
            new()
            {
                { ServiceLifetime.Scoped, (serviceCollection, serviceType, implementationType) => serviceCollection.AddScoped(serviceType, implementationType) },
                { ServiceLifetime.Transient, (serviceCollection, serviceType, implementationType) => serviceCollection.AddTransient(serviceType, implementationType) },
                { ServiceLifetime.Singleton, (serviceCollection, serviceType, implementationType) => serviceCollection.AddSingleton(serviceType, implementationType) }
            };

        private static readonly Dictionary<ServiceLifetime, Action<IServiceCollection, Type, Func<IServiceProvider, object>>> ServiceCreatorsByFactory =
            new()
            {
                { ServiceLifetime.Scoped, (serviceCollection, serviceType, factory) => serviceCollection.AddScoped(serviceType, factory) },
                { ServiceLifetime.Transient, (serviceCollection, serviceType, factory) => serviceCollection.AddTransient(serviceType, factory) },
                { ServiceLifetime.Singleton, (serviceCollection, serviceType, factory) => serviceCollection.AddSingleton(serviceType, factory) }
            };

        private static IServiceCollection AutoRegisterWithLifetime<TServiceRegistrar, TServiceType>(IServiceCollection serviceCollection, Action<IServiceRegistrarOptions>? configure,
            ServiceLifetime lifetime)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            _ = serviceCollection.WhenNotNull();

            return AutoRegisterWithLifetime(serviceCollection, new TServiceRegistrar(), [typeof(TServiceType)], configure, lifetime);
        }

        private static IServiceCollection AutoRegisterWithLifetime<TServiceRegistrar>(this IServiceCollection serviceCollection, IEnumerable<Type> serviceTypes,
            Action<IServiceRegistrarOptions>? configure, ServiceLifetime lifetime)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            _ = serviceCollection.WhenNotNull();
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty().AsReadOnlyCollection();

            return AutoRegisterWithLifetime(serviceCollection, new TServiceRegistrar(), allServiceTypes, configure, lifetime);
        }

        private static IServiceCollection AutoRegisterWithLifetime<TServiceType>(this IServiceCollection serviceCollection, IServiceRegistrar serviceRegistrar,
            Action<IServiceRegistrarOptions>? configure, ServiceLifetime lifetime)
        {
            _ = serviceCollection.WhenNotNull();
            _ = serviceRegistrar.WhenNotNull();

            return AutoRegisterWithLifetime(serviceCollection, serviceRegistrar, [typeof(TServiceType)], configure, lifetime);
        }

        private static IServiceCollection AutoRegisterWithLifetime(this IServiceCollection serviceCollection, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            Action<IServiceRegistrarOptions>? configure, ServiceLifetime lifetime)
        {
            _ = serviceCollection.WhenNotNull();
            _ = serviceRegistrar.WhenNotNull();
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty().AsReadOnlyCollection();

            serviceRegistrar.AutoRegisterServices(
                allServiceTypes,
                (serviceType, implementationType) =>
                {
                    var descriptors = serviceCollection
                        .Where(service => service.ServiceType == serviceType && service.ImplementationType == implementationType)
                        .AsReadOnlyList();

                    if (descriptors.Any())
                    {
                        var firstMismatch = descriptors.FirstOrDefault(item => item.Lifetime != lifetime);

                        if (firstMismatch is not null)
                        {
                            throw new DependencyRegistrationException($"The service type {serviceType.GetFriendlyName()} is already registered to the implementation type " +
                                                                      $"{implementationType.GetFriendlyName()} but has a different lifetime ({firstMismatch.Lifetime}).");
                        }

                        return;
                    }

                    // Call AddScoped(), AddTransient(), AddSingleton() with the types as required
                    ServiceCreatorsByType[lifetime].Invoke(serviceCollection, serviceType, implementationType);
                },
                configure);

            return serviceCollection;
        }

        private static IServiceCollection AutoRegisterWithLifetime(this IServiceCollection serviceCollection, IEnumerable<IServiceRegistrar> serviceRegistrars, IEnumerable<Type> serviceTypes,
            Action<IServiceRegistrarOptions>? configure, ServiceLifetime lifetime)
        {
            _ = serviceCollection.WhenNotNull();
            var allServiceRegistrars = serviceRegistrars.WhenNotNullOrEmpty().AsReadOnlyCollection();
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty().AsReadOnlyCollection();

            foreach (var serviceRegistrar in allServiceRegistrars)
            {
                AutoRegisterWithLifetime(serviceCollection, serviceRegistrar, allServiceTypes, configure, lifetime);
            }

            return serviceCollection;
        }

        private static IServiceCollection AutoRegisterWithLifetime<TServiceRegistrar>(this IServiceCollection serviceCollection, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions>? configure, ServiceLifetime lifetime)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            _ = serviceCollection.WhenNotNull();
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty().AsReadOnlyCollection();
            _ = constructorArgsResolver.WhenNotNull();

            return AutoRegisterWithLifetime(serviceCollection, new TServiceRegistrar(), allServiceTypes, constructorArgsResolver, configure, lifetime);
        }

        private static IServiceCollection AutoRegisterWithLifetime(this IServiceCollection serviceCollection, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions>? configure, ServiceLifetime lifetime)
        {
            _ = serviceCollection.WhenNotNull();
            _ = serviceRegistrar.WhenNotNull();
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty().AsReadOnlyCollection();
            _ = constructorArgsResolver.WhenNotNull();

            serviceRegistrar.AutoRegisterServices(
                allServiceTypes,
                (serviceType, implementationType) =>
                {
                    object CreateImplementation(IServiceProvider provider)
                    {
                        var args = constructorArgsResolver.Invoke(provider, implementationType);
                        return Activator.CreateInstance(implementationType, [.. args])!;
                    }

                    // Call AddScoped(), AddTransient(), AddSingleton() with the factory as required
                    ServiceCreatorsByFactory[lifetime].Invoke(serviceCollection, serviceType, CreateImplementation);
                },
                configure);

            return serviceCollection;
        }

        private static IServiceCollection AutoRegisterWithLifetime(this IServiceCollection serviceCollection, IEnumerable<IServiceRegistrar> serviceRegistrars, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions>? configure, ServiceLifetime lifetime)
        {
            _ = serviceCollection.WhenNotNull();
            var allServiceRegistrars = serviceRegistrars.WhenNotNullOrEmpty();
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty().AsReadOnlyCollection();
            _ = constructorArgsResolver.WhenNotNull();

            foreach (var serviceRegistrar in allServiceRegistrars)
            {
                AutoRegisterWithLifetime(serviceCollection, serviceRegistrar, allServiceTypes, constructorArgsResolver, configure, lifetime);
            }

            return serviceCollection;
        }
    }
}
