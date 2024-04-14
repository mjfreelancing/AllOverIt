using AllOverIt.Aspects;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace InterceptorDemo.Interceptors.ClassLevel
{
    // Note: Interceptors cannot be sealed as they are the base class for the generated proxy.
    // Change the final result
    internal class ChangeFinalResultInterceptor : InterceptorBase<ISecretService>
    {
        private static readonly MethodInfo GetSecretMethodInfo = typeof(ISecretService).GetMethod(nameof(ISecretService.GetSecret))!;
        private static readonly MethodInfo GetSecretAsyncMethodInfo = typeof(ISecretService).GetMethod(nameof(ISecretService.GetSecretAsync))!;

        // Only intercept these methods
        private static readonly MethodInfo[] FilteredMethods = [GetSecretMethodInfo, GetSecretAsyncMethodInfo];

        protected override InterceptorState BeforeInvoke(MethodInfo? targetMethod, ref object?[]? args)
        {
            var methodName = targetMethod!.Name;

            if (!FilteredMethods.Contains(targetMethod))
            {
                Console.WriteLine($"Skipping {methodName}");
            }
            else
            {
                Console.WriteLine($"Before {methodName}");
            }

            return new InterceptorState();
        }

        protected override void AfterInvoke(MethodInfo targetMethod, object?[]? args, InterceptorState state)
        {
            if (!FilteredMethods.Contains(targetMethod))
            {
                return;
            }

            var stateResult = targetMethod == GetSecretMethodInfo
                 ? state.GetResult<string>()!
                 : state.GetResult<Task<string>>()!.Result!;       // safe since the Task has run to completion if it gets here


            Console.WriteLine($"After {targetMethod.Name}, current result is '{stateResult}'");

            stateResult = stateResult.ToUpperInvariant();

            if (targetMethod == GetSecretMethodInfo)
            {
                state.SetResult(stateResult);
            }
            else
            {
                state.SetResult(Task.FromResult(stateResult));
            }

            Console.WriteLine($"  => Result modified to {stateResult}");

        }
    }


}