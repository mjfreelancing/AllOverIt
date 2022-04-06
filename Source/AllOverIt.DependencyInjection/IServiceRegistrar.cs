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
        /// <param name="includeMatchingInterface"></param>
        /// <param name="registrationAction"></param>
        void AutoRegisterServices(IEnumerable<Type> serviceTypes, bool includeMatchingInterface, Action<Type, Type> registrationAction);
    }
}