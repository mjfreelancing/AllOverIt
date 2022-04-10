using System;
using System.Collections.Generic;
using AllOverIt.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.DependencyInjection.Tests.Helpers
{
    internal static class DependencyHelper
    {
        internal static IServiceCollection AutoRegisterUsingMode<TServiceRegistrar, TServiceType>(ServiceLifetime mode, IServiceCollection services,
            Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return mode switch
            {
                ServiceLifetime.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton<TServiceRegistrar, TServiceType>(services, configure),
                
                ServiceLifetime.Scoped => ServiceCollectionExtensions.AutoRegisterScoped<TServiceRegistrar, TServiceType>(services, configure),

                ServiceLifetime.Transient => ServiceCollectionExtensions.AutoRegisterTransient<TServiceRegistrar, TServiceType>(services, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        internal static IServiceCollection AutoRegisterUsingMode<TServiceRegistrar, TServiceType>(this IServiceCollection services, ServiceLifetime mode,
            Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return AutoRegisterUsingMode<TServiceRegistrar, TServiceType>(mode, services, configure);
        }

        internal static IServiceCollection AutoRegisterUsingMode<TServiceRegistrar>(ServiceLifetime mode, IServiceCollection services,
            IEnumerable<Type> serviceTypes, Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return mode switch
            {
                ServiceLifetime.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton<TServiceRegistrar>(services, serviceTypes, configure),

                ServiceLifetime.Scoped => ServiceCollectionExtensions.AutoRegisterScoped<TServiceRegistrar>(services, serviceTypes, configure),

                ServiceLifetime.Transient => ServiceCollectionExtensions.AutoRegisterTransient<TServiceRegistrar>(services, serviceTypes, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        internal static IServiceCollection AutoRegisterUsingMode<TServiceRegistrar>(this IServiceCollection services, ServiceLifetime mode,
            IEnumerable<Type> serviceTypes, Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return AutoRegisterUsingMode<TServiceRegistrar>(mode, services, serviceTypes, configure);
        }

        internal static IServiceCollection AutoRegisterUsingMode<TServiceType>(ServiceLifetime mode, IServiceCollection services,
            IServiceRegistrar serviceRegistrar, Action<IServiceRegistrarOptions> configure = default)
        {
            return mode switch
            {
                ServiceLifetime.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton<TServiceType>(services, serviceRegistrar, configure),

                ServiceLifetime.Scoped => ServiceCollectionExtensions.AutoRegisterScoped<TServiceType>(services, serviceRegistrar, configure),

                ServiceLifetime.Transient => ServiceCollectionExtensions.AutoRegisterTransient<TServiceType>(services, serviceRegistrar, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        internal static IServiceCollection AutoRegisterUsingMode<TServiceType>(this IServiceCollection services, ServiceLifetime mode,
            IServiceRegistrar serviceRegistrar, Action<IServiceRegistrarOptions> configure = default)
        {
            return AutoRegisterUsingMode<TServiceType>(mode, services, serviceRegistrar, configure);
        }

        internal static IServiceCollection AutoRegisterUsingMode(ServiceLifetime mode, IServiceCollection services, IServiceRegistrar serviceRegistrar,
            IEnumerable<Type> serviceTypes, Action<IServiceRegistrarOptions> configure = default)
        {
            return mode switch
            {
                ServiceLifetime.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton(services, serviceRegistrar, serviceTypes, configure),

                ServiceLifetime.Scoped => ServiceCollectionExtensions.AutoRegisterScoped(services, serviceRegistrar, serviceTypes, configure),

                ServiceLifetime.Transient => ServiceCollectionExtensions.AutoRegisterTransient(services, serviceRegistrar, serviceTypes, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        internal static IServiceCollection AutoRegisterUsingMode(this IServiceCollection services, ServiceLifetime mode, IServiceRegistrar serviceRegistrar,
            IEnumerable<Type> serviceTypes, Action<IServiceRegistrarOptions> configure = default)
        {
            return AutoRegisterUsingMode(mode, services, serviceRegistrar, serviceTypes, configure);
        }

        internal static IServiceCollection AutoRegisterUsingMode(ServiceLifetime mode, IServiceCollection services, IEnumerable<IServiceRegistrar> serviceRegistrars,
            IEnumerable<Type> serviceTypes, Action<IServiceRegistrarOptions> configure = default)
        {
            return mode switch
            {
                ServiceLifetime.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton(services, serviceRegistrars, serviceTypes, configure),

                ServiceLifetime.Scoped => ServiceCollectionExtensions.AutoRegisterScoped(services, serviceRegistrars, serviceTypes, configure),

                ServiceLifetime.Transient => ServiceCollectionExtensions.AutoRegisterTransient(services, serviceRegistrars, serviceTypes, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        internal static IServiceCollection AutoRegisterUsingMode(this IServiceCollection services, ServiceLifetime mode, IEnumerable<IServiceRegistrar> serviceRegistrars,
            IEnumerable<Type> serviceTypes, Action<IServiceRegistrarOptions> configure = default)
        {
            return AutoRegisterUsingMode(mode, services, serviceRegistrars, serviceTypes, configure);
        }

        internal static IServiceCollection AutoRegisterUsingMode<TServiceRegistrar>(ServiceLifetime mode, IServiceCollection services, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return mode switch
            {
                ServiceLifetime.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton<TServiceRegistrar>(services, serviceTypes, constructorArgsResolver, configure),

                ServiceLifetime.Scoped => ServiceCollectionExtensions.AutoRegisterScoped<TServiceRegistrar>(services, serviceTypes, constructorArgsResolver, configure),

                ServiceLifetime.Transient => ServiceCollectionExtensions.AutoRegisterTransient<TServiceRegistrar>(services, serviceTypes, constructorArgsResolver, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        internal static IServiceCollection AutoRegisterUsingMode<TServiceRegistrar>(this IServiceCollection services, ServiceLifetime mode, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return AutoRegisterUsingMode<TServiceRegistrar>(mode, services, serviceTypes, constructorArgsResolver, configure);
        }

        internal static IServiceCollection AutoRegisterUsingMode(ServiceLifetime mode, IServiceCollection services, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
        {
            return mode switch
            {
                ServiceLifetime.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton(services, serviceRegistrar, serviceTypes, constructorArgsResolver, configure),

                ServiceLifetime.Scoped => ServiceCollectionExtensions.AutoRegisterScoped(services, serviceRegistrar, serviceTypes, constructorArgsResolver, configure),

                ServiceLifetime.Transient => ServiceCollectionExtensions.AutoRegisterTransient(services, serviceRegistrar, serviceTypes, constructorArgsResolver, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        internal static IServiceCollection AutoRegisterUsingMode(this IServiceCollection services, ServiceLifetime mode, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
        {
            return AutoRegisterUsingMode(mode, services, serviceRegistrar, serviceTypes, constructorArgsResolver, configure);
        }

        internal static IServiceCollection AutoRegisterUsingMode(ServiceLifetime mode, IServiceCollection services, IEnumerable<IServiceRegistrar> serviceRegistrars, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
        {
            return mode switch
            {
                ServiceLifetime.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton(services, serviceRegistrars, serviceTypes, constructorArgsResolver, configure),

                ServiceLifetime.Scoped => ServiceCollectionExtensions.AutoRegisterScoped(services, serviceRegistrars, serviceTypes, constructorArgsResolver, configure),

                ServiceLifetime.Transient => ServiceCollectionExtensions.AutoRegisterTransient(services, serviceRegistrars, serviceTypes, constructorArgsResolver, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        internal static IServiceCollection AutoRegisterUsingMode(this IServiceCollection services, ServiceLifetime mode, IEnumerable<IServiceRegistrar> serviceRegistrars, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
        {
            return AutoRegisterUsingMode(mode, services, serviceRegistrars, serviceTypes, constructorArgsResolver, configure);
        }
    }
}