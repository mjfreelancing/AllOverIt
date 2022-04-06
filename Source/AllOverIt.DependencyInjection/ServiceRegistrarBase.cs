using System;
using System.Collections.Generic;
using System.Linq;
using AllOverIt.Assertion;
using AllOverIt.Extensions;

namespace AllOverIt.DependencyInjection
{
    public abstract class ServiceRegistrarBase : IServiceRegistrar
    {
        public void AutoRegisterServices(IEnumerable<Type> serviceTypes, bool includeMatchingInterface, Action<Type, Type> registrationAction)
        {
            var allServiceTypes = serviceTypes
                .WhenNotNullOrEmpty(nameof(serviceTypes))
                .AsReadOnlyCollection();

            var implementationCandidates = GetType().Assembly
                .GetTypes()
                .Where(type => type.IsClass && !type.IsGenericType && !type.IsNested && !type.IsAbstract && !type.IsInterface);

            foreach (var implementationCandidate in implementationCandidates)
            {
                ProcessImplementationCandidate(implementationCandidate, allServiceTypes, includeMatchingInterface, registrationAction);
            }
        }

        protected virtual bool IncludeRegistration(Type serviceType, Type implementationType)
        {
            return true;
        }

        private void ProcessImplementationCandidate(Type implementationCandidate, IEnumerable<Type> allServiceTypes, bool includeMatchingInterface,
            Action<Type, Type> registrationAction)
        {
            var candidateInterfaces = implementationCandidate
                .GetInterfaces()
                .AsReadOnlyCollection();

            var serviceTypes = allServiceTypes.Where(implementationCandidate.IsDerivedFrom);

            foreach (var serviceType in serviceTypes)
            {
                var interfaces = candidateInterfaces
                    .Where(@interface => @interface == serviceType && includeMatchingInterface ||
                                         @interface.IsDerivedFrom(serviceType))
                    .Where(@interface => IncludeRegistration(@interface, implementationCandidate));

                foreach (var @interface in interfaces)
                {
                    registrationAction.Invoke(@interface, implementationCandidate);
                }
            }
        }
    }
}