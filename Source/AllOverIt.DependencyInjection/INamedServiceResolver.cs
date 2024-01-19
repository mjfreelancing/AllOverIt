namespace AllOverIt.DependencyInjection
{
    /// <summary>Represents a resolver that resolves named services from a service collection.</summary>
    /// <typeparam name="TService">The service type to be resolved by name.</typeparam>
    public interface INamedServiceResolver<TService>
    {
        /// <summary>Resolves a service instance by name. The service to be resolved must implement <typeparamref name="TService"/>.</summary>
        /// <param name="name">The name of the service type to be resolved.</param>
        /// <returns>The resolved service instance.</returns>
        TService GetRequiredNamedService(string name);
    }

    /// <summary>Represents a resolver that can resolve and service type by name.</summary>
    public interface INamedServiceResolver
    {
        /// <summary>Resolves a service instance by name. The service to be resolved must implement <typeparamref name="TService"/>.</summary>
        /// <typeparam name="TService">The service type to be resolved by name.</typeparam>
        /// <param name="name">The name of the service type to be resolved.</param>
        /// <returns>The resolved service instance.</returns>
        TService GetRequiredNamedService<TService>(string name);
    }
}