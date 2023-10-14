using AllOverIt.Aspects.Interceptor;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace InterceptorDemo.Interceptors
{
    internal sealed class GetSecretAsyncHandler : InterceptorHandlerAsyncBase<string>
    {
        private readonly long _minimimReportableMilliseconds;

        public override MethodInfo[] TargetMethods { get; } = [typeof(ISecretService).GetMethod(nameof(ISecretService.GetSecretAsync))];

        public GetSecretAsyncHandler(long minimimReportableMilliseconds)
        {
            _minimimReportableMilliseconds = minimimReportableMilliseconds;
        }

        protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref Task<string> result)
        {
            var accessKey = (string) args[0];

            args[0] = accessKey.ToUpperInvariant();

            Console.WriteLine($"Before {targetMethod.Name}({accessKey})");

            return new TimedInterceptorState();
        }

        protected override Task<string> AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, Task<string> result)
        {
            var accessKey = (string) args[0];

            // Using Task.Result is safe because AfterInvoke() is only called if the Task completed successfully.
            var value = result.Result;

            Console.WriteLine($"After {targetMethod.Name}({accessKey}), result = {value}");

            value = value.ToLowerInvariant();

            Console.WriteLine($"  => Result modified to {value}");

            CheckElapsedPeriod(state);

            return Task.FromResult(value);
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