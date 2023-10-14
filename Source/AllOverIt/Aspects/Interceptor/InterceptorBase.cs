using AllOverIt.Extensions;
using AllOverIt.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace AllOverIt.Aspects.Interceptor
{









    public class MethodInterceptor<TService> : InterceptorBase<TService>
    {
        private readonly IDictionary<MethodInfo, IInterceptorHandler> _methodInterceptors = new Dictionary<MethodInfo, IInterceptorHandler>();

        public MethodInterceptor<TService> AddMethodHandler(IInterceptorHandler methodInterceptor)
        {
            foreach (var targetMethod in methodInterceptor.TargetMethods)
            {
                _methodInterceptors.Add(targetMethod, methodInterceptor);
            }

            return this;
        }

        protected override bool ShouldInterceptMethod(MethodInfo targetMethod)
        {
            return _methodInterceptors.ContainsKey(targetMethod);
        }

        protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref object result)
        {
            var methodInterceptor = _methodInterceptors[targetMethod];

            return methodInterceptor.BeforeInvoke(targetMethod, ref args, ref result);
        }

        protected override void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, ref object result)
        {
            var methodInterceptor = _methodInterceptors[targetMethod];

            methodInterceptor.AfterInvoke(targetMethod, args, state, ref result);
        }
    }





    /// <summary>Provides a base class for all interceptors (dispatch proxies) created via
    /// <see cref="InterceptorFactory.CreateInterceptor{TServiceType, TInterceptor}(TServiceType, Action{TInterceptor})"/>.
    /// Derived Interceptors must be public and non-sealed as they are the base class for the generated proxy.</summary>
    /// <typeparam name="TServiceType"></typeparam>
    public abstract class InterceptorBase<TServiceType> : DispatchProxy
    {
        internal TServiceType _serviceInstance;

        /// <summary>Determines if this interceptor should be invoked for the provided target method. All methods will pass
        /// through this interceptor if not overriden.</summary>
        /// <param name="targetMethod">The method info for the method being intercepted.</param>
        /// <returns><see langword="True"/> if the interceptor can handle the method, otherwise <see langword="False"/>.</returns>
        protected virtual bool ShouldInterceptMethod(MethodInfo targetMethod)
        {
            return true;
        }

        /// <summary>The <see cref="BeforeInvoke(MethodInfo, ref object[], ref object)"/> method is called before calling the decorated
        /// object's method, and ends with calling <see cref="AfterInvoke(MethodInfo, object[], InterceptorState, ref object)"/> if
        /// no exception is raised, otherwise <see cref="Faulted(MethodInfo, object[], InterceptorState, Exception)"/>
        /// is called.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments passed to the intercepted method.</param>
        /// <returns>The result of the method invoked on the decorated instance.</returns>
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (!ShouldInterceptMethod(targetMethod))
            {
                return InvokeServiceInstance(targetMethod, args);
            }

            object result = default;

            var state = BeforeInvoke(targetMethod, ref args, ref result);

            try
            {
                var resultHandled = result is not null;

                if (!resultHandled)
                {
                    result = InvokeServiceInstance(targetMethod, args);
                }

                if (result is Task taskResult)
                {
                    result = GetAsyncResult(targetMethod, args, state, taskResult);
                }
                else
                {
                    AfterInvoke(targetMethod, args, state, ref result);
                }
            }
            catch (TargetInvocationException exception)
            {
                // The InnerException will never be null - it holds the underlying exception thrown by the invoked method
                var fault = exception.InnerException;

                Faulted(targetMethod, args, state, fault);

                ExceptionDispatchInfo.Capture(fault).Throw();
            }

            return result;
        }

        /// <summary>Called before the decorated instance method is called.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments passed to the intercepted method, passed by ref.</param>
        /// <param name="result">Can be set to a result compatible with the method call. If the method being intercepted
        /// returns a <see cref="Task{T}"/> be sure to wrap the value in a call to <c>Task.FromResult()</c>.
        /// If the <see cref="InterceptorState.Handled"/> property is set to <see langword="True"/> then this result will
        /// be returned to the caller without invoking the actual service that is being decorated, although the
        /// <see cref="AfterInvoke(MethodInfo, object[], InterceptorState, ref object)"/> method will be called.</param>
        /// <returns>A state object that will be passed to the <see cref="AfterInvoke(MethodInfo, object[], InterceptorState, ref object)"/>
        /// or <see cref="Faulted(MethodInfo, object[], InterceptorState, Exception)"/> method, as applicable.
        /// If no state is required then return <see cref="InterceptorState.Unit"/>.</returns>
        protected virtual InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref object result)
        {
            return InterceptorState.None;
        }

        /// <summary>Called after the instance method has completed execution without faulting (throwing an exception).</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments passed to the intercepted method.</param>
        /// <param name="state">The state object returned by <see cref="BeforeInvoke(MethodInfo, ref object[], ref object)"/>.
        /// If the <see cref="BeforeInvoke(MethodInfo, ref object[], ref object)"/> method is not overriden then this will
        /// be <see cref="InterceptorState.Unit"/>.</param>
        /// <param name="result">Can be set to a result compatible with the method call. If the method being intercepted
        /// returns a <see cref="Task{T}"/> be sure to wrap the value in a call to <c>Task.FromResult()</c>.</param>
        protected virtual void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, ref object result)
        {
        }

        /// <summary>Called when the decorated instance method invocation faults (throws an exception).</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments passed to the intercepted method.</param>
        /// <param name="state">The state object returned by <see cref="BeforeInvoke(MethodInfo, ref object[], ref object)"/>.
        /// If the <see cref="BeforeInvoke(MethodInfo, ref object[], ref object)"/> method is not overriden then this will
        /// be <see cref="InterceptorState.Unit"/>.</param>
        /// <param name="exception">The exception that was thrown by the instance method.</param>
        protected virtual void Faulted(MethodInfo targetMethod, object[] args, InterceptorState state, Exception exception)
        {
        }

        private object GetAsyncResult(MethodInfo targetMethod, object[] args, InterceptorState state, Task taskResult)
        {
            // Without interception, taskResult is what would normally be awaited by the caller. We need to call AfterInvoke()
            // before that await completes so we need to instead return a different task; one that is backed by a
            // TaskCompletionSource<>. For the case where a method as a void or Task return type we will use a
            // TaskCompletionSource<object>, and for all other cases we'll create a TaskCompletionSource<TResult> based on
            // the intercepted method's return type.
            //
            // The original 'taskResult' can then have a continuation appended that allows for the Faulted() / AfterInvoke()
            // calls, followed by setting the final result.

            var methodReturnType = targetMethod.ReturnType;

            var hasReturnType = methodReturnType.IsDerivedFrom(typeof(Task<>));

            var tcsReturnType = hasReturnType
                ? methodReturnType.GetGenericArguments()[0]
                : CommonTypes.ObjectType;

            // Get TaskCompletionSource<T> Type (where T could be object or the method's return type).
            var tcsType = typeof(TaskCompletionSource<>).MakeGenericType(tcsReturnType);

            // Create an instance of TaskCompletionSource<T>.
            var tcs = Activator.CreateInstance(tcsType);

            // Get the Task from the TaskCompletionSource<T> that the caller will now be awaiting.
            var result = tcsType.GetProperty("Task").GetValue(tcs, null);

            // Add a continuation so we can call Faulted() / AfterInvoke() before setting the final result to be returned.
            taskResult
                .ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        var exception = task.Exception.InnerException;

                        Faulted(targetMethod, args, state, exception);

                        // Make sure the task throws when it is awaited
                        tcs.InvokeMethod(tcsType, "SetException", [typeof(Exception)], [exception]);
                    }
                    else
                    {
                        // Start with either Task or Task<T>
                        object completion = task;

                        AfterInvoke(targetMethod, args, state, ref completion);

                        // The TaskCompletionSource needs to be set the result returned by the decorated service / interceptor,
                        // or null if the method's return type is void or Task.
                        var returnValue = hasReturnType
                            ? completion.GetPropertyValue(methodReturnType, "Result")
                            : null;

                        // Set the final result.
                        tcs.InvokeMethod(tcsType, "SetResult", [returnValue]);
                    }

                }, TaskContinuationOptions.ExecuteSynchronously);

            return result;
        }

        private object InvokeServiceInstance(MethodInfo targetMethod, object[] args)
        {
            return targetMethod.Invoke(_serviceInstance, args);
        }
    }
}
