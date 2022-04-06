using System;
using System.Collections.Generic;

namespace AllOverIt.DependencyInjection
{
    public interface IServiceRegistrar
    {
        void AutoRegisterServices(IEnumerable<Type> serviceTypes, bool includeMatchingInterface, Action<Type, Type> registrationAction);
    }
}