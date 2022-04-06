using System;
using System.Collections.Generic;

namespace AllOverIt.DependencyInjection
{
    public interface IServiceRegistrar
    {
        void AutoRegisterTypes(IEnumerable<Type> serviceTypes, bool includeMatchingInterface, Action<Type, Type> registrationAction);
    }
}