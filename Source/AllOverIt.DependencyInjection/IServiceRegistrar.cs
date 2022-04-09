using System;
using System.Collections.Generic;

namespace AllOverIt.DependencyInjection
{
    /// <summary>Represents a registrar responsible for registering services against suitable concrete implementations.</summary>
    public interface IServiceRegistrar
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceTypes"></param>
        /// <param name="registrationAction"></param>
        /// <param name="configure"></param>
        void AutoRegisterServices(IEnumerable<Type> serviceTypes, Action<Type, Type> registrationAction, Action<IServiceRegistrarOptions> configure = default);
    }

    public interface IServiceRegistrarOptions
    {
        void ExcludeTypes(params Type[] types);

        // ServiceType, ImplementationType
        void Filter(Func<Type, Type, bool> filter);
    }
}