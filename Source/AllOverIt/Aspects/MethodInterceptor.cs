using System.Collections.Generic;
using System.Reflection;

namespace AllOverIt.Aspects
{
    // A more advanced implementation: https://github.com/castleproject/Core/blob/master/docs/dynamicproxy.md

    /// <summary>Provides the ability to register one or more method intercept handlers for a given <typeparamref name="TService"/>.
    /// Use this in preference to <see cref="InterceptorBase{TServiceType}"/> when you require more granular control of your
    /// interceptors rather than execute the same code for all methods on the service.</summary>
    /// <typeparam name="TService"></typeparam>
    public class MethodInterceptor<TService> : InterceptorBase<TService>        // Cannot be sealed (required for the generated proxy)
    {
        private readonly Dictionary<MethodInfo, IInterceptorMethodHandler> _methodInterceptors = [];

        /// <summary>Adds a method intercept handler that will be invoked when the method to be intercepted matches
        /// one of the associated the <see cref="IInterceptorHandler.TargetMethods"/>.</summary>
        /// <param name="methodInterceptor">The method intercept handler instance.</param>
        /// <returns>The same instance so method calls can be chained.</returns>
        public MethodInterceptor<TService> AddMethodHandler(IInterceptorMethodHandler methodInterceptor)
        {
            foreach (var targetMethod in methodInterceptor.TargetMethods)
            {
                _methodInterceptors.Add(targetMethod, methodInterceptor);
            }

            return this;
        }

        /// <summary>Determines if a intercept handler has been registered for the provided <paramref name="targetMethod"/>.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> of the method to determine if there is a registered interceptor.</param>
        /// <returns><see langword="True"/> if an interceptor is registered, otherwise <see langword="False"/>.</returns>
        protected override bool CanInterceptMethod(MethodInfo targetMethod)
        {
            return _methodInterceptors.ContainsKey(targetMethod);
        }

        /// <inheritdoc />
        protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args)
        {
            var methodInterceptor = _methodInterceptors[targetMethod];

            return methodInterceptor.BeforeInvoke(targetMethod, ref args);
        }

        /// <inheritdoc />
        protected override void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state)
        {
            var methodInterceptor = _methodInterceptors[targetMethod];

            methodInterceptor.AfterInvoke(targetMethod, args, state);
        }
    }
}
