using AllOverIt.Assertion;
using AllOverIt.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using AllOverIt.DependencyInjection.Exceptions;

namespace AllOverIt.DependencyInjection.Extensions
{
    // Helper methods for use by AutoRegisterScoped(), AutoRegisterSingleton(), and AutoRegisterTransient()
    public static partial class ServiceCollectionExtensions
    {
        private static IServiceCollection AutoRegisterWithLifetime<TServiceRegistrar, TServiceType>(IServiceCollection serviceCollection, Action<IServiceRegistrarOptions> configure,
            ServiceLifetime lifetime)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            _ = serviceCollection.WhenNotNull(nameof(serviceCollection));

            return AutoRegisterWithLifetime(serviceCollection, new TServiceRegistrar(), new[] { typeof(TServiceType) }, configure, lifetime);
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

            return AutoRegisterWithLifetime(serviceCollection, serviceRegistrar, new[] { typeof(TServiceType) }, configure, lifetime);
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

                    switch (lifetime)
                    {
                        case ServiceLifetime.Scoped:
                            serviceCollection.AddScoped(serviceType, implementationType);
                            break;

                        case ServiceLifetime.Transient:
                            serviceCollection.AddTransient(serviceType, implementationType);
                            break;

                        case ServiceLifetime.Singleton:
                            serviceCollection.AddSingleton(serviceType, implementationType);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
                    }
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
                    switch (lifetime)
                    {
                        case ServiceLifetime.Scoped:
                            serviceCollection.AddScoped(serviceType, provider =>
                            {
                                var args = constructorArgsResolver.Invoke(provider, implementationType);
                                return Activator.CreateInstance(implementationType, args.ToArray());
                            });
                            break;

                        case ServiceLifetime.Transient:
                            serviceCollection.AddTransient(serviceType, provider =>
                            {
                                var args = constructorArgsResolver.Invoke(provider, implementationType);
                                return Activator.CreateInstance(implementationType, args.ToArray());
                            });
                            break;

                        case ServiceLifetime.Singleton:
                            serviceCollection.AddSingleton(serviceType, provider =>
                            {
                                var args = constructorArgsResolver.Invoke(provider, implementationType);
                                return Activator.CreateInstance(implementationType, args.ToArray());
                            });
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
                    }
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
