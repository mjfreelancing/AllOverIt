using AllOverIt.Aws.AppSync.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AllOverIt.Aws.AppSync.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAppSyncClient(this IServiceCollection services, Func<IServiceProvider, IAppSyncClientConfiguration> configurationResolver)
        {
            services.AddSingleton<IAppSyncClient>(provider =>
            {
                var configuration = configurationResolver.Invoke(provider);
                return new AppSyncClient(configuration);
            });
        }

        public static void AddAppSyncClient(this IServiceCollection services, IAppSyncClientConfiguration configuration)
        {
            services.AddAppSyncClient(_ => configuration);
        }

        public static void AddAppSyncSubscriptionClient(this IServiceCollection services, Func<IServiceProvider, ISubscriptionClientConfiguration> configurationResolver)
        {
            services.AddSingleton<IAppSyncSubscriptionClient>(provider =>
            {
                var configuration = configurationResolver.Invoke(provider);
                return new AppSyncSubscriptionClient(configuration);
            });
        }

        public static void AddAppSyncSubscriptionClient(this IServiceCollection services, ISubscriptionClientConfiguration configuration)
        {
            services.AddAppSyncSubscriptionClient(_ => configuration);
        }
    }
}
