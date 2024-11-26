using AllOverIt.Aspects;
using AllOverIt.Assertion;
using System.Reflection;

namespace InterceptorDemo.Interceptors.ClassLevel
{
    // Note: Interceptors cannot be sealed as they are the base class for the generated proxy.
    // Change input arg
    internal class ChangeInputArgInterceptor : InterceptorBase<ISecretService>
    {
        private static readonly MethodInfo GetSecretMethodInfo = typeof(ISecretService).GetMethod(nameof(ISecretService.GetSecret))!;
        private static readonly MethodInfo GetSecretAsyncMethodInfo = typeof(ISecretService).GetMethod(nameof(ISecretService.GetSecretAsync))!;

        // Only intercept these methods
        private static readonly MethodInfo[] FilteredMethods = [GetSecretMethodInfo, GetSecretAsyncMethodInfo];

        protected override InterceptorState BeforeInvoke(MethodInfo? targetMethod, ref object?[]? args)
        {
            // This demo assumes args is not null
            _ = args.WhenNotNull();

            var methodName = targetMethod!.Name;

            if (!FilteredMethods.Contains(targetMethod))
            {
                Console.WriteLine($"Skipping {methodName}");
            }
            else
            {
                var accessKey = (string) args[0]!;

                args[0] = accessKey.ToUpperInvariant();

                Console.WriteLine($"Before {methodName}, changing arg[0] from '{accessKey}' to '{args[0]}'");
            }

            return new InterceptorState();
        }

        protected override void AfterInvoke(MethodInfo targetMethod, object?[]? args, InterceptorState state)
        {
            // This demo assumes args is not null
            _ = args.WhenNotNull();

            if (!FilteredMethods.Contains(targetMethod))
            {
                return;
            }

            var accessKey = (string) args[0]!;

            Console.WriteLine($"After {targetMethod.Name}, arg[0] is now '{accessKey}'");
        }
    }


}