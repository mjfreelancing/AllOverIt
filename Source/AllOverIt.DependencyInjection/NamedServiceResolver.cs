using AllOverIt.Assertion;
using AllOverIt.DependencyInjection.Exceptions;
using AllOverIt.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.DependencyInjection
{
    internal sealed class NamedServiceResolver<TService> : INamedServiceRegistration<TService>,
                                                           INamedServiceResolver<TService> where TService : class
    {
        private readonly Dictionary<string, Type> _namedImplementations = [];
        internal IServiceProvider? _provider;        // assigned through field injection

        [SuppressMessage("Usage", "CA2263:Prefer generic overload when type is known", Justification = "Would be a recursive call, resulting in a stack overflow")]
        void INamedServiceRegistration<TService>.Register<TImplementation>(string name)
        {
            _ = name.WhenNotNullOrEmpty();

            (this as INamedServiceRegistration<TService>).Register(name, typeof(TImplementation));
        }

        void INamedServiceRegistration<TService>.Register(string name, Type implementationType)
        {
            _ = name.WhenNotNullOrEmpty();

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
            _ = name.WhenNotNullOrEmpty();

            if (_namedImplementations.TryGetValue(name, out var implementationType))
            {
                Throw<InvalidOperationException>.WhenNull(_provider, "The service provider has not been assigned.");

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
            _provider = provider.WhenNotNull();
        }

        TService INamedServiceResolver.GetRequiredNamedService<TService>(string name)
        {
            _ = name.WhenNotNullOrEmpty();

            var resolver = _provider.GetRequiredService<INamedServiceResolver<TService>>();

            return resolver.GetRequiredNamedService(name);
        }
    }
}