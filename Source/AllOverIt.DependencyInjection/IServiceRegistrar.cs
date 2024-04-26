namespace AllOverIt.DependencyInjection
{
    /// <summary>Represents a registrar responsible for registering services against suitable concrete implementations.</summary>
    public interface IServiceRegistrar
    {
        /// <summary>Registers one or more service types against implementation types found in the registrar's assembly. The registrar will register any implementation
        /// type that inherits any service type that is an abstract class. The registrar will register any implementation type that inherits any service type that is an
        /// interface, along with other implementing interfaces that inherit the service interface type.</summary>
        /// <param name="serviceTypes">The service types (abstract classes or interfaces) to register.</param>
        /// <param name="registrationAction">The action to invoke to register a service type against an implementation type.</param>
        /// <param name="configure">Optional configuration options that provide the ability to exclude or otherwise filter service or implementation types.</param>
        void AutoRegisterServices(IEnumerable<Type> serviceTypes, Action<Type, Type> registrationAction, Action<IServiceRegistrarOptions> configure = default);
    }
}