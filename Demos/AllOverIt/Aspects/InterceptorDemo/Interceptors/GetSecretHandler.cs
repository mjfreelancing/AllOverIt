using AllOverIt.Aspects.Interceptor;
using System;
using System.Reflection;

namespace InterceptorDemo.Interceptors
{
    internal sealed class GetSecretHandler : InterceptorHandlerBase<string>
    {
        private readonly long _minimimReportableMilliseconds;

        public override MethodInfo[] TargetMethods { get; } = [typeof(ISecretService).GetMethod(nameof(ISecretService.GetSecret))];

        public GetSecretHandler(long minimimReportableMilliseconds)
        {
            _minimimReportableMilliseconds = minimimReportableMilliseconds;
        }

        protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref string result)
        {
            var accessKey = (string) args[0];

            args[0] = accessKey.ToUpperInvariant();

            Console.WriteLine($"Before {targetMethod.Name}({accessKey})");

            return new TimedInterceptorState();
        }

        protected override string AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, string result)
        {
            var accessKey = (string) args[0];

            Console.WriteLine($"After {targetMethod.Name}({accessKey}), result = {result}");

            result = result.ToLowerInvariant();

            Console.WriteLine($"  => Result modified to {result}");

            CheckElapsedPeriod(state);

            return result;
        }

        private void CheckElapsedPeriod(InterceptorState state)
        {
            var timedState = state as TimedInterceptorState;
            var elapsed = timedState.Stopwatch.ElapsedMilliseconds;

            if (elapsed >= _minimimReportableMilliseconds)
            {
                Console.WriteLine($" >> WARNING: Elapsed exceeded minimum {_minimimReportableMilliseconds}ms, actual = {elapsed}ms");
            }
            else
            {
                Console.WriteLine($" >> Elapsed = {elapsed}ms");
            }
        }
    }
}