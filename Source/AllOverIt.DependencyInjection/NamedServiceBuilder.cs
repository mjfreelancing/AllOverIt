using AllOverIt.Assertion;
using Microsoft.Extensions.DependencyInjection;

namespace AllOverIt.DependencyInjection
{
    internal sealed class NamedServiceBuilder<TService> : INamedServiceBuilder<TService> where TService : class
    {
        internal NamedServiceResolver<TService> _resolver = new();

        private readonly IServiceCollection _services;

        public NamedServiceBuilder(IServiceCollection services)
        {
            _services = services.WhenNotNull();
        }

        public INamedServiceBuilder<TService> AsTransient<TImplementation>(string name) where TImplementation : class, TService
        {
            RegisterImplementation<TImplementation>(name);

            if (!IsRegistered<TImplementation>(ServiceLifetime.Transient))
            {
                _services.AddTransient<TService, TImplementation>();
            }

            return this;
        }

        public INamedServiceBuilder<TService> AsTransient(string name, Type implementationType)
        {
            RegisterImplementation(name, implementationType);

            if (!IsRegistered(implementationType, ServiceLifetime.Transient))
            {
                _services.AddTransient(typeof(TService), implementationType);
            }

            return this;
        }

        public INamedServiceBuilder<TService> AsScoped<TImplementation>(string name) where TImplementation : class, TService
        {
            RegisterImplementation<TImplementation>(name);

            if (!IsRegistered<TImplementation>(ServiceLifetime.Scoped))
            {
                _services.AddScoped<TService, TImplementation>();
            }

            return this;
        }

        public INamedServiceBuilder<TService> AsScoped(string name, Type implementationType)
        {
            RegisterImplementation(name, implementationType);

            if (!IsRegistered(implementationType, ServiceLifetime.Scoped))
            {
                _services.AddScoped(typeof(TService), implementationType);
            }

            return this;
        }

        public INamedServiceBuilder<TService> AsSingleton<TImplementation>(string name) where TImplementation : class, TService
        {
            RegisterImplementation<TImplementation>(name);

            if (!IsRegistered<TImplementation>(ServiceLifetime.Singleton))
            {
                _services.AddSingleton<TService, TImplementation>();
            }

            return this;
        }

        public INamedServiceBuilder<TService> AsSingleton(string name, Type implementationType)
        {
            RegisterImplementation(name, implementationType);

            if (!IsRegistered(implementationType, ServiceLifetime.Singleton))
            {
                _services.AddSingleton(typeof(TService), implementationType);
            }

            return this;
        }

        private void RegisterImplementation<TImplementation>(string name) where TImplementation : class, TService
        {
            _ = name.WhenNotNullOrEmpty();

            ((INamedServiceRegistration<TService>) _resolver).Register<TImplementation>(name);
        }

        private void RegisterImplementation(string name, Type implementationType)
        {
            _ = name.WhenNotNullOrEmpty();

            ((INamedServiceRegistration<TService>) _resolver).Register(name, implementationType);
        }

        private bool IsRegistered<TImplementation>(ServiceLifetime lifetime)
        {
            return IsRegistered(typeof(TImplementation), lifetime);
        }

        private bool IsRegistered(Type implementationType, ServiceLifetime lifetime)
        {
            // Can't use TryAddXXX() from Microsoft.Extensions.DependencyInjection.Extensions as this only
            // checks the service type. This implementation allows multiple names to register the same
            // concrete for the same service type.
            var serviceType = typeof(TService);

            foreach (var descriptor in _services)
            {
                if (descriptor.Lifetime == lifetime &&
                    descriptor.ServiceType == serviceType &&
                    descriptor.ImplementationType == implementationType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}