using AllOverIt.Assertion;
using AllOverIt.DependencyInjection.Exceptions;
using AllOverIt.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.DependencyInjection
{
    internal sealed class NamedServiceResolver<TService> : INamedServiceRegistration<TService>,
                                                           INamedServiceResolver<TService> where TService : class
    {
        private readonly Dictionary<string, Type> _namedImplementations = [];
        internal IServiceProvider _provider;        // assigned through field injection

        void INamedServiceRegistration<TService>.Register<TImplementation>(string name)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));

            ((INamedServiceRegistration<TService>) this).Register(name, typeof(TImplementation));
        }

        void INamedServiceRegistration<TService>.Register(string name, Type implementationType)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));

            if (_namedImplementations.TryGetValue(name, out var namedType))
            {
                throw new DependencyRegistrationException($"The name '{name}' has already been registered against the type '{namedType.GetFriendlyName()}'.");
            }

            if (!implementationType.IsDerivedFrom(typeof(TService)))
            {
                throw new DependencyRegistrationException($"The type '{implementationType.GetFriendlyName()}' with name '{name}' does not implement '{typeof(TService).GetFriendlyName()}'.");
            }

            _namedImplementations.Add(name, implementationType);
        }

        TService INamedServiceResolver<TService>.GetRequiredNamedService(string name)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));

            if (_namedImplementations.TryGetValue(name, out var implementationType))
            {
                return _provider
                    .GetRequiredService<IEnumerable<TService>>()
                    .Single(service => service.GetType() == implementationType);
            }

            throw new DependencyRegistrationException($"No service of type {typeof(TService).GetFriendlyName()} was found for the name {name}.");
        }
    }

    internal sealed class NamedServiceResolver : INamedServiceResolver
    {
        private readonly IServiceProvider _provider;

        public NamedServiceResolver(IServiceProvider provider)
        {
            _provider = provider.WhenNotNull(nameof(provider));
        }

        TService INamedServiceResolver.GetRequiredNamedService<TService>(string name)
        {
            _ = name.WhenNotNullOrEmpty(nameof(name));

            var resolver = _provider.GetRequiredService<INamedServiceResolver<TService>>();

            return resolver.GetRequiredNamedService(name);
        }
    }
}