using System;
using System.Linq;
using AllOverIt.DependencyInjection.Exceptions;
using AllOverIt.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.DependencyInjection.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection Decorate<TService, TDecorator>(this IServiceCollection services)
            where TDecorator : TService
        {
            var serviceType = typeof(TService);
            var descriptors = services.Where(service => service.ServiceType == serviceType).AsReadOnlyList();

            if (!descriptors.Any())
            {
                throw new DependencyRegistrationException($"No registered services found for the type '{serviceType.GetFriendlyName()}'");
            }

            foreach (var descriptor in descriptors)
            {
                var index = services.IndexOf(descriptor);
                services[index] = Decorate(descriptor, typeof(TDecorator));
            }

            return services;
        }

        private static ServiceDescriptor Decorate(ServiceDescriptor descriptor, Type decoratorType)
        {
            return ServiceDescriptor.Describe(
                descriptor.ServiceType,
                provider => ActivatorUtilities.CreateInstance(provider, decoratorType, GetInstance(provider, descriptor)),
                descriptor.Lifetime);
        }

        private static object GetInstance(IServiceProvider provider, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationInstance != null)
            {
                return descriptor.ImplementationInstance;
            }

            return descriptor.ImplementationType != null
                ? ActivatorUtilities.GetServiceOrCreateInstance(provider, descriptor.ImplementationType)
                : descriptor.ImplementationFactory!.Invoke(provider);
        }
    }
}