using System;
using System.Collections.Generic;
using System.Linq;
using AllOverIt.Assertion;
using AllOverIt.Extensions;

namespace AllOverIt.DependencyInjection
{
    public abstract class ServiceRegistrarBase : IServiceRegistrar
    {
        public void AutoRegisterTypes(IEnumerable<Type> serviceTypes, bool includeMatchingInterface, Action<Type, Type> registrationAction)
        {
            var allServiceTypes = serviceTypes
                .WhenNotNullOrEmpty(nameof(serviceTypes))
                .AsReadOnlyCollection();

            var assemblyTypes = GetType().Assembly
                .GetTypes()
                .Where(type => type.IsClass && !type.IsGenericType && !type.IsNested && !type.IsAbstract && !type.IsInterface)
                .AsReadOnlyCollection();

            foreach (var assemblyType in assemblyTypes)
            {
                RegisterAssemblyServiceTypes(assemblyType, allServiceTypes, includeMatchingInterface, registrationAction);
            }
        }

        protected virtual bool IncludeRegistration(Type serviceType, Type implementationType)
        {
            return true;
        }

        private void RegisterAssemblyServiceTypes(Type assemblyType, IEnumerable<Type> allServiceTypes, bool includeMatchingInterface,
            Action<Type, Type> registrationAction)
        {
            var assemblyTypeInterfaces = assemblyType
                .GetInterfaces()
                .AsReadOnlyCollection();

            var serviceTypes = allServiceTypes.Where(assemblyType.IsDerivedFrom);

            foreach (var serviceType in serviceTypes)
            {
                var interfaces = assemblyTypeInterfaces
                    .Where(@interface => @interface == serviceType && includeMatchingInterface ||
                                         @interface.IsDerivedFrom(serviceType))
                    .Where(@interface => IncludeRegistration(@interface, assemblyType));

                foreach (var @interface in interfaces)
                {
                    registrationAction.Invoke(@interface, assemblyType);
                }
            }
        }
    }
}