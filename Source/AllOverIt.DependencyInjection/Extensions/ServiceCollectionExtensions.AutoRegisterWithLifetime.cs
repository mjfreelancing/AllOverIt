using AllOverIt.Assertion;
using AllOverIt.DependencyInjection.Exceptions;
using AllOverIt.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.DependencyInjection.Extensions
{
    // Helper methods for use by AutoRegisterScoped(), AutoRegisterSingleton(), and AutoRegisterTransient()
    public static partial class ServiceCollectionExtensions
    {
        private static readonly IDictionary<ServiceLifetime, Action<IServiceCollection, Type, Type>> ServiceCreatorsByType =
            new Dictionary<ServiceLifetime, Action<IServiceCollection, Type, Type>>
            {
                { ServiceLifetime.Scoped, (serviceCollection, serviceType, implementationType) => serviceCollection.AddScoped(serviceType, implementationType) },
                { ServiceLifetime.Transient, (serviceCollection, serviceType, implementationType) => serviceCollection.AddTransient(serviceType, implementationType) },
                { ServiceLifetime.Singleton, (serviceCollection, serviceType, implementationType) => serviceCollection.AddSingleton(serviceType, implementationType) }
            };

        private static readonly IDictionary<ServiceLifetime, Action<IServiceCollection, Type, Func<IServiceProvider, object>>> ServiceCreatorsByFactory =
            new Dictionary<ServiceLifetime, Action<IServiceCollection, Type, Func<IServiceProvider, object>>>
            {
                { ServiceLifetime.Scoped, (serviceCollection, serviceType, factory) => serviceCollection.AddScoped(serviceType, factory) },
                { ServiceLifetime.Transient, (serviceCollection, serviceType, factory) => serviceCollection.AddTransient(serviceType, factory) },
                { ServiceLifetime.Singleton, (serviceCollection, serviceType, factory) => serviceCollection.AddSingleton(serviceType, factory) }
            };

        private static IServiceCollection AutoRegisterWithLifetime<TServiceRegistrar, TServiceType>(IServiceCollection serviceCollection, Action<IServiceRegistrarOptions> configure,
            ServiceLifetime lifetime)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            _ = serviceCollection.WhenNotNull(nameof(serviceCollection));

            return AutoRegisterWithLifetime(serviceCollection, new TServiceRegistrar(), [typeof(TServiceType)], configure, lifetime);
        }

        private static IServiceCollection AutoRegisterWithLifetime<TServiceRegistrar>(this IServiceCollection serviceCollection, IEnumerable<Type> serviceTypes,
            Action<IServiceRegistrarOptions> configure, ServiceLifetime lifetime)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            _ = serviceCollection.WhenNotNull(nameof(serviceCollection));
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty(nameof(serviceTypes)).AsReadOnlyCollection();

            return AutoRegisterWithLifetime(serviceCollection, new TServiceRegistrar(), allServiceTypes, configure, lifetime);
        }

        private static IServiceCollection AutoRegisterWithLifetime<TServiceType>(this IServiceCollection serviceCollection, IServiceRegistrar serviceRegistrar,
            Action<IServiceRegistrarOptions> configure, ServiceLifetime lifetime)
        {
            _ = serviceCollection.WhenNotNull(nameof(serviceCollection));
            _ = serviceRegistrar.WhenNotNull(nameof(serviceRegistrar));

            return AutoRegisterWithLifetime(serviceCollection, serviceRegistrar, [typeof(TServiceType)], configure, lifetime);
        }

        private static IServiceCollection AutoRegisterWithLifetime(this IServiceCollection serviceCollection, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            Action<IServiceRegistrarOptions> configure, ServiceLifetime lifetime)
        {
            _ = serviceCollection.WhenNotNull(nameof(serviceCollection));
            _ = serviceRegistrar.WhenNotNull(nameof(serviceRegistrar));
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty(nameof(serviceTypes)).AsReadOnlyCollection();

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

                        if (firstMismatch != null)
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
            Action<IServiceRegistrarOptions> configure, ServiceLifetime lifetime)
        {
            _ = serviceCollection.WhenNotNull(nameof(serviceCollection));
            var allServiceRegistrars = serviceRegistrars.WhenNotNullOrEmpty(nameof(serviceRegistrars)).AsReadOnlyCollection();
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty(nameof(serviceTypes)).AsReadOnlyCollection();

            foreach (var serviceRegistrar in allServiceRegistrars)
            {
                AutoRegisterWithLifetime(serviceCollection, serviceRegistrar, allServiceTypes, configure, lifetime);
            }

            return serviceCollection;
        }

        private static IServiceCollection AutoRegisterWithLifetime<TServiceRegistrar>(this IServiceCollection serviceCollection, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure, ServiceLifetime lifetime)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            _ = serviceCollection.WhenNotNull(nameof(serviceCollection));
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty(nameof(serviceTypes)).AsReadOnlyCollection();
            _ = constructorArgsResolver.WhenNotNull(nameof(constructorArgsResolver));

            return AutoRegisterWithLifetime(serviceCollection, new TServiceRegistrar(), allServiceTypes, constructorArgsResolver, configure, lifetime);
        }

        private static IServiceCollection AutoRegisterWithLifetime(this IServiceCollection serviceCollection, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure, ServiceLifetime lifetime)
        {
            _ = serviceCollection.WhenNotNull(nameof(serviceCollection));
            _ = serviceRegistrar.WhenNotNull(nameof(serviceRegistrar));
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty(nameof(serviceTypes)).AsReadOnlyCollection();
            _ = constructorArgsResolver.WhenNotNull(nameof(constructorArgsResolver));

            serviceRegistrar.AutoRegisterServices(
                allServiceTypes,
                (serviceType, implementationType) =>
                {
                    object CreateImplementation(IServiceProvider provider)
                    {
                        var args = constructorArgsResolver.Invoke(provider, implementationType);
                        return Activator.CreateInstance(implementationType, args.ToArray());
                    }

                    // Call AddScoped(), AddTransient(), AddSingleton() with the factory as required
                    ServiceCreatorsByFactory[lifetime].Invoke(serviceCollection, serviceType, CreateImplementation);
                },
                configure);

            return serviceCollection;
        }

        private static IServiceCollection AutoRegisterWithLifetime(this IServiceCollection serviceCollection, IEnumerable<IServiceRegistrar> serviceRegistrars, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure, ServiceLifetime lifetime)
        {
            _ = serviceCollection.WhenNotNull(nameof(serviceCollection));
            var allServiceRegistrars = serviceRegistrars.WhenNotNullOrEmpty(nameof(serviceRegistrars));
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty(nameof(serviceTypes)).AsReadOnlyCollection();
            _ = constructorArgsResolver.WhenNotNull(nameof(constructorArgsResolver));

            foreach (var serviceRegistrar in allServiceRegistrars)
            {
                AutoRegisterWithLifetime(serviceCollection, serviceRegistrar, allServiceTypes, constructorArgsResolver, configure, lifetime);
            }

            return serviceCollection;
        }
    }
}
