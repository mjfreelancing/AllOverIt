using System;
using System.Collections.Generic;
using System.Linq;
using AllOverIt.DependencyInjection.Extensions;
using AllOverIt.Extensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.DependencyInjection.Tests.Helpers
{
    internal static class DependencyHelper
    {
        internal static IServiceCollection AutoRegisterUsingServiceLifetime<TServiceRegistrar, TServiceType>(ServiceLifetime lifetime, IServiceCollection services,
            Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return lifetime switch
            {
                ServiceLifetime.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton<TServiceRegistrar, TServiceType>(services, configure),
                
                ServiceLifetime.Scoped => ServiceCollectionExtensions.AutoRegisterScoped<TServiceRegistrar, TServiceType>(services, configure),

                ServiceLifetime.Transient => ServiceCollectionExtensions.AutoRegisterTransient<TServiceRegistrar, TServiceType>(services, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };
        }

        internal static IServiceCollection AutoRegisterUsingServiceLifetime<TServiceRegistrar, TServiceType>(this IServiceCollection services, ServiceLifetime mode,
            Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return AutoRegisterUsingServiceLifetime<TServiceRegistrar, TServiceType>(mode, services, configure);
        }

        internal static IServiceCollection AutoRegisterUsingServiceLifetime<TServiceRegistrar>(ServiceLifetime lifetime, IServiceCollection services,
            IEnumerable<Type> serviceTypes, Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return lifetime switch
            {
                ServiceLifetime.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton<TServiceRegistrar>(services, serviceTypes, configure),

                ServiceLifetime.Scoped => ServiceCollectionExtensions.AutoRegisterScoped<TServiceRegistrar>(services, serviceTypes, configure),

                ServiceLifetime.Transient => ServiceCollectionExtensions.AutoRegisterTransient<TServiceRegistrar>(services, serviceTypes, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };
        }

        internal static IServiceCollection AutoRegisterUsingServiceLifetime<TServiceRegistrar>(this IServiceCollection services, ServiceLifetime mode,
            IEnumerable<Type> serviceTypes, Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return AutoRegisterUsingServiceLifetime<TServiceRegistrar>(mode, services, serviceTypes, configure);
        }

        internal static IServiceCollection AutoRegisterUsingServiceLifetime<TServiceType>(ServiceLifetime lifetime, IServiceCollection services,
            IServiceRegistrar serviceRegistrar, Action<IServiceRegistrarOptions> configure = default)
        {
            return lifetime switch
            {
                ServiceLifetime.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton<TServiceType>(services, serviceRegistrar, configure),

                ServiceLifetime.Scoped => ServiceCollectionExtensions.AutoRegisterScoped<TServiceType>(services, serviceRegistrar, configure),

                ServiceLifetime.Transient => ServiceCollectionExtensions.AutoRegisterTransient<TServiceType>(services, serviceRegistrar, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };
        }

        internal static IServiceCollection AutoRegisterUsingServiceLifetime<TServiceType>(this IServiceCollection services, ServiceLifetime mode,
            IServiceRegistrar serviceRegistrar, Action<IServiceRegistrarOptions> configure = default)
        {
            return AutoRegisterUsingServiceLifetime<TServiceType>(mode, services, serviceRegistrar, configure);
        }

        internal static IServiceCollection AutoRegisterUsingServiceLifetime(ServiceLifetime lifetime, IServiceCollection services, IServiceRegistrar serviceRegistrar,
            IEnumerable<Type> serviceTypes, Action<IServiceRegistrarOptions> configure = default)
        {
            return lifetime switch
            {
                ServiceLifetime.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton(services, serviceRegistrar, serviceTypes, configure),

                ServiceLifetime.Scoped => ServiceCollectionExtensions.AutoRegisterScoped(services, serviceRegistrar, serviceTypes, configure),

                ServiceLifetime.Transient => ServiceCollectionExtensions.AutoRegisterTransient(services, serviceRegistrar, serviceTypes, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };
        }

        internal static IServiceCollection AutoRegisterUsingServiceLifetime(this IServiceCollection services, ServiceLifetime mode, IServiceRegistrar serviceRegistrar,
            IEnumerable<Type> serviceTypes, Action<IServiceRegistrarOptions> configure = default)
        {
            return AutoRegisterUsingServiceLifetime(mode, services, serviceRegistrar, serviceTypes, configure);
        }

        internal static IServiceCollection AutoRegisterUsingServiceLifetime(ServiceLifetime lifetime, IServiceCollection services, IEnumerable<IServiceRegistrar> serviceRegistrars,
            IEnumerable<Type> serviceTypes, Action<IServiceRegistrarOptions> configure = default)
        {
            return lifetime switch
            {
                ServiceLifetime.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton(services, serviceRegistrars, serviceTypes, configure),

                ServiceLifetime.Scoped => ServiceCollectionExtensions.AutoRegisterScoped(services, serviceRegistrars, serviceTypes, configure),

                ServiceLifetime.Transient => ServiceCollectionExtensions.AutoRegisterTransient(services, serviceRegistrars, serviceTypes, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };
        }

        internal static IServiceCollection AutoRegisterUsingServiceLifetime(this IServiceCollection services, ServiceLifetime mode, IEnumerable<IServiceRegistrar> serviceRegistrars,
            IEnumerable<Type> serviceTypes, Action<IServiceRegistrarOptions> configure = default)
        {
            return AutoRegisterUsingServiceLifetime(mode, services, serviceRegistrars, serviceTypes, configure);
        }

        internal static IServiceCollection AutoRegisterUsingServiceLifetime<TServiceRegistrar>(ServiceLifetime lifetime, IServiceCollection services, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return lifetime switch
            {
                ServiceLifetime.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton<TServiceRegistrar>(services, serviceTypes, constructorArgsResolver, configure),

                ServiceLifetime.Scoped => ServiceCollectionExtensions.AutoRegisterScoped<TServiceRegistrar>(services, serviceTypes, constructorArgsResolver, configure),

                ServiceLifetime.Transient => ServiceCollectionExtensions.AutoRegisterTransient<TServiceRegistrar>(services, serviceTypes, constructorArgsResolver, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };
        }

        internal static IServiceCollection AutoRegisterUsingServiceLifetime<TServiceRegistrar>(this IServiceCollection services, ServiceLifetime mode, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
            where TServiceRegistrar : IServiceRegistrar, new()
        {
            return AutoRegisterUsingServiceLifetime<TServiceRegistrar>(mode, services, serviceTypes, constructorArgsResolver, configure);
        }

        internal static IServiceCollection AutoRegisterUsingServiceLifetime(ServiceLifetime lifetime, IServiceCollection services, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
        {
            return lifetime switch
            {
                ServiceLifetime.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton(services, serviceRegistrar, serviceTypes, constructorArgsResolver, configure),

                ServiceLifetime.Scoped => ServiceCollectionExtensions.AutoRegisterScoped(services, serviceRegistrar, serviceTypes, constructorArgsResolver, configure),

                ServiceLifetime.Transient => ServiceCollectionExtensions.AutoRegisterTransient(services, serviceRegistrar, serviceTypes, constructorArgsResolver, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };
        }

        internal static IServiceCollection AutoRegisterUsingServiceLifetime(this IServiceCollection services, ServiceLifetime mode, IServiceRegistrar serviceRegistrar, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
        {
            return AutoRegisterUsingServiceLifetime(mode, services, serviceRegistrar, serviceTypes, constructorArgsResolver, configure);
        }

        internal static IServiceCollection AutoRegisterUsingServiceLifetime(ServiceLifetime lifetime, IServiceCollection services, IEnumerable<IServiceRegistrar> serviceRegistrars, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
        {
            return lifetime switch
            {
                ServiceLifetime.Singleton => ServiceCollectionExtensions.AutoRegisterSingleton(services, serviceRegistrars, serviceTypes, constructorArgsResolver, configure),

                ServiceLifetime.Scoped => ServiceCollectionExtensions.AutoRegisterScoped(services, serviceRegistrars, serviceTypes, constructorArgsResolver, configure),

                ServiceLifetime.Transient => ServiceCollectionExtensions.AutoRegisterTransient(services, serviceRegistrars, serviceTypes, constructorArgsResolver, configure),

                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null)
            };
        }

        internal static IServiceCollection AutoRegisterUsingServiceLifetime(this IServiceCollection services, ServiceLifetime mode, IEnumerable<IServiceRegistrar> serviceRegistrars, IEnumerable<Type> serviceTypes,
            Func<IServiceProvider, Type, IEnumerable<object>> constructorArgsResolver, Action<IServiceRegistrarOptions> configure = default)
        {
            return AutoRegisterUsingServiceLifetime(mode, services, serviceRegistrars, serviceTypes, constructorArgsResolver, configure);
        }

        internal static void AssertInstanceEquality<TType>(IEnumerable<TType> items1, IEnumerable<TType> items2, bool expected)
        {
            var instances1 = items1.OrderBy(item => item.GetType().AssemblyQualifiedName);
            var instances2 = items2.OrderBy(item => item.GetType().AssemblyQualifiedName);

            instances1
                .Zip(instances2)
                .ForEach((instance, _) =>
                {
                    instance.First
                        .Should()
                        .BeOfType(instance.Second.GetType());

                    ReferenceEquals(instance.First, instance.Second).Should().Be(expected);
                });
        }

        internal static void AssertExpectation<TServiceType>(IServiceProvider provider, IEnumerable<Type> expectedTypes)
        {
            var actual = provider.GetService<IEnumerable<TServiceType>>()!.SelectAsReadOnlyCollection(item => item.GetType());

            expectedTypes.Should().BeEquivalentTo(actual);
        }
    }
}