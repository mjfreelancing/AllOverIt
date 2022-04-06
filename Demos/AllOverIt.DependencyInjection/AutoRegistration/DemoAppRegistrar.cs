using System;
using AllOverIt.Assertion;
using AllOverIt.DependencyInjection;
using AllOverIt.Extensions;

namespace AutoRegistration
{
    internal sealed class DemoAppRegistrar : ServiceRegistrarBase
    {
        private readonly Action<string> _logAction;

        // Just for the demo - cannot use DI to inject a logger as this class is used during the registration process
        public DemoAppRegistrar(Action<string> logAction)
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