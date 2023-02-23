﻿using System;
using System.Reflection;

namespace AllOverIt.Aspects.Interceptor
{
    /// <summary>Provides a factory that creates an interceptor (proxy) for a provided service instance.</summary>
    public static class InterceptorFactory
    {
        // Note: Indirectly tested via InterceptorBaseFixture

        /// <summary>Creates an interceptor (proxy) that derives from <typeparamref name="TInterceptor"/>, which must be a <see cref="InterceptorBase{TServiceType}"/>,
        /// and implements <typeparamref name="TServiceType"/>.</summary>
        /// <typeparam name="TServiceType">The interface type that the interceptor implements.</typeparam>
        /// <typeparam name="TInterceptor">The base class for the interceptor, which must be a <see cref="InterceptorBase{TServiceType}"/>.</typeparam>
        /// <param name="serviceInstance">The object instance to be intercepted.</param>
        /// <param name="configure">An optional configuration option that allows for customization of the created interceptor.</param>
        /// <returns>An interceptor that implements <typeparamref name="TServiceType"/>.</returns>
        public static TServiceType CreateInterceptor<TServiceType, TInterceptor>(TServiceType serviceInstance, Action<TInterceptor> configure = default)
            where TInterceptor : InterceptorBase<TServiceType>
        {
            object proxyInstance = DispatchProxy.Create<TServiceType, TInterceptor>();

            var proxyDecorator = (InterceptorBase<TServiceType>) proxyInstance;
            proxyDecorator._serviceInstance = serviceInstance;

            if (configure is not null)
            {
                var interceptor = (TInterceptor) proxyInstance;
                configure.Invoke(interceptor);
            }

            return (TServiceType) proxyInstance;
        }
    }
}
