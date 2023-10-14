using System.Reflection;
using System.Threading.Tasks;

namespace AllOverIt.Aspects.Interceptor
{
    // Interceptors for methods with a void return type
    public abstract class InterceptorHandlerBase : IInterceptorHandler
    {
        public abstract MethodInfo[] TargetMethods { get; }

        public InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref object result)
        {
            return BeforeInvoke(targetMethod, ref args);
        }

        public void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, ref object result)
        {
            AfterInvoke(targetMethod, args, state);
        }

        protected virtual InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args)
        {
            return InterceptorState.None;
        }

        protected virtual void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state)
        {
        }
    }


    // Interceptors for methods with a TResult return type
    public abstract class InterceptorHandlerBase<TResult> : IInterceptorHandler
    {
        public abstract MethodInfo[] TargetMethods { get; }

        public InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref object result)
        {
            var typedResult = (TResult) result;

            var state = BeforeInvoke(targetMethod, ref args, ref typedResult);

            result = typedResult;

            return state;
        }

        public void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, ref object result)
        {
            var typedResult = (TResult) result;

            result = AfterInvoke(targetMethod, args, state, typedResult);
        }

        protected virtual InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref TResult result)
        {
            return InterceptorState.None;
        }

        protected virtual TResult AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, TResult result)
        {
            return result;
        }
    }



    // Interceptors for methods with a Task return type
    public abstract class InterceptorHandlerAsyncBase : IInterceptorHandler
    {
        public abstract MethodInfo[] TargetMethods { get; }

        public InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref object result)
        {
            var typedResult = (Task) result;

            var state = BeforeInvoke(targetMethod, ref args, ref typedResult);

            result = typedResult;

            return state;
        }

        public void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, ref object result)
        {
            var typedResult = (Task) result;

            result = AfterInvoke(targetMethod, args, state, typedResult);
        }

        protected virtual InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref Task result)
        {
            return InterceptorState.None;
        }

        protected virtual Task AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, Task result)
        {
            return result;
        }
    }


    // Interceptors for methods with a Task<TResult> return type
    public abstract class InterceptorHandlerAsyncBase<TResult> : IInterceptorHandler
    {
        public abstract MethodInfo[] TargetMethods { get; }

        public InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref object result)
        {
            var typedResult = (Task<TResult>) result;

            var state = BeforeInvoke(targetMethod, ref args, ref typedResult);

            result = typedResult;

            return state;
        }

        public void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, ref object result)
        {
            var typedResult = (Task<TResult>) result;

            result = AfterInvoke(targetMethod, args, state, typedResult);
        }

        protected virtual InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref Task<TResult> result)
        {
            return InterceptorState.None;
        }

        protected virtual Task<TResult> AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, Task<TResult> result)
        {
            return result;
        }
    }

}
