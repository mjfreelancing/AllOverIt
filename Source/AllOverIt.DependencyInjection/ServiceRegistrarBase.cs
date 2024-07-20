using AllOverIt.Assertion;
using AllOverIt.DependencyInjection.Exceptions;
using AllOverIt.Extensions;

namespace AllOverIt.DependencyInjection
{
    /// <summary>Base class for service registrars that scan for, and register, service and implementation types located in the containing assembly.
    /// If a class implements multiple interfaces, or includes an abstract class, and those service types are being searched, then each service
    /// type will have its own registration against the implementation type with the service collection.
    /// <br/><br/>
    /// As an example, if ConcreteA implements IService1 and IService2, then resolving IService1 and IService2 will result in different ConcreteA
    /// instances being resolved, even if they are registered as Scoped or a Singleton.
    /// </summary>
    public abstract class ServiceRegistrarBase : IServiceRegistrar, IServiceRegistrarOptions
    {
        private readonly Lazy<IReadOnlyCollection<Type>> _implementationCandidates;
        private Func<Type, Type, bool>? _registrationFilter;

        private IReadOnlyCollection<Type> ImplementationCandidates => _implementationCandidates.Value;

        /// <summary>Constructor.</summary>
        public ServiceRegistrarBase()
        {
            _implementationCandidates = new Lazy<IReadOnlyCollection<Type>>(() =>
            {
                return GetType().Assembly
                    .GetTypes()
                    .Where(type => type.IsClass && !type.IsGenericType && !type.IsNested && !type.IsAbstract)
                    .AsReadOnlyCollection();
            });
        }

        /// <inheritdoc />
        public void AutoRegisterServices(IEnumerable<Type> serviceTypes, Action<Type, Type> registrationAction, Action<IServiceRegistrarOptions>? configure = default)
        {
            configure?.Invoke(this);

            var allServiceTypes = serviceTypes
                .WhenNotNullOrEmpty()
                .AsReadOnlyCollection();

            ValidateServiceTypes(allServiceTypes);

            foreach (var implementationCandidate in ImplementationCandidates)
            {
                ProcessImplementationCandidate(implementationCandidate, allServiceTypes, registrationAction);
            }
        }

        void IServiceRegistrarOptions.Filter(Func<Type, Type, bool> filter)
        {
            _registrationFilter = filter.WhenNotNull();
        }

        private static void ValidateServiceTypes(IEnumerable<Type> allServiceTypes)
        {
            var invalidServiceTypes = allServiceTypes
                .Where(serviceType => !serviceType.IsInterface && !serviceType.IsAbstract)
                .AsReadOnlyCollection();

            if (invalidServiceTypes.Count != 0)
            {
                var invalidTypes = string.Join(", ", invalidServiceTypes.Select(serviceType => serviceType.GetFriendlyName()));
                throw new DependencyRegistrationException($"Cannot register {invalidTypes}. All service types must be an interface or abstract type.");
            }
        }

        private void ProcessImplementationCandidate(Type implementationCandidate, IEnumerable<Type> serviceTypes, Action<Type, Type> registrationAction)
        {
            var candidateInterfaces = implementationCandidate
                .GetInterfaces()
                .AsReadOnlyCollection();

            var candidateServiceTypes = serviceTypes.Where(implementationCandidate.IsDerivedFrom);

            foreach (var serviceType in candidateServiceTypes)
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