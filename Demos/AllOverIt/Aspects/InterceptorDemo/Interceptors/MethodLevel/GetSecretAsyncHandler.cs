using AllOverIt.Aspects;
using InterceptorDemo.Interceptors.ClassLevel;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace InterceptorDemo.Interceptors.MethodLevel
{
    internal sealed class GetSecretAsyncHandler : InterceptorMethodHandlerAsyncBase<string>
    {
        private readonly long _minimumReportableMilliseconds;

        public override MethodInfo[] TargetMethods { get; } = [typeof(ISecretService).GetMethod(nameof(ISecretService.GetSecretAsync))];

        public GetSecretAsyncHandler(long minimimReportableMilliseconds)
        {
            _minimumReportableMilliseconds = minimimReportableMilliseconds;
        }

        protected override InterceptorState<Task<string>> DoBeforeInvoke(MethodInfo targetMethod, ref object[] args)
        {
            var accessKey = (string) args[0];

            args[0] = accessKey.ToUpperInvariant();

            Console.WriteLine($"Before {targetMethod.Name}({accessKey})");

            return new TimedInterceptorState<Task<string>>();
        }

        protected override void DoAfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState<Task<string>> state)
        {
            var accessKey = (string) args[0];

            var result = state.Result;

            // Using Task.Result is safe because AfterInvoke() is only called if the Task completed successfully.
            var value = result.Result;

            Console.WriteLine($"After {targetMethod.Name}({accessKey}), result = {value}");

            value = value.ToLowerInvariant();

            Console.WriteLine($"  => Result modified to {value}");

            CheckElapsedPeriod(state);

            state.Result = Task.FromResult(value);
        }

        private void CheckElapsedPeriod(InterceptorState<Task<string>> state)
        {
            var timedState = state as TimedInterceptorState<Task<string>>;
            var elapsed = timedState.Stopwatch.ElapsedMilliseconds;

            if (elapsed >= _minimumReportableMilliseconds)
            {
                Console.WriteLine($" >> WARNING: Elapsed exceeded minimum {_minimumReportableMilliseconds}ms, actual = {elapsed}ms");
            }
            else
            {
                Console.WriteLine($" >> Elapsed = {elapsed}ms");
            }
        }
    }
}