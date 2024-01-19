using System;

namespace AllOverIt.DependencyInjection
{
    /// <summary>Represents the ability to register a service type by name.</summary>
    /// <typeparam name="TService">The service type to be registered by name.</typeparam>
    public interface INamedServiceRegistration<TService>
    {
        /// <summary>Registers a named implementation of <typeparamref name="TService"/>
        /// as a <typeparamref name="TImplementation"/>.</summary>
        /// <typeparam name="TImplementation">The implementation type to be registered.</typeparam>
        /// <param name="name">The name used to identify the implementation to be resolved.</param>
        /// <remarks>Multiple names can be registered against the same service and implementation types if required.</remarks>
        void Register<TImplementation>(string name) where TImplementation : TService;

        /// <summary>Registers a named implementation of <typeparamref name="TService"/>
        /// based on its <see cref="Type"/>. This type must implement <typeparamref name="TService"/>.</summary>
        /// <param name="name">The name used to identify the implementation to be resolved.</param>
        /// <param name="implementationType">The type implementing <typeparamref name="TService"/> that will
        /// later be resolved based on the provided <paramref name="name"/>.</param>
        /// <remarks>Multiple names can be registered against the same service and implementation types if required.</remarks>
        void Register(string name, Type implementationType);
    }
}