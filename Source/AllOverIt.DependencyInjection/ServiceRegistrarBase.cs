using System;
using System.Collections.Generic;
using System.Linq;
using AllOverIt.Assertion;
using AllOverIt.Extensions;

namespace AllOverIt.DependencyInjection
{
    public abstract class ServiceRegistrarBase : IServiceRegistrar
    {
        /// <inheritdoc />
        public void AutoRegisterServices(IEnumerable<Type> serviceTypes, bool includeMatchingInterface, Action<Type, Type> registrationAction)
        {
            var allServiceTypes = serviceTypes
                .WhenNotNullOrEmpty(nameof(serviceTypes))
                .AsReadOnlyCollection();

            var invalidServiceTypes = allServiceTypes
                .Where(serviceType => !serviceType.IsInterface && !serviceType.IsAbstract)
                .AsReadOnlyCollection();

            if (invalidServiceTypes.Any())
            {
                // TODO: Create a custom exception
                var invalidTypes = string.Join(", ", invalidServiceTypes.Select(serviceType => serviceType.GetFriendlyName()));
                throw new InvalidOperationException($"Cannot register {invalidTypes}. All service types must be an interface or abstract type.");
            }

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
            // Intentions:
            //
            // If a serviceType is an abstract class then register it against any concrete implementations against that type;
            // When 'includeMatchingInterface' == true, if a serviceType is an interface then register all concrete implementations against that interface;
            // When 'includeMatchingInterface' == false, if a serviceType is an interface then register all concrete implementations against all inheriting interfaces;

            var candidateInterfaces = implementationCandidate
                .GetInterfaces()
                .AsReadOnlyCollection();

            var serviceTypes = allServiceTypes.Where(implementationCandidate.IsDerivedFrom);

            foreach (var serviceType in serviceTypes)
            {
                if (serviceType.IsInterface)
                {
                    var interfaces = candidateInterfaces
                        .Where(@interface => @interface == serviceType && includeMatchingInterface ||
                                             @interface.IsDerivedFrom(serviceType));

                    foreach (var @interface in interfaces)
                    {
                        TryRegisterType(@interface, implementationCandidate, registrationAction);
                    }
                }
                else if (serviceType.IsAbstract)
                {
                    TryRegisterType(serviceType, implementationCandidate, registrationAction);
                }
            }
        }

        private void TryRegisterType(Type serviceType, Type implementationCandidate, Action<Type, Type> registrationAction)
        {
            if (IncludeRegistration(serviceType, implementationCandidate))
            {
                registrationAction.Invoke(serviceType, implementationCandidate);
            }
        }
    }
}