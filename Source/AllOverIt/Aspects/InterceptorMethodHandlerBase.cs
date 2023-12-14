﻿using System;
using System.Reflection;
using System.Threading.Tasks;

namespace AllOverIt.Aspects
{
    /// <summary>Provides a base class for implementing a method-level interceptor that has a void return type.</summary>
    public abstract class InterceptorMethodHandlerBase : IInterceptorMethodHandler
    {
        /// <inheritdoc />
        public abstract MethodInfo[] TargetMethods { get; }

        /// <inheritdoc />
        InterceptorState IInterceptorMethodHandler.BeforeInvoke(MethodInfo targetMethod, ref object[] args)
        {
            return DoBeforeInvoke(targetMethod, ref args);
        }

        /// <inheritdoc />
        void IInterceptorMethodHandler.AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state)
        {
            DoAfterInvoke(targetMethod, args, state);
        }

        /// <inheritdoc />
        void IInterceptorMethodHandler.Faulted(MethodInfo targetMethod, object[] args, InterceptorState state, Exception exception)
        {
            DoOnFaulted(targetMethod, args, state as InterceptorState, exception);
        }

        /// <summary>This method is called before the intercepted method is called on the decorated instance.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments provided to the method being intercepted.</param>
        /// <returns>The interceptor state that will be forwarded to the <see cref="DoAfterInvoke(MethodInfo, object[], InterceptorState)"/>
        /// method.</returns>
        protected virtual InterceptorState DoBeforeInvoke(MethodInfo targetMethod, ref object[] args)
        {
            return new InterceptorState();
        }

        /// <summary>This method is called after the method interception has completed.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments provided to the method being intercepted.</param>
        /// <param name="state">The interceptor state returned from the <see cref="DoBeforeInvoke(MethodInfo, ref object[])"/> method.</param>
        protected virtual void DoAfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state)
        {
        }

        protected virtual void DoOnFaulted(MethodInfo targetMethod, object[] args, InterceptorState state, Exception exception)
        {
        }
    }

    /// <summary>Provides a base class for implementing a method-level interceptor that has a <typeparamref name="TResult"/> return type.</summary>
    public abstract class InterceptorMethodHandlerBase<TResult> : IInterceptorMethodHandler
    {
        /// <inheritdoc />
        public abstract MethodInfo[] TargetMethods { get; }

        /// <inheritdoc />
        InterceptorState IInterceptorMethodHandler.BeforeInvoke(MethodInfo targetMethod, ref object[] args)
        {
            return BeforeMethodInvoke(targetMethod, ref args);
        }

        /// <inheritdoc />
        void IInterceptorMethodHandler.AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state)
        {
            AfterMethodInvoke(targetMethod, args, state as InterceptorState<TResult>);
        }

        /// <inheritdoc />
        void IInterceptorMethodHandler.Faulted(MethodInfo targetMethod, object[] args, InterceptorState state, Exception exception)
        {
            OnMethodFaulted(targetMethod, args, state as InterceptorState<TResult>, exception);
        }

        /// <summary>This method is called before the intercepted method is called on the decorated instance.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments provided to the method being intercepted.</param>
        /// <returns>The interceptor state that will be forwarded to the <see cref="AfterMethodInvoke(MethodInfo, object[], InterceptorState{TResult})"/>
        /// method.</returns>
        protected virtual InterceptorState<TResult> BeforeMethodInvoke(MethodInfo targetMethod, ref object[] args)
        {
            return new InterceptorState<TResult>();
        }

        /// <summary>This method is called after the method interception has completed.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments provided to the method being intercepted.</param>
        /// <param name="state">The interceptor state returned from the <see cref="BeforeMethodInvoke(MethodInfo, ref object[])"/> method.</param>
        protected virtual void AfterMethodInvoke(MethodInfo targetMethod, object[] args, InterceptorState<TResult> state)
        {
        }

        protected virtual void OnMethodFaulted(MethodInfo targetMethod, object[] args, InterceptorState<TResult> state, Exception exception)
        {

        }
    }

    /// <summary>Provides a base class for implementing a method-level interceptor that has a <seealso cref="Task"/> return type.</summary>
    public abstract class InterceptorMethodHandlerAsyncBase : IInterceptorMethodHandler
    {
        /// <inheritdoc />
        public abstract MethodInfo[] TargetMethods { get; }

        /// <inheritdoc />
        InterceptorState IInterceptorMethodHandler.BeforeInvoke(MethodInfo targetMethod, ref object[] args)
        {
            return DoBeforeInvoke(targetMethod, ref args);
        }

        /// <inheritdoc />
        void IInterceptorMethodHandler.AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state)
        {
            DoAfterInvoke(targetMethod, args, state as InterceptorState<Task>);
        }

        /// <inheritdoc />
        void IInterceptorMethodHandler.Faulted(MethodInfo targetMethod, object[] args, InterceptorState state, Exception exception)
        {
            DoOnFaulted(targetMethod, args, state as InterceptorState<Task>, exception);
        }

        /// <summary>This method is called before the intercepted method is called on the decorated instance.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments provided to the method being intercepted.</param>
        /// <returns>The interceptor state that will be forwarded to the <see cref="DoAfterInvoke(MethodInfo, object[], InterceptorState{Task})"/>
        /// method.</returns>
        protected virtual InterceptorState<Task> DoBeforeInvoke(MethodInfo targetMethod, ref object[] args)
        {
            return new InterceptorState<Task>();
        }

        /// <summary>This method is called after the method interception has completed.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments provided to the method being intercepted.</param>
        /// <param name="state">The interceptor state returned from the <see cref="DoBeforeInvoke(MethodInfo, ref object[])"/> method.</param>
        protected virtual void DoAfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState<Task> state)
        {
        }

        protected virtual void DoOnFaulted(MethodInfo targetMethod, object[] args, InterceptorState<Task> state, Exception exception)
        {

        }
    }

    /// <summary>Provides a base class for implementing a method-level interceptor that has a <seealso cref="Task{TResult}"/> return type.</summary>
    public abstract class InterceptorMethodHandlerAsyncBase<TResult> : IInterceptorMethodHandler
    {
        /// <inheritdoc />
        public abstract MethodInfo[] TargetMethods { get; }

        /// <inheritdoc />
        InterceptorState IInterceptorMethodHandler.BeforeInvoke(MethodInfo targetMethod, ref object[] args)
        {
            return DoBeforeInvoke(targetMethod, ref args);
        }

        /// <inheritdoc />
        void IInterceptorMethodHandler.AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state)
        {
            DoAfterInvoke(targetMethod, args, state as InterceptorState<Task<TResult>>);
        }

        /// <inheritdoc />
        void IInterceptorMethodHandler.Faulted(MethodInfo targetMethod, object[] args, InterceptorState state, Exception exception)
        {
            DoOnFaulted(targetMethod, args, state as InterceptorState<Task<TResult>>, exception);
        }

        /// <summary>This method is called before the intercepted method is called on the decorated instance.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments provided to the method being intercepted.</param>
        /// <returns>The interceptor state that will be forwarded to the <see cref="DoAfterInvoke(MethodInfo, object[], InterceptorState{Task{TResult}})"/>
        /// method.</returns>
        protected virtual InterceptorState<Task<TResult>> DoBeforeInvoke(MethodInfo targetMethod, ref object[] args)
        {
            return new InterceptorState<Task<TResult>>();
        }

        /// <summary>This method is called after the method interception has completed.</summary>
        /// <param name="targetMethod">The <see cref="MethodInfo"/> for the method being intercepted.</param>
        /// <param name="args">The arguments provided to the method being intercepted.</param>
        /// <param name="state">The interceptor state returned from the <see cref="DoBeforeInvoke(MethodInfo, ref object[])"/> method.</param>
        protected virtual void DoAfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState<Task<TResult>> state)
        {
        }

        protected virtual void DoOnFaulted(MethodInfo targetMethod, object[] args, InterceptorState<Task<TResult>> state, Exception exception)
        {

        }
    }
}
