using AllOverIt.Assertion;
using AllOverIt.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.DependencyInjection.Extensions
{
    /// <summary>Provides dependency injection extension methods for <see cref="IServiceCollection"/>.</summary>
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>Auto registers a <typeparamref name="TServiceType" /> type with a lifetime of scoped by scanning for inherited types within the assembly containing the
        /// <typeparamref name="TServiceRegistrar" /> type. If the <typeparamref name="TServiceType" /> is an abstract class then all inherited classes will be registered.
        /// If the <typeparamref name="TServiceType" /> type is an interface then all inherited classes will be registered against all of its implemented interfaces.</summary>
        /// <typeparam name="TServiceRegistrar">The registrar used to register all types within the contained assembly.</typeparam>
        /// <typeparam name="TServiceType">The service type to register classes against.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="configure">Optional configuration options that provide the ability to exclude or otherwise filter service or implementation types.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection AutoRegisterScoped<TServiceRegistrar, TServiceType>(this IServiceCollection serviceCollection, Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            _ = serviceCollection.WhenNotNull(nameof(serviceCollection));

            return AutoRegisterScoped(serviceCollection, new TServiceRegistrar(), new[] { typeof(TServiceType) }, configure);
        }

        /// <summary>Auto registers one or more service types with a lifetime of scoped by scanning for inherited types within the assembly containing the
        /// <typeparamref name="TServiceRegistrar" /> type. If a service type is an abstract class then all inherited classes will be registered.
        /// If a service type is an interface then all inherited classes will be registered against all of its implemented interfaces.</summary>
        /// <typeparam name="TServiceRegistrar">The registrar used to register all types within the contained assembly.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="serviceTypes">One or more service types (abstract class or interface) to register classes against.</param>
        /// <param name="configure">Optional configuration options that provide the ability to exclude or otherwise filter service or implementation types.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection AutoRegisterScoped<TServiceRegistrar>(this IServiceCollection serviceCollection, IEnumerable<Type> serviceTypes,
            Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            _ = serviceCollection.WhenNotNull(nameof(serviceCollection));
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty(nameof(serviceTypes)).AsReadOnlyCollection();

            return AutoRegisterScoped(serviceCollection, new TServiceRegistrar(), allServiceTypes, configure);
        }

        /// <summary>Auto registers a <typeparamref name="TServiceType" /> type with a lifetime of scoped by scanning for inherited types within the assembly containing the
        /// provided registrar. If the <typeparamref name="TServiceType" /> is an abstract class then all inherited classes will be registered.
        /// If the <typeparamref name="TServiceType" /> type is an interface then all inherited classes will be registered against all of its implemented interfaces.</summary>
        /// <typeparam name="TServiceType">The service type to register classes against.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="serviceRegistrar">The registrar used to register all types within the contained assembly.</param>
        /// <param name="configure">Optional configuration options that provide the ability to exclude or otherwise filter service or implementation types.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection AutoRegisterScoped<TServiceType>(this IServiceCollection serviceCollection, IServiceRegistrar serviceRegistrar,
            Action<IServiceRegistrarOptions> configure = default)
        {
            _ = serviceCollection.WhenNotNull(nameof(serviceCollection));
            _ = serviceRegistrar.WhenNotNull(nameof(serviceRegistrar));

            return AutoRegisterScoped(serviceCollection, serviceRegistrar, new[] { typeof(TServiceType) }, configure);
        }

        /// <summary>Auto registers one or more service types with a lifetime of scoped by scanning for inherited types within the assembly containing the
        /// provided registrar. If a service type is an abstract class then all inherited classes will be registered.
        /// If a service type is an interface then all inherited classes will be registered against all of its implemented interfaces.</summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="serviceRegistrar">The registrar used to register all types within the contained assembly.</param>
        /// <param name="serviceTypes">One or more service types (abstract class or interface) to register classes against.</param>
        /// <param name="configure">Optional configuration options that provide the ability to exclude or otherwise filter service or implementation types.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection AutoRegisterScoped(this IServiceCollection serviceCollection, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            Action<IServiceRegistrarOptions> configure = default)
        {
            _ = serviceCollection.WhenNotNull(nameof(serviceCollection));
            _ = serviceRegistrar.WhenNotNull(nameof(serviceRegistrar));
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty(nameof(serviceTypes)).AsReadOnlyCollection();

            serviceRegistrar.AutoRegisterServices(
                allServiceTypes,
                (serviceType, implementationType) => serviceCollection.AddScoped(serviceType, implementationType),
                configure);

            return serviceCollection;
        }

        /// <summary>Auto registers one or more service types with a lifetime of scoped by scanning for inherited types within the assembly containing the
        /// provided registrars. If a service type is an abstract class then all inherited classes will be registered.
        /// If a service type is an interface then all inherited classes will be registered against all of its implemented interfaces.</summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="serviceRegistrars">One or more registrars used to register all types within the contained assembly.</param>
        /// <param name="serviceTypes">One or more service types (abstract class or interface) to register classes against.</param>
        /// <param name="configure">Optional configuration options that provide the ability to exclude or otherwise filter service or implementation types.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection AutoRegisterScoped(this IServiceCollection serviceCollection, IEnumerable<IServiceRegistrar> serviceRegistrars, IEnumerable<Type> serviceTypes,
            Action<IServiceRegistrarOptions> configure = default)
        {
            _ = serviceCollection.WhenNotNull(nameof(serviceCollection));
            var allServiceRegistrars = serviceRegistrars.WhenNotNullOrEmpty(nameof(serviceRegistrars)).AsReadOnlyCollection();
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty(nameof(serviceTypes)).AsReadOnlyCollection();

            foreach (var serviceRegistrar in allServiceRegistrars)
            {
                AutoRegisterScoped(serviceCollection, serviceRegistrar, allServiceTypes, configure);
            }

            return serviceCollection;
        }

        /// <summary>Auto registers one or more service types with a lifetime of scoped by scanning for inherited types within the assembly containing the
        /// <typeparamref name="TServiceRegistrar" /> type. If a service type is an abstract class then all inherited classes will be registered.
        /// If a service type is an interface then all inherited classes will be registered against all of its implemented interfaces.</summary>
        /// <typeparam name="TServiceRegistrar">The registrar used to register all types within the contained assembly.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="serviceTypes">One or more service types (abstract class or interface) to register classes against.</param>
        /// <param name="constructorArgsResolver">Provides the constructor arguments to be provided to each instantiated implementation type.</param>
        /// <param name="configure">Optional configuration options that provide the ability to exclude or otherwise filter service or implementation types.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection AutoRegisterScoped<TServiceRegistrar>(this IServiceCollection serviceCollection, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            _ = serviceCollection.WhenNotNull(nameof(serviceCollection));
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty(nameof(serviceTypes)).AsReadOnlyCollection();
            _ = constructorArgsResolver.WhenNotNull(nameof(constructorArgsResolver));

            return AutoRegisterScoped(serviceCollection, new TServiceRegistrar(), allServiceTypes, constructorArgsResolver, configure);
        }

        /// <summary>Auto registers one or more service types with a lifetime of scoped by scanning for inherited types within the assembly containing the
        /// provided registrar. If a service type is an abstract class then all inherited classes will be registered.
        /// If a service type is an interface then all inherited classes will be registered against all of its implemented interfaces.</summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="serviceRegistrar">The registrar used to register all types within the contained assembly.</param>
        /// <param name="serviceTypes">One or more service types (abstract class or interface) to register classes against.</param>
        /// <param name="constructorArgsResolver">Provides the constructor arguments to be provided to each instantiated implementation type.</param>
        /// <param name="configure">Optional configuration options that provide the ability to exclude or otherwise filter service or implementation types.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection AutoRegisterScoped(this IServiceCollection serviceCollection, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
        {
            _ = serviceCollection.WhenNotNull(nameof(serviceCollection));
            _ = serviceRegistrar.WhenNotNull(nameof(serviceRegistrar));
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty(nameof(serviceTypes)).AsReadOnlyCollection();
            _ = constructorArgsResolver.WhenNotNull(nameof(constructorArgsResolver));

            serviceRegistrar.AutoRegisterServices(
                allServiceTypes,
                (serviceType, implementationType) => serviceCollection.AddScoped(serviceType, provider =>
                {
                    var args = constructorArgsResolver.Invoke(provider, implementationType);
                    return Activator.CreateInstance(implementationType, args.ToArray());
                }),
                configure);

            return serviceCollection;
        }

        /// <summary>Auto registers one or more service types with a lifetime of scoped by scanning for inherited types within the assembly containing the
        /// provided registrar. If a service type is an abstract class then all inherited classes will be registered.
        /// If a service type is an interface then all inherited classes will be registered against all of its implemented interfaces.</summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="serviceRegistrars">One or more registrars used to register all types within the contained assembly.</param>
        /// <param name="serviceTypes">One or more service types (abstract class or interface) to register classes against.</param>
        /// <param name="constructorArgsResolver">Provides the constructor arguments to be provided to each instantiated implementation type.</param>
        /// <param name="configure">Optional configuration options that provide the ability to exclude or otherwise filter service or implementation types.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection AutoRegisterScoped(this IServiceCollection serviceCollection, IEnumerable<IServiceRegistrar> serviceRegistrars, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
        {
            _ = serviceCollection.WhenNotNull(nameof(serviceCollection));
            var allServiceRegistrars = serviceRegistrars.WhenNotNullOrEmpty(nameof(serviceRegistrars));
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty(nameof(serviceTypes)).AsReadOnlyCollection();
            _ = constructorArgsResolver.WhenNotNull(nameof(constructorArgsResolver));

            foreach (var serviceRegistrar in allServiceRegistrars)
            {
                AutoRegisterScoped(serviceCollection, serviceRegistrar, allServiceTypes, constructorArgsResolver, configure);
            }

            return serviceCollection;
        }
    }
}
