using System;
using System.Collections.Generic;
using System.Linq;
using AllOverIt.Assertion;
using AllOverIt.DependencyInjection.Exceptions;
using AllOverIt.Extensions;

namespace AllOverIt.DependencyInjection
{
    public abstract class ServiceRegistrarBase : IServiceRegistrar, IServiceRegistrarOptions
    {
        private Func<Type, Type, bool> _registrationFilter;

        /// <inheritdoc />
        public void AutoRegisterServices(IEnumerable<Type> serviceTypes, Action<Type, Type> registrationAction, Action<IServiceRegistrarOptions> configure = default)
        {
            configure?.Invoke(this);

            var allServiceTypes = serviceTypes
                .WhenNotNullOrEmpty(nameof(serviceTypes))
                .AsReadOnlyCollection();

            ValidateServiceTypes(allServiceTypes);

            var implementationCandidates = GetType().Assembly
                .GetTypes()
                .Where(type => type.IsClass && !type.IsGenericType && !type.IsNested && !type.IsAbstract && !type.IsInterface);

            foreach (var implementationCandidate in implementationCandidates)
            {
                ProcessImplementationCandidate(implementationCandidate, allServiceTypes, registrationAction);
            }
        }

        void IServiceRegistrarOptions.Filter(Func<Type, Type, bool> filter)
        {
            _registrationFilter = filter.WhenNotNull(nameof(filter));
        }

        private static void ValidateServiceTypes(IEnumerable<Type> allServiceTypes)
        {
            var invalidServiceTypes = allServiceTypes
                .Where(serviceType => !serviceType.IsInterface && !serviceType.IsAbstract)
                .AsReadOnlyCollection();

            if (invalidServiceTypes.Any())
            {
                var invalidTypes = string.Join(", ", invalidServiceTypes.Select(serviceType => serviceType.GetFriendlyName()));
                throw new DependencyRegistrationException($"Cannot register {invalidTypes}. All service types must be an interface or abstract type.");
            }
        }

        private void ProcessImplementationCandidate(Type implementationCandidate, IEnumerable<Type> allServiceTypes, Action<Type, Type> registrationAction)
        {
            var candidateInterfaces = implementationCandidate
                .GetInterfaces()
                .AsReadOnlyCollection();

            var serviceTypes = allServiceTypes.Where(implementationCandidate.IsDerivedFrom);

            foreach (var serviceType in serviceTypes)
            {
                if (serviceType.IsInterface)
                {
                    var interfaces = candidateInterfaces.Where(@interface => @interface == serviceType || @interface.IsDerivedFrom(serviceType));

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
            var canRegister = _registrationFilter?.Invoke(serviceType, implementationCandidate) ?? true;

            if (canRegister)
            {
                registrationAction.Invoke(serviceType, implementationCandidate);
            }
        }
    }
}