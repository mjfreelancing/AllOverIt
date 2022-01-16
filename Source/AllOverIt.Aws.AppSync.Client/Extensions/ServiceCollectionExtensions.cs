﻿using AllOverIt.Aws.AppSync.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AllOverIt.Aws.AppSync.Client.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IServiceCollection"/>.</summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>Registers an <see cref="IAppSyncClient"/> using the configuration provided by the configuration resolver.</summary>
        /// <param name="services">The <see cref="IServiceCollection"/> being extended.</param>
        /// <param name="configurationResolver">Provides the configuration required by the <see cref="IAppSyncClient"/> instance.</param>
        /// <returns>An <see cref="AppSyncClient"/> instance, configured with the resolved configuration.</returns>
        public static IServiceCollection AddAppSyncClient(this IServiceCollection services, Func<IServiceProvider, IAppSyncClientConfiguration> configurationResolver)
        {
            services.AddSingleton<IAppSyncClient>(provider =>
            {
                var configuration = configurationResolver.Invoke(provider);
                return new AppSyncClient(configuration);
            });

            return services;
        }

        /// <summary>Registers an <see cref="IAppSyncClient"/> using the provided configuration.</summary>
        /// <param name="services">The <see cref="IServiceCollection"/> being extended.</param>
        /// <param name="configuration">The configuration required by the <see cref="IAppSyncClient"/> instance.</param>
        /// <returns>An <see cref="AppSyncClient"/> instance, configured with the provided configuration.</returns>
        public static IServiceCollection AddAppSyncClient(this IServiceCollection services, IAppSyncClientConfiguration configuration)
        {
            return services.AddAppSyncClient(_ => configuration);
        }

        /// <summary>Registers an <see cref="IAppSyncSubscriptionClient"/> using the configuration provided by the configuration resolver.</summary>
        /// <param name="services">The <see cref="IServiceCollection"/> being extended.</param>
        /// <param name="configurationResolver">Provides the configuration required by the <see cref="IAppSyncSubscriptionClient"/> instance.</param>
        /// <returns>An <see cref="AppSyncSubscriptionClient"/> instance, configured with the resolved configuration.</returns>
        public static IServiceCollection AddAppSyncSubscriptionClient(this IServiceCollection services, Func<IServiceProvider, ISubscriptionClientConfiguration> configurationResolver)
        {
            services.AddSingleton<IAppSyncSubscriptionClient>(provider =>
            {
                var configuration = configurationResolver.Invoke(provider);
                return new AppSyncSubscriptionClient(configuration);
            });

            return services;
        }

        /// <summary>Registers an <see cref="IAppSyncSubscriptionClient"/> using the provided configuration.</summary>
        /// <param name="services">The <see cref="IServiceCollection"/> being extended.</param>
        /// <param name="configuration">The configuration required by the <see cref="IAppSyncSubscriptionClient"/> instance.</param>
        /// <returns>An <see cref="AppSyncSubscriptionClient"/> instance, configured with the resolved configuration.</returns>
        public static IServiceCollection AddAppSyncSubscriptionClient(this IServiceCollection services, ISubscriptionClientConfiguration configuration)
        {
            return services.AddAppSyncSubscriptionClient(_ => configuration);
        }

        /// <summary>Registers an <see cref="INamedAppSyncClientProvider"/> for the purpose of requesting <see cref="IAppSyncClient"/> instances by name.</summary>
        /// <param name="services">The <see cref="IServiceCollection"/> being extended.</param>
        /// <param name="configResolver">Provides the configuration required by the named <see cref="IAppSyncClient"/> instance.</param>
        /// <returns>An <see cref="AppSyncClient"/> instance, per name, configured with the provided configuration.</returns>
        public static IServiceCollection AddNamedAppSyncClient(this IServiceCollection services, Func<IServiceProvider, string, IAppSyncClientConfiguration> configResolver)
        {
            // Although a singleton, the registered delegate will be called EACH time a named client is requested via NamedAppSyncClientDelegate.
            // The INamedAppSyncClientProvider ensures a client (and it's configuration) is only ever requested once per name.
            services.AddSingleton<NamedAppSyncClientConfigurationDelegate>(provider => clientName => configResolver.Invoke(provider, clientName));

            // Although a singleton, the registered delegate will be called EACH time a named client is requested.
            // The INamedAppSyncClientProvider ensures a client (and it's configuration) is only ever requested once per name.
            services.AddSingleton<NamedAppSyncClientDelegate>(provider => name =>
            {
                var configuration = provider.GetService<NamedAppSyncClientConfigurationDelegate>();
                var namedConfig = configuration(name);

                return new AppSyncClient(namedConfig);
            });

            // This provider will only ever call the NamedAppSyncClientDelegate once per name
            services.AddSingleton<INamedAppSyncClientProvider, NamedAppSyncClientProvider>();

            return services;
        }
    }
}
