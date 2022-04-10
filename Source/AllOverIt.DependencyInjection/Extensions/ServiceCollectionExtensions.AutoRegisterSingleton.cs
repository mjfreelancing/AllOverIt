﻿using AllOverIt.Assertion;
using AllOverIt.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.DependencyInjection.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AutoRegisterSingleton<TServiceRegistrar, TServiceType>(this IServiceCollection services, Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            _ = services.WhenNotNull(nameof(services));

            return AutoRegisterSingleton(services, new TServiceRegistrar(), new[] {typeof(TServiceType)}, configure);
        }

        public static IServiceCollection AutoRegisterSingleton<TServiceRegistrar>(this IServiceCollection services, IEnumerable<Type> serviceTypes,
            Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            _ = services.WhenNotNull(nameof(services));
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty(nameof(serviceTypes)).AsReadOnlyCollection();

            return AutoRegisterSingleton(services, new TServiceRegistrar(), allServiceTypes, configure);
        }

        public static IServiceCollection AutoRegisterSingleton<TServiceType>(this IServiceCollection services, IServiceRegistrar serviceRegistrar,
            Action<IServiceRegistrarOptions> configure = default)
        {
            _ = services.WhenNotNull(nameof(services));
            _ = serviceRegistrar.WhenNotNull(nameof(serviceRegistrar));

            return AutoRegisterSingleton(services, serviceRegistrar, new[] {typeof(TServiceType)}, configure);
        }

        public static IServiceCollection AutoRegisterSingleton(this IServiceCollection services, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            Action<IServiceRegistrarOptions> configure = default)
        {
            _ = services.WhenNotNull(nameof(services));
            _ = serviceRegistrar.WhenNotNull(nameof(serviceRegistrar));
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty(nameof(serviceTypes)).AsReadOnlyCollection();

            serviceRegistrar.AutoRegisterServices(
                allServiceTypes,
                (serviceType, implementationType) => services.AddSingleton(serviceType, implementationType),
                configure);

            return services;
        }

        public static IServiceCollection AutoRegisterSingleton(this IServiceCollection services, IEnumerable<IServiceRegistrar> serviceRegistrars, IEnumerable<Type> serviceTypes,
            Action<IServiceRegistrarOptions> configure = default)
        {
            _ = services.WhenNotNull(nameof(services));
            var allServiceRegistrars = serviceRegistrars.WhenNotNullOrEmpty(nameof(serviceRegistrars)).AsReadOnlyCollection();
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty(nameof(serviceTypes)).AsReadOnlyCollection();

            foreach (var serviceRegistrar in allServiceRegistrars)
            {
                AutoRegisterSingleton(services, serviceRegistrar, allServiceTypes, configure);
            }

            return services;
        }

        public static IServiceCollection AutoRegisterSingleton<TServiceRegistrar>(this IServiceCollection services, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            _ = services.WhenNotNull(nameof(services));
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty(nameof(serviceTypes)).AsReadOnlyCollection();
            _ = constructorArgsResolver.WhenNotNull(nameof(constructorArgsResolver));

            return AutoRegisterSingleton(services, new TServiceRegistrar(), allServiceTypes, constructorArgsResolver, configure);
        }

        public static IServiceCollection AutoRegisterSingleton(this IServiceCollection services, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
        {
            _ = services.WhenNotNull(nameof(services));
            _ = serviceRegistrar.WhenNotNull(nameof(serviceRegistrar));
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty(nameof(serviceTypes)).AsReadOnlyCollection();
            _ = constructorArgsResolver.WhenNotNull(nameof(constructorArgsResolver));

            serviceRegistrar.AutoRegisterServices(
                allServiceTypes,
                (serviceType, implementationType) => services.AddSingleton(serviceType, provider =>
                {
                    var args = constructorArgsResolver.Invoke(provider, implementationType);
                    return Activator.CreateInstance(implementationType, args.ToArray());
                }),
                configure);

            return services;
        }

        public static IServiceCollection AutoRegisterSingleton(this IServiceCollection services, IEnumerable<IServiceRegistrar> serviceRegistrars, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
        {
            _ = services.WhenNotNull(nameof(services));
            var allServiceRegistrars = serviceRegistrars.WhenNotNullOrEmpty(nameof(serviceRegistrars));
            var allServiceTypes = serviceTypes.WhenNotNullOrEmpty(nameof(serviceTypes)).AsReadOnlyCollection();
            _ = constructorArgsResolver.WhenNotNull(nameof(constructorArgsResolver));

            foreach (var serviceRegistrar in allServiceRegistrars)
            {
                AutoRegisterSingleton(services, serviceRegistrar, allServiceTypes, constructorArgsResolver, configure);
            }

            return services;
        }
    }
}
