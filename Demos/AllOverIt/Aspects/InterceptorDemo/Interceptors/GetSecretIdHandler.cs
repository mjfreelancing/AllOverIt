using AllOverIt.Aspects;
using System.Reflection;

namespace InterceptorDemo.Interceptors
{
    internal sealed class GetSecretIdHandler : InterceptorMethodHandlerBase<int>
    {
        public override MethodInfo[] TargetMethods { get; } = [typeof(ISecretService).GetMethod(nameof(ISecretService.GetSecretId))];

        protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref int result)
        {
            return base.BeforeInvoke(targetMethod, ref args, ref result);
        }
    }
}