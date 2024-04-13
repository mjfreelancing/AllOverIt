using System;
using System.Reflection;

namespace AllOverIt.Aspects
{
    /// <summary>Represents an interceptor's (dispatch proxy) method handler for one or more methods on an object.</summary>
    public interface IInterceptorMethodHandler
    {
        /// <summary>The <see cref="MethodInfo"/> for the method(s) this handler supports.</summary>
        MethodInfo[] TargetMethods { get; }

        /// <summary>This method is called before the intercepted method is called on the decorated instance.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments provided to the method being intercepted.</param>
        /// <returns>The interceptor state that will be forwarded to the <see cref="AfterInvoke(MethodInfo, object[], InterceptorState)"/> method.</returns>
        InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object?[]? args);

        /// <summary>This method is called after the method interception has completed.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments provided to the method being intercepted.</param>
        /// <param name="state">The interceptor state returned from the <see cref="BeforeInvoke(MethodInfo, ref object[])"/> method.</param>
        void AfterInvoke(MethodInfo targetMethod, object?[]? args, InterceptorState state);

        /// <summary>This method is called when the decorated instance method invocation faults (throws an exception).</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments passed to the intercepted method.</param>
        /// <param name="state">The state object returned by <see cref="BeforeInvoke(MethodInfo, ref object[])"/>.</param>
        /// <param name="exception">The exception that was thrown by the instance method.</param>
        void Faulted(MethodInfo targetMethod, object?[]? args, InterceptorState state, Exception exception);
    }
}
