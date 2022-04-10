using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace AllOverIt.DependencyInjection.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        /// <summary>Auto registers a <typeparamref name="TServiceType" /> type with a lifetime of transient by scanning for inherited types within the assembly containing the
        /// <typeparamref name="TServiceRegistrar" /> type. If the <typeparamref name="TServiceType" /> is an abstract class then all inherited classes will be registered.
        /// If the <typeparamref name="TServiceType" /> type is an interface then all inherited classes will be registered against all of its implemented interfaces.</summary>
        /// <typeparam name="TServiceRegistrar">The registrar used to register all types within the contained assembly.</typeparam>
        /// <typeparam name="TServiceType">The service type to register classes against.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="configure">Optional configuration options that provide the ability to exclude or otherwise filter service or implementation types.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection AutoRegisterTransient<TServiceRegistrar, TServiceType>(this IServiceCollection serviceCollection, Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return AutoRegisterWithLifetime<TServiceRegistrar, TServiceType>(serviceCollection, configure, ServiceLifetime.Transient);
        }

        /// <summary>Auto registers one or more service types with a lifetime of transient by scanning for inherited types within the assembly containing the
        /// <typeparamref name="TServiceRegistrar" /> type. If a service type is an abstract class then all inherited classes will be registered.
        /// If a service type is an interface then all inherited classes will be registered against all of its implemented interfaces.</summary>
        /// <typeparam name="TServiceRegistrar">The registrar used to register all types within the contained assembly.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="serviceTypes">One or more service types (abstract class or interface) to register classes against.</param>
        /// <param name="configure">Optional configuration options that provide the ability to exclude or otherwise filter service or implementation types.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection AutoRegisterTransient<TServiceRegistrar>(this IServiceCollection serviceCollection, IEnumerable<Type> serviceTypes,
            Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return AutoRegisterWithLifetime<TServiceRegistrar>(serviceCollection, serviceTypes, configure, ServiceLifetime.Transient);
        }

        /// <summary>Auto registers a <typeparamref name="TServiceType" /> type with a lifetime of transient by scanning for inherited types within the assembly containing the
        /// provided registrar. If the <typeparamref name="TServiceType" /> is an abstract class then all inherited classes will be registered.
        /// If the <typeparamref name="TServiceType" /> type is an interface then all inherited classes will be registered against all of its implemented interfaces.</summary>
        /// <typeparam name="TServiceType">The service type to register classes against.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="serviceRegistrar">The registrar used to register all types within the contained assembly.</param>
        /// <param name="configure">Optional configuration options that provide the ability to exclude or otherwise filter service or implementation types.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection AutoRegisterTransient<TServiceType>(this IServiceCollection serviceCollection, IServiceRegistrar serviceRegistrar,
            Action<IServiceRegistrarOptions> configure = default)
        {
            return AutoRegisterWithLifetime<TServiceType>(serviceCollection, serviceRegistrar, configure, ServiceLifetime.Transient);
        }

        /// <summary>Auto registers one or more service types with a lifetime of transient by scanning for inherited types within the assembly containing the
        /// provided registrar. If a service type is an abstract class then all inherited classes will be registered.
        /// If a service type is an interface then all inherited classes will be registered against all of its implemented interfaces.</summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="serviceRegistrar">The registrar used to register all types within the contained assembly.</param>
        /// <param name="serviceTypes">One or more service types (abstract class or interface) to register classes against.</param>
        /// <param name="configure">Optional configuration options that provide the ability to exclude or otherwise filter service or implementation types.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection AutoRegisterTransient(this IServiceCollection serviceCollection, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            Action<IServiceRegistrarOptions> configure = default)
        {
            return AutoRegisterWithLifetime(serviceCollection, serviceRegistrar, serviceTypes, configure, ServiceLifetime.Transient);
        }

        /// <summary>Auto registers one or more service types with a lifetime of transient by scanning for inherited types within the assembly containing the
        /// provided registrars. If a service type is an abstract class then all inherited classes will be registered.
        /// If a service type is an interface then all inherited classes will be registered against all of its implemented interfaces.</summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="serviceRegistrars">One or more registrars used to register all types within the contained assembly.</param>
        /// <param name="serviceTypes">One or more service types (abstract class or interface) to register classes against.</param>
        /// <param name="configure">Optional configuration options that provide the ability to exclude or otherwise filter service or implementation types.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection AutoRegisterTransient(this IServiceCollection serviceCollection, IEnumerable<IServiceRegistrar> serviceRegistrars, IEnumerable<Type> serviceTypes,
            Action<IServiceRegistrarOptions> configure = default)
        {
            return AutoRegisterWithLifetime(serviceCollection, serviceRegistrars, serviceTypes, configure, ServiceLifetime.Transient);
        }

        /// <summary>Auto registers one or more service types with a lifetime of transient by scanning for inherited types within the assembly containing the
        /// <typeparamref name="TServiceRegistrar" /> type. If a service type is an abstract class then all inherited classes will be registered.
        /// If a service type is an interface then all inherited classes will be registered against all of its implemented interfaces.</summary>
        /// <typeparam name="TServiceRegistrar">The registrar used to register all types within the contained assembly.</typeparam>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="serviceTypes">One or more service types (abstract class or interface) to register classes against.</param>
        /// <param name="constructorArgsResolver">Provides the constructor arguments to be provided to each instantiated implementation type.</param>
        /// <param name="configure">Optional configuration options that provide the ability to exclude or otherwise filter service or implementation types.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection AutoRegisterTransient<TServiceRegistrar>(this IServiceCollection serviceCollection, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return AutoRegisterWithLifetime<TServiceRegistrar>(serviceCollection, serviceTypes, constructorArgsResolver, configure, ServiceLifetime.Transient);
        }

        /// <summary>Auto registers one or more service types with a lifetime of transient by scanning for inherited types within the assembly containing the
        /// provided registrar. If a service type is an abstract class then all inherited classes will be registered.
        /// If a service type is an interface then all inherited classes will be registered against all of its implemented interfaces.</summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="serviceRegistrar">The registrar used to register all types within the contained assembly.</param>
        /// <param name="serviceTypes">One or more service types (abstract class or interface) to register classes against.</param>
        /// <param name="constructorArgsResolver">Provides the constructor arguments to be provided to each instantiated implementation type.</param>
        /// <param name="configure">Optional configuration options that provide the ability to exclude or otherwise filter service or implementation types.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection AutoRegisterTransient(this IServiceCollection serviceCollection, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
        {
            return AutoRegisterWithLifetime(serviceCollection, serviceRegistrar, serviceTypes, constructorArgsResolver, configure, ServiceLifetime.Transient);
        }

        /// <summary>Auto registers one or more service types with a lifetime of transient by scanning for inherited types within the assembly containing the
        /// provided registrar. If a service type is an abstract class then all inherited classes will be registered.
        /// If a service type is an interface then all inherited classes will be registered against all of its implemented interfaces.</summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <param name="serviceRegistrars">One or more registrars used to register all types within the contained assembly.</param>
        /// <param name="serviceTypes">One or more service types (abstract class or interface) to register classes against.</param>
        /// <param name="constructorArgsResolver">Provides the constructor arguments to be provided to each instantiated implementation type.</param>
        /// <param name="configure">Optional configuration options that provide the ability to exclude or otherwise filter service or implementation types.</param>
        /// <returns>The original service collection to allow for a fluent syntax.</returns>
        public static IServiceCollection AutoRegisterTransient(this IServiceCollection serviceCollection, IEnumerable<IServiceRegistrar> serviceRegistrars, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
        {
            return AutoRegisterWithLifetime(serviceCollection, serviceRegistrars, serviceTypes, constructorArgsResolver, configure, ServiceLifetime.Transient);
        }
    }
}
