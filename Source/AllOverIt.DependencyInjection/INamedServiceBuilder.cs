using System;

namespace AllOverIt.DependencyInjection
{
    /// <summary>Represents a builder that allows for the registration of named services, where each
    /// implementation must implement <typeparamref name="TService"/>.</summary>
    /// <typeparam name="TService">The service type to be registered.</typeparam>
    public interface INamedServiceBuilder<TService> where TService : class
    {
        /// <summary>Registers a transient implementation of <typeparamref name="TService"/> as a <typeparamref name="TImplementation"/>
        /// using the provided <paramref name="name"/>.</summary>
        /// <typeparam name="TImplementation">The typw implementing <typeparamref name="TService"/>.</typeparam>
        /// <param name="name">The name identifying the implementation type to be resolved.</param>
        /// <returns>The builder instance so multiple registrations can be chained.</returns>
        /// <remarks>Multiple names can be registered against the same service and implementation types if required.</remarks>
        INamedServiceBuilder<TService> AsTransient<TImplementation>(string name) where TImplementation : class, TService;

        /// <summary>Registers a transient implementation of <typeparamref name="TService"/> as a <see cref="Type"/>
        /// using the provided <paramref name="name"/>. The <paramref name="implementationType"/> must implement
        /// <typeparamref name="TService"/>.</summary>
        /// <param name="name">The name identifying the implementation type to be resolved.</param>
        /// <param name="implementationType">The type implementing <typeparamref name="TService"/> being registered.</param>
        /// <returns>The builder instance so multiple registrations can be chained.</returns>
        /// <remarks>Multiple names can be registered against the same service and implementation types if required.</remarks>
        INamedServiceBuilder<TService> AsTransient(string name, Type implementationType);

        /// <summary>Registers a scoped implementation of <typeparamref name="TService"/> as a <typeparamref name="TImplementation"/>
        /// using the provided <paramref name="name"/>.</summary>
        /// <typeparam name="TImplementation">The typw implementing <typeparamref name="TService"/>.</typeparam>
        /// <param name="name">The name identifying the implementation type to be resolved.</param>
        /// <returns>The builder instance so multiple registrations can be chained.</returns>
        /// <remarks>Multiple names can be registered against the same service and implementation types if required.</remarks>
        INamedServiceBuilder<TService> AsScoped<TImplementation>(string name) where TImplementation : class, TService;

        /// <summary>Registers a scoped implementation of <typeparamref name="TService"/> as a <see cref="Type"/>
        /// using the provided <paramref name="name"/>. The <paramref name="implementationType"/> must implement
        /// <typeparamref name="TService"/>.</summary>
        /// <param name="name">The name identifying the implementation type to be resolved.</param>
        /// <param name="implementationType">The type implementing <typeparamref name="TService"/> being registered.</param>
        /// <returns>The builder instance so multiple registrations can be chained.</returns>
        /// <remarks>Multiple names can be registered against the same service and implementation types if required.</remarks>
        INamedServiceBuilder<TService> AsScoped(string name, Type implementationType);

        /// <summary>Registers a singleton implementation of <typeparamref name="TService"/> as a <typeparamref name="TImplementation"/>
        /// using the provided <paramref name="name"/>.</summary>
        /// <typeparam name="TImplementation">The typw implementing <typeparamref name="TService"/>.</typeparam>
        /// <param name="name">The name identifying the implementation type to be resolved.</param>
        /// <returns>The builder instance so multiple registrations can be chained.</returns>
        /// <remarks>Multiple names can be registered against the same service and implementation types if required.</remarks>
        INamedServiceBuilder<TService> AsSingleton<TImplementation>(string name) where TImplementation : class, TService;

        /// <summary>Registers a singleton implementation of <typeparamref name="TService"/> as a <see cref="Type"/>
        /// using the provided <paramref name="name"/>. The <paramref name="implementationType"/> must implement
        /// <typeparamref name="TService"/>.</summary>
        /// <param name="name">The name identifying the implementation type to be resolved.</param>
        /// <param name="implementationType">The type implementing <typeparamref name="TService"/> being registered.</param>
        /// <returns>The builder instance so multiple registrations can be chained.</returns>
        /// <remarks>Multiple names can be registered against the same service and implementation types if required.</remarks>
        INamedServiceBuilder<TService> AsSingleton(string name, Type implementationType);
    }
}