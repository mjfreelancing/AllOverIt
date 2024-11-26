using AllOverIt.Aspects;
using System.Reflection;

namespace InterceptorDemo.Interceptors.MethodLevel
{
    internal sealed class GetSecretIdHandler : InterceptorMethodHandlerBase<int>
    {
        public override MethodInfo[] TargetMethods { get; } = [typeof(ISecretService).GetMethod(nameof(ISecretService.GetSecretId))!];

        protected override InterceptorState<int> BeforeMethodInvoke(MethodInfo targetMethod, ref object?[]? args)
        {
            Console.WriteLine($"Before {targetMethod.Name} - returning a handled result");

            return new InterceptorState<int>
            {
                Result = 21,
                IsHandled = true
            };
        }
    }
}