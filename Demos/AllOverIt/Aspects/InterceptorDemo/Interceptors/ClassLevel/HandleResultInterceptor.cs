using AllOverIt.Aspects;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace InterceptorDemo.Interceptors.ClassLevel
{
    // Note: Interceptors cannot be sealed as they are the base class for the generated proxy.
    // Return a handled result - will not called the decorated instance
    internal class HandleResultInterceptor : InterceptorBase<ISecretService>
    {
        private static readonly MethodInfo GetSecretMethodInfo = typeof(ISecretService).GetMethod(nameof(ISecretService.GetSecret));
        private static readonly MethodInfo GetSecretAsyncMethodInfo = typeof(ISecretService).GetMethod(nameof(ISecretService.GetSecretAsync));

        // Only intercept these methods
        private static readonly MethodInfo[] FilteredMethods = [GetSecretMethodInfo, GetSecretAsyncMethodInfo];

        protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args)
        {
            if (!FilteredMethods.Contains(targetMethod))
            {
                Console.WriteLine($"Skipping {targetMethod.Name}");

                return new InterceptorState();
            }

            Console.WriteLine($"Before {targetMethod.Name}");

            var state = new InterceptorState
            {
                IsHandled = true,
            };

            var result = $"{targetMethod.Name} has been intercepted - will not call the decorated instance";

            // This is a classic reason why class level interceptors are not ideal for this scenario. Best to use method level interceptors.
            if (targetMethod == GetSecretMethodInfo)
            {
                state.SetResult(result);
            }
            else
            {
                state.SetResult(Task.FromResult(result));
            }

            return state;
        }

        protected override void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state)
        {
            if (!FilteredMethods.Contains(targetMethod))
            {
                return;
            }

            var stateResult = targetMethod == GetSecretMethodInfo
                ? state.GetResult<string>()
                : state.GetResult<Task<string>>().Result;       // safe since the Task has run to completion if it gets here

            Console.WriteLine($"After {targetMethod.Name}, handled result is '{stateResult}'");
        }
    }


}