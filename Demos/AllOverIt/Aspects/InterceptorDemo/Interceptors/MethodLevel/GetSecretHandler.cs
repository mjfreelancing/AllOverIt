using AllOverIt.Aspects;
using AllOverIt.Assertion;
using System.Reflection;

namespace InterceptorDemo.Interceptors.MethodLevel
{
    internal sealed class GetSecretHandler : InterceptorMethodHandlerBase<string>
    {
        private readonly long _minimumReportableMilliseconds;
        private readonly bool _useCache;

        public override MethodInfo[] TargetMethods { get; } = [typeof(ISecretService).GetMethod(nameof(ISecretService.GetSecret))!];

        public GetSecretHandler(long minimimReportableMilliseconds, bool useCache)
        {
            _minimumReportableMilliseconds = minimimReportableMilliseconds;
            _useCache = useCache;
        }

        protected override InterceptorState<string> BeforeMethodInvoke(MethodInfo? targetMethod, ref object?[]? args)
        {
            // This demo assumes args is not null
            _ = args.WhenNotNull();

            var accessKey = (string) args[0]!;

            args[0] = accessKey.ToUpperInvariant();

            string? result = default;
            var isHandled = false;

            var methodName = targetMethod!.Name;

            if (_useCache)
            {
                // Pretend to get the result from a cache here - just setting a value
                var cachedValue = accessKey.ToArray();
                Array.Reverse(cachedValue);

                result = new string(cachedValue).ToLowerInvariant();
                isHandled = true;

                Console.WriteLine($"Before {methodName}({accessKey}) - using a cache");
            }
            else
            {
                Console.WriteLine($"Before {methodName}({accessKey}) - not using a cache");
            }

            return new TimedInterceptorState<string>
            {
                Result = result,
                IsHandled = isHandled
            };
        }

        protected override void AfterMethodInvoke(MethodInfo targetMethod, object?[]? args, InterceptorState<string> state)
        {
            // This demo assumes state.Result is not null
            _ = args.WhenNotNull();
            _ = state.Result.WhenNotNull();

            var accessKey = (string) args[0]!;

            Console.WriteLine($"After {targetMethod.Name}, arg[0] = {accessKey}, state result = {state.Result}");

            state.Result = state.Result.ToLowerInvariant();

            Console.WriteLine($"  => Modified state result rrto {state.Result}");

            CheckElapsedPeriod(state);
        }

        private void CheckElapsedPeriod(InterceptorState<string> state)
        {
            var timedState = (TimedInterceptorState<string>) state;
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