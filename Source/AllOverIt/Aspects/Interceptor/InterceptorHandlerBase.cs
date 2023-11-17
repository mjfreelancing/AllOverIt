using System.Reflection;
using System.Threading.Tasks;

namespace AllOverIt.Aspects.Interceptor
{
    /// <summary>Provides a base class for implementing a method-level interceptor that has a void return type.</summary>
    public abstract class InterceptorHandlerBase : IInterceptorHandler
    {
        /// <inheritdoc />
        public abstract MethodInfo[] TargetMethods { get; }

        /// <inheritdoc />
        public InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref object result)
        {
            return BeforeInvoke(targetMethod, ref args);
        }

        /// <inheritdoc />
        public void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, ref object result)
        {
            AfterInvoke(targetMethod, args, state);
        }

        /// <summary>This method is called before the intercepted method is called on the decorated instance.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments provided to the method being intercepted.</param>
        /// <returns>The interceptor state that will be forwarded to the <see cref="AfterInvoke(MethodInfo, object[], InterceptorState, ref object)"/> method.</returns>
        protected virtual InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args)
        {
            return InterceptorState.None;
        }

        /// <summary>This method is called after the method interception has completed.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments provided to the method being intercepted.</param>
        /// <param name="state">The interceptor state returned from the <see cref="BeforeInvoke(MethodInfo, ref object[], ref object)"/> method.</param>
        protected virtual void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state)
        {
        }
    }

    /// <summary>Provides a base class for implementing a method-level interceptor that has a <typeparamref name="TResult"/> return type.</summary>
    public abstract class InterceptorHandlerBase<TResult> : IInterceptorHandler
    {
        /// <inheritdoc />
        public abstract MethodInfo[] TargetMethods { get; }

        /// <inheritdoc />
        public InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref object result)
        {
            var typedResult = (TResult) result;

            var state = BeforeInvoke(targetMethod, ref args, ref typedResult);

            result = typedResult;

            return state;
        }

        /// <inheritdoc />
        public void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, ref object result)
        {
            var typedResult = (TResult) result;

            result = AfterInvoke(targetMethod, args, state, typedResult);
        }

        /// <summary>This method is called before the intercepted method is called on the decorated instance.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments provided to the method being intercepted.</param>
        /// <param name="result">Will be the <typeparamref name="TResult"/> <see langword="default"/> value when called. If set to an
        /// alternative result then the intercepted method will be considered handled and the decorated instance method will not be called.
        /// The <see cref="AfterInvoke(MethodInfo, object[], InterceptorState, ref object)"/> method will still be called.</param>
        /// <returns>The interceptor state that will be forwarded to the <see cref="AfterInvoke(MethodInfo, object[], InterceptorState, ref object)"/> method.</returns>
        protected virtual InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref TResult result)
        {
            return InterceptorState.None;
        }

        /// <summary>This method is called after the method interception has completed.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments provided to the method being intercepted.</param>
        /// <param name="state">The interceptor state returned from the <see cref="BeforeInvoke(MethodInfo, ref object[], ref object)"/> method.</param>
        /// <param name="result">The result returned from the decorated service, or the <see cref="BeforeInvoke(MethodInfo, ref object[], ref object)"/>
        /// method if it was 'handled' there. This is the final opportunity to change the result, if required.</param>
        /// <returns>The final result to be returned from the caller.</returns>
        protected virtual TResult AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, TResult result)
        {
            return result;
        }
    }

    /// <summary>Provides a base class for implementing a method-level interceptor that has a <seealso cref="Task"/> return type.</summary>
    public abstract class InterceptorHandlerAsyncBase : IInterceptorHandler
    {
        /// <inheritdoc />
        public abstract MethodInfo[] TargetMethods { get; }

        /// <inheritdoc />
        public InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref object result)
        {
            var typedResult = (Task) result;

            var state = BeforeInvoke(targetMethod, ref args, ref typedResult);

            result = typedResult;

            return state;
        }

        /// <inheritdoc />
        public void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, ref object result)
        {
            var typedResult = (Task) result;

            result = AfterInvoke(targetMethod, args, state, typedResult);
        }

        /// <summary>This method is called before the intercepted method is called on the decorated instance.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments provided to the method being intercepted.</param>
        /// <param name="result">Will be <see langword="null"/> when called. If set to an
        /// alternative result then the intercepted method will be considered handled and the decorated instance method will not be called.
        /// The <see cref="AfterInvoke(MethodInfo, object[], InterceptorState, ref object)"/> method will still be called.</param>
        /// <returns>The interceptor state that will be forwarded to the <see cref="AfterInvoke(MethodInfo, object[], InterceptorState, ref object)"/> method.</returns>
        protected virtual InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref Task result)
        {
            return InterceptorState.None;
        }

        /// <summary>This method is called after the method interception has completed.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments provided to the method being intercepted.</param>
        /// <param name="state">The interceptor state returned from the <see cref="BeforeInvoke(MethodInfo, ref object[], ref Task)"/> method.</param>
        /// <param name="result">The result returned from the decorated service, or the <see cref="BeforeInvoke(MethodInfo, ref object[], ref Task)"/>
        /// method if it was 'handled' there. This is the final opportunity to change the result, if required.</param>
        /// <returns>The final result to be returned from the caller.</returns>
        protected virtual Task AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, Task result)
        {
            return result;
        }
    }

    /// <summary>Provides a base class for implementing a method-level interceptor that has a <seealso cref="Task{TResult}"/> return type.</summary>
    public abstract class InterceptorHandlerAsyncBase<TResult> : IInterceptorHandler
    {
        /// <inheritdoc />
        public abstract MethodInfo[] TargetMethods { get; }

        /// <inheritdoc />
        public InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref object result)
        {
            var typedResult = (Task<TResult>) result;

            var state = BeforeInvoke(targetMethod, ref args, ref typedResult);

            result = typedResult;

            return state;
        }

        /// <inheritdoc />
        public void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, ref object result)
        {
            var typedResult = (Task<TResult>) result;

            result = AfterInvoke(targetMethod, args, state, typedResult);
        }

        /// <summary>This method is called before the intercepted method is called on the decorated instance.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments provided to the method being intercepted.</param>
        /// <param name="result">Will be <see langword="null"/> when called. If set to an  alternative result then the intercepted
        /// method will be considered handled and the decorated instance method will not be called.
        /// The <see cref="AfterInvoke(MethodInfo, object[], InterceptorState, Task{TResult})"/> method will still be called.</param>
        /// <returns>The interceptor state that will be forwarded to the <see cref="AfterInvoke(MethodInfo, object[], InterceptorState, ref object)"/> method.</returns>
        protected virtual InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref Task<TResult> result)
        {
            return InterceptorState.None;
        }

        /// <summary>This method is called after the method interception has completed.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments provided to the method being intercepted.</param>
        /// <param name="state">The interceptor state returned from the <see cref="BeforeInvoke(MethodInfo, ref object[], ref Task{TResult})"/> method.</param>
        /// <param name="result">The result returned from the decorated service, or the <see cref="BeforeInvoke(MethodInfo, ref object[], ref Task{TResult})"/>
        /// method if it was 'handled' there. This is the final opportunity to change the result, if required.</param>
        /// <returns>The final result to be returned from the caller.</returns>
        protected virtual Task<TResult> AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, Task<TResult> result)
        {
            return result;
        }
    }
}
