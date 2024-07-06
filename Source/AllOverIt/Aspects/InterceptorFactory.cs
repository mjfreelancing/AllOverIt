using AllOverIt.Assertion;
using System.Reflection;

namespace AllOverIt.Aspects
{
    /// <summary>Provides a factory that creates an interceptor (proxy) for a provided service instance.</summary>
    public static class InterceptorFactory
    {
        /// <summary>Creates an interceptor (proxy) that derives from <typeparamref name="TInterceptor"/>, which must be a <see cref="InterceptorBase{TServiceType}"/>,
        /// and implements <typeparamref name="TServiceType"/>.</summary>
        /// <typeparam name="TServiceType">The interface type that the interceptor implements.</typeparam>
        /// <typeparam name="TInterceptor">The base class for the interceptor, which must be a <see cref="InterceptorBase{TServiceType}"/>.</typeparam>
        /// <param name="serviceInstance">The object instance to be intercepted.</param>
        /// <param name="configure">An optional configuration action that allows for customization of the created interceptor.</param>
        /// <returns>An interceptor that implements <typeparamref name="TServiceType"/>.</returns>
        public static TServiceType CreateInterceptor<TServiceType, TInterceptor>(TServiceType serviceInstance,
            Action<TInterceptor>? configure = default) where TInterceptor : InterceptorBase<TServiceType>
        {
            var proxyInstance = GetServiceProxy<TServiceType, TInterceptor>(serviceInstance);

            if (configure is not null)
            {
                var interceptor = (TInterceptor) proxyInstance;
                configure.Invoke(interceptor);
            }

            return (TServiceType) proxyInstance;
        }

        /// <summary>Creates an interceptor (proxy) that derives from <typeparamref name="TInterceptor"/>, which must be a <see cref="InterceptorBase{TServiceType}"/>,
        /// and implements <typeparamref name="TServiceType"/>.</summary>
        /// <typeparam name="TServiceType">The interface type that the interceptor implements.</typeparam>
        /// <typeparam name="TInterceptor">The base class for the interceptor, which must be a <see cref="InterceptorBase{TServiceType}"/>.</typeparam>
        /// <param name="serviceInstance">The object instance to be intercepted.</param>
        /// <param name="serviceProvider">The service provider. This can be <see langword="null"/> when <paramref name="configure"/> is <see langword="null"/>.</param>
        /// <param name="configure">An optional configuration action that allows for customization of the created interceptor.</param>
        /// <returns>An interceptor that implements <typeparamref name="TServiceType"/>.</returns>
        public static TServiceType CreateInterceptor<TServiceType, TInterceptor>(TServiceType serviceInstance, IServiceProvider? serviceProvider,
            Action<IServiceProvider, TInterceptor>? configure = default) where TInterceptor : InterceptorBase<TServiceType>
        {
            if (configure is not null)
            {
                _ = serviceProvider.WhenNotNull(nameof(serviceProvider));
            }

            var proxyInstance = GetServiceProxy<TServiceType, TInterceptor>(serviceInstance);

            if (configure is not null)
            {
                var interceptor = (TInterceptor) proxyInstance;
                configure.Invoke(serviceProvider!, interceptor);
            }

            return (TServiceType) proxyInstance;
        }

        private static object GetServiceProxy<TServiceType, TInterceptor>(TServiceType serviceInstance) where TInterceptor : InterceptorBase<TServiceType>
        {
            object proxyInstance = DispatchProxy.Create<TServiceType, TInterceptor>()!;

            var proxyDecorator = (InterceptorBase<TServiceType>) proxyInstance;
            proxyDecorator._serviceInstance = serviceInstance;

            return proxyInstance;
        }
    }
}
