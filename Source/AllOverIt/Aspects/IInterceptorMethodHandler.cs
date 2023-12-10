using System.Reflection;

namespace AllOverIt.Aspects
{
    /// <summary>Represents a interceptor (dispatch proxy) handler for one or more methods on an object.</summary>
    public interface IInterceptorMethodHandler

    {
        /// <summary>The <see cref="MethodInfo"/> for the method(s) this handler supports.</summary>
        MethodInfo[] TargetMethods { get; }

        /// <summary>This method is called before the intercepted method is called on the decorated instance.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments provided to the method being intercepted.</param>
        /// <param name="result">Will be <see langword="null"/> when called. If set to an alternative result then the intercepted
        /// method will be considered handled and the decorated instance method will not be called. The
        /// <see cref="AfterInvoke(MethodInfo, object[], InterceptorState, ref object)"/> method will still be called.</param>
        /// <returns>The interceptor state that will be forwarded to the <see cref="AfterInvoke(MethodInfo, object[], InterceptorState, ref object)"/> method.</returns>
        InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref object result);

        /// <summary>This method is called after the method interception has completed.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments provided to the method being intercepted.</param>
        /// <param name="state">The interceptor state returned from the <see cref="BeforeInvoke(MethodInfo, ref object[], ref object)"/> method.</param>
        /// <param name="result">The result returned from the decorated service, or the <see cref="BeforeInvoke(MethodInfo, ref object[], ref object)"/>
        /// method if it was 'handled' there. This is the final opportunity to change the result, if required.</param>
        void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, ref object result);
    }
}
