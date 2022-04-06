using AllOverIt.Assertion;
using AllOverIt.DependencyInjection;
using AllOverIt.Extensions;
using System;

namespace ExternalDependencies
{
    public sealed class ExternalDependenciesRegistrar : ServiceRegistrarBase
    {
        private readonly Action<string> _logAction;

        // Just for the demo - cannot use DI to inject a logger as this class is used during the registration process
        public ExternalDependenciesRegistrar(Action<string> logAction)
        {
            _logAction = logAction.WhenNotNull();
        }

        protected override bool IncludeRegistration(Type serviceType, Type implementationType)
        {
            _logAction.Invoke($"Register {serviceType.GetFriendlyName()} => {implementationType.GetFriendlyName()}");

            return true;
        }
    }
}