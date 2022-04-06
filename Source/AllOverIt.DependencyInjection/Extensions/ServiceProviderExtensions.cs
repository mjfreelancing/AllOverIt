using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.DependencyInjection.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static IServiceCollection AutoRegisterSingleton<TServiceRegistrar>(this IServiceCollection services, params Type[] serviceTypes)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            var serviceRegistrar = new TServiceRegistrar();
            return AutoRegisterSingleton(services, serviceRegistrar, serviceTypes, false);
        }

        public static IServiceCollection AutoRegisterSingleton<TServiceRegistrar>(this IServiceCollection services, IEnumerable<Type> serviceTypes,
            bool includeMatchingInterface = false)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            var serviceRegistrar = new TServiceRegistrar();
            return AutoRegisterSingleton(services, serviceRegistrar, serviceTypes, includeMatchingInterface);
        }

        public static IServiceCollection AutoRegisterSingleton<TServiceRegistrar>(this IServiceCollection services, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, bool includeMatchingInterface = false)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            var serviceRegistrar = new TServiceRegistrar();
            return AutoRegisterSingleton(services, serviceRegistrar, serviceTypes, constructorArgsResolver, includeMatchingInterface);
        }

        public static IServiceCollection AutoRegisterSingleton(this IServiceCollection services, IServiceRegistrar serviceRegistrar, params Type[] serviceTypes)
        {
            return AutoRegisterSingleton(services, serviceRegistrar, serviceTypes, false);
        }

        public static IServiceCollection AutoRegisterSingleton(this IServiceCollection services, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            bool includeMatchingInterface = false)
        {
            serviceRegistrar.AutoRegisterServices(
                serviceTypes,
                includeMatchingInterface,
                (serviceType, implementationType) => services.AddSingleton(serviceType, implementationType));

            return services;
        }

        public static IServiceCollection AutoRegisterSingleton(this IServiceCollection services, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, bool includeMatchingInterface = false)
        {
            serviceRegistrar.AutoRegisterServices(
                serviceTypes,
                includeMatchingInterface,
                (serviceType, implementationType) => services.AddSingleton(serviceType, provider =>
                {
                    var args = constructorArgsResolver.Invoke(provider, implementationType);
                    return Activator.CreateInstance(implementationType, args.ToArray());
                }));

            return services;
        }




        public static IServiceCollection AutoRegisterScoped<TServiceRegistrar>(this IServiceCollection services, params Type[] serviceTypes)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            var serviceRegistrar = new TServiceRegistrar();
            return AutoRegisterScoped(services, serviceRegistrar, serviceTypes, false);
        }

        public static IServiceCollection AutoRegisterScoped<TServiceRegistrar>(this IServiceCollection services, IEnumerable<Type> serviceTypes, bool includeMatchingInterface = false)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            var serviceRegistrar = new TServiceRegistrar();
            return AutoRegisterScoped(services, serviceRegistrar, serviceTypes, includeMatchingInterface);
        }

        public static IServiceCollection AutoRegisterScoped<TServiceRegistrar>(this IServiceCollection services, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, bool includeMatchingInterface = false)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            var serviceRegistrar = new TServiceRegistrar();
            return AutoRegisterSingleton(services, serviceRegistrar, serviceTypes, constructorArgsResolver, includeMatchingInterface);
        }

        public static IServiceCollection AutoRegisterScoped(this IServiceCollection services, IServiceRegistrar serviceRegistrar, params Type[] serviceTypes)
        {
            return AutoRegisterScoped(services, serviceRegistrar, serviceTypes, false);
        }

        public static IServiceCollection AutoRegisterScoped(this IServiceCollection services, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            bool includeMatchingInterface = false)
        {
            serviceRegistrar.AutoRegisterServices(
                serviceTypes,
                includeMatchingInterface,
                (serviceType, implementationType) => services.AddScoped(serviceType, implementationType));

            return services;
        }

        public static IServiceCollection AutoRegisterScoped(this IServiceCollection services, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, bool includeMatchingInterface = false)
        {
            serviceRegistrar.AutoRegisterServices(
                serviceTypes,
                includeMatchingInterface,
                (serviceType, implementationType) => services.AddScoped(serviceType, provider =>
                {
                    var args = constructorArgsResolver.Invoke(provider, implementationType);
                    return Activator.CreateInstance(implementationType, args.ToArray());
                }));

            return services;
        }




        public static IServiceCollection AutoRegisterTransient<TServiceRegistrar>(this IServiceCollection services, params Type[] serviceTypes)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            var serviceRegistrar = new TServiceRegistrar();
            return AutoRegisterTransient(services, serviceRegistrar, serviceTypes, false);
        }

        public static IServiceCollection AutoRegisterTransient<TServiceRegistrar>(this IServiceCollection services, IEnumerable<Type> serviceTypes, bool includeMatchingInterface = false)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            var serviceRegistrar = new TServiceRegistrar();
            return AutoRegisterTransient(services, serviceRegistrar, serviceTypes, includeMatchingInterface);
        }

        public static IServiceCollection AutoRegisterTransient<TServiceRegistrar>(this IServiceCollection services, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, bool includeMatchingInterface = false)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            var serviceRegistrar = new TServiceRegistrar();
            return AutoRegisterSingleton(services, serviceRegistrar, serviceTypes, constructorArgsResolver, includeMatchingInterface);
        }

        public static IServiceCollection AutoRegisterTransient(this IServiceCollection services, IServiceRegistrar serviceRegistrar, params Type[] serviceTypes)
        {
            return AutoRegisterTransient(services, serviceRegistrar, serviceTypes, false);
        }

        public static IServiceCollection AutoRegisterTransient(this IServiceCollection services, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            bool includeMatchingInterface = false)
        {
            serviceRegistrar.AutoRegisterServices(
                serviceTypes,
                includeMatchingInterface,
                (serviceType, implementationType) => services.AddTransient(serviceType, implementationType));

            return services;
        }

        public static IServiceCollection AutoRegisterTransient(this IServiceCollection services, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, bool includeMatchingInterface = false)
        {
            serviceRegistrar.AutoRegisterServices(
                serviceTypes,
                includeMatchingInterface,
                (serviceType, implementationType) => services.AddTransient(serviceType, provider =>
                {
                    var args = constructorArgsResolver.Invoke(provider, implementationType);
                    return Activator.CreateInstance(implementationType, args.ToArray());
                }));

            return services;
        }
    }
}
