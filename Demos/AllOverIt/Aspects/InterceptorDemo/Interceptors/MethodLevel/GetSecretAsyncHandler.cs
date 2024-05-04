using AllOverIt.Aspects;
using System.Reflection;

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

        protected override InterceptorState<Task<string>> BeforeMethodInvoke(MethodInfo targetMethod, ref object[] args)
        {
            var accessKey = (string) args[0];

            args[0] = accessKey.ToUpperInvariant();

            Console.WriteLine($"Before {targetMethod.Name}({accessKey})");

            return new TimedInterceptorState<Task<string>>();
        }

        protected override void AfterMethodInvoke(MethodInfo targetMethod, object[] args, InterceptorState<Task<string>> state)
        {
            var accessKey = (string) args[0];

            var stateResult = state.Result;

            // Using Task.Result is safe because DoAfterInvoke() is only called if the Task completed successfully.
            var value = stateResult.Result;

            Console.WriteLine($"After {targetMethod.Name}, arg[0] = {accessKey}, state result = {state.Result}");

            value = value.ToLowerInvariant();

            state.Result = Task.FromResult(value);

            Console.WriteLine($"  => Modified state result to {state.Result}");

            CheckElapsedPeriod(state);
        }

        protected override void MethodFaulted(MethodInfo targetMethod, object[] args, InterceptorState<Task<string>> state, Exception exception)
        {
            base.MethodFaulted(targetMethod, args, state, exception);

            Console.WriteLine($"  => Faulted: {exception.Message}");
        }

        private void CheckElapsedPeriod(InterceptorState<Task<string>> state)
        {
            var timedState = state as TimedInterceptorState<Task<string>>;
            var elapsed = timedState.Stopwatch.ElapsedMilliseconds;

            if (elapsed >= _minimumReportableMilliseconds)
            {
                Console.WriteLine($"  >> WARNING: Elapsed exceeded minimum {_minimumReportableMilliseconds}ms, actual = {elapsed}ms");
            }
            else
            {
                Console.WriteLine($"  >> Elapsed = {elapsed}ms");
            }
        }
    }
}