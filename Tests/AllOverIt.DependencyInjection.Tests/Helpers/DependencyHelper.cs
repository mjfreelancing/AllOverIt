using System;
using System.Collections.Generic;
using AllOverIt.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.DependencyInjection.Tests.Helpers
{
    internal static class DependencyHelper
    {
        public static IServiceCollection AutoRegisterUsingMode<TServiceRegistrar, TServiceType>(RegistrationMode mode, IServiceCollection services,
            Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return mode switch
            {
                RegistrationMode.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton<TServiceRegistrar, TServiceType>(services, configure),
                
                RegistrationMode.Scoped => ServiceCollectionExtensions.AutoRegisterScoped<TServiceRegistrar, TServiceType>(services, configure),

                RegistrationMode.Transient => ServiceCollectionExtensions.AutoRegisterTransient<TServiceRegistrar, TServiceType>(services, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        public static IServiceCollection AutoRegisterUsingMode<TServiceRegistrar, TServiceType>(this IServiceCollection services, RegistrationMode mode,
            Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return AutoRegisterUsingMode<TServiceRegistrar, TServiceType>(mode, services, configure);
        }

        public static IServiceCollection AutoRegisterUsingMode<TServiceRegistrar>(RegistrationMode mode, IServiceCollection services,
            IEnumerable<Type> serviceTypes, Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return mode switch
            {
                RegistrationMode.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton<TServiceRegistrar>(services, serviceTypes, configure),

                RegistrationMode.Scoped => ServiceCollectionExtensions.AutoRegisterScoped<TServiceRegistrar>(services, serviceTypes, configure),

                RegistrationMode.Transient => ServiceCollectionExtensions.AutoRegisterTransient<TServiceRegistrar>(services, serviceTypes, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        public static IServiceCollection AutoRegisterUsingMode<TServiceRegistrar>(this IServiceCollection services, RegistrationMode mode,
            IEnumerable<Type> serviceTypes, Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return AutoRegisterUsingMode<TServiceRegistrar>(mode, services, serviceTypes, configure);
        }

        public static IServiceCollection AutoRegisterUsingMode<TServiceType>(RegistrationMode mode, IServiceCollection services,
            IServiceRegistrar serviceRegistrar, Action<IServiceRegistrarOptions> configure = default)
        {
            return mode switch
            {
                RegistrationMode.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton<TServiceType>(services, serviceRegistrar, configure),

                RegistrationMode.Scoped => ServiceCollectionExtensions.AutoRegisterScoped<TServiceType>(services, serviceRegistrar, configure),

                RegistrationMode.Transient => ServiceCollectionExtensions.AutoRegisterTransient<TServiceType>(services, serviceRegistrar, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        public static IServiceCollection AutoRegisterUsingMode<TServiceType>(this IServiceCollection services, RegistrationMode mode,
            IServiceRegistrar serviceRegistrar, Action<IServiceRegistrarOptions> configure = default)
        {
            return AutoRegisterUsingMode<TServiceType>(mode, services, serviceRegistrar, configure);
        }

        public static IServiceCollection AutoRegisterUsingMode(RegistrationMode mode, IServiceCollection services, IServiceRegistrar serviceRegistrar,
            IEnumerable<Type> serviceTypes, Action<IServiceRegistrarOptions> configure = default)
        {
            return mode switch
            {
                RegistrationMode.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton(services, serviceRegistrar, serviceTypes, configure),

                RegistrationMode.Scoped => ServiceCollectionExtensions.AutoRegisterScoped(services, serviceRegistrar, serviceTypes, configure),

                RegistrationMode.Transient => ServiceCollectionExtensions.AutoRegisterTransient(services, serviceRegistrar, serviceTypes, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        public static IServiceCollection AutoRegisterUsingMode(this IServiceCollection services, RegistrationMode mode, IServiceRegistrar serviceRegistrar,
            IEnumerable<Type> serviceTypes, Action<IServiceRegistrarOptions> configure = default)
        {
            return AutoRegisterUsingMode(mode, services, serviceRegistrar, serviceTypes, configure);
        }

        public static IServiceCollection AutoRegisterUsingMode(RegistrationMode mode, IServiceCollection services, IEnumerable<IServiceRegistrar> serviceRegistrars,
            IEnumerable<Type> serviceTypes, Action<IServiceRegistrarOptions> configure = default)
        {
            return mode switch
            {
                RegistrationMode.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton(services, serviceRegistrars, serviceTypes, configure),

                RegistrationMode.Scoped => ServiceCollectionExtensions.AutoRegisterScoped(services, serviceRegistrars, serviceTypes, configure),

                RegistrationMode.Transient => ServiceCollectionExtensions.AutoRegisterTransient(services, serviceRegistrars, serviceTypes, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        public static IServiceCollection AutoRegisterUsingMode(this IServiceCollection services, RegistrationMode mode, IEnumerable<IServiceRegistrar> serviceRegistrars,
            IEnumerable<Type> serviceTypes, Action<IServiceRegistrarOptions> configure = default)
        {
            return AutoRegisterUsingMode(mode, services, serviceRegistrars, serviceTypes, configure);
        }

        public static IServiceCollection AutoRegisterUsingMode<TServiceRegistrar>(RegistrationMode mode, IServiceCollection services, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return mode switch
            {
                RegistrationMode.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton<TServiceRegistrar>(services, serviceTypes, constructorArgsResolver, configure),

                RegistrationMode.Scoped => ServiceCollectionExtensions.AutoRegisterScoped<TServiceRegistrar>(services, serviceTypes, constructorArgsResolver, configure),

                RegistrationMode.Transient => ServiceCollectionExtensions.AutoRegisterTransient<TServiceRegistrar>(services, serviceTypes, constructorArgsResolver, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        public static IServiceCollection AutoRegisterUsingMode<TServiceRegistrar>(this IServiceCollection services, RegistrationMode mode, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return AutoRegisterUsingMode<TServiceRegistrar>(mode, services, serviceTypes, constructorArgsResolver, configure);
        }

        public static IServiceCollection AutoRegisterUsingMode(RegistrationMode mode, IServiceCollection services, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
        {
            return mode switch
            {
                RegistrationMode.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton(services, serviceRegistrar, serviceTypes, constructorArgsResolver, configure),

                RegistrationMode.Scoped => ServiceCollectionExtensions.AutoRegisterScoped(services, serviceRegistrar, serviceTypes, constructorArgsResolver, configure),

                RegistrationMode.Transient => ServiceCollectionExtensions.AutoRegisterTransient(services, serviceRegistrar, serviceTypes, constructorArgsResolver, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        public static IServiceCollection AutoRegisterUsingMode(this IServiceCollection services, RegistrationMode mode, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
        {
            return AutoRegisterUsingMode(mode, services, serviceRegistrar, serviceTypes, constructorArgsResolver, configure);
        }

        public static IServiceCollection AutoRegisterUsingMode(RegistrationMode mode, IServiceCollection services, IEnumerable<IServiceRegistrar> serviceRegistrars, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
        {
            return mode switch
            {
                RegistrationMode.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton(services, serviceRegistrars, serviceTypes, constructorArgsResolver, configure),

                RegistrationMode.Scoped => ServiceCollectionExtensions.AutoRegisterScoped(services, serviceRegistrars, serviceTypes, constructorArgsResolver, configure),

                RegistrationMode.Transient => ServiceCollectionExtensions.AutoRegisterTransient(services, serviceRegistrars, serviceTypes, constructorArgsResolver, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        public static IServiceCollection AutoRegisterUsingMode(this IServiceCollection services, RegistrationMode mode, IEnumerable<IServiceRegistrar> serviceRegistrars, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
        {
            return AutoRegisterUsingMode(mode, services, serviceRegistrars, serviceTypes, constructorArgsResolver, configure);
        }
    }
}