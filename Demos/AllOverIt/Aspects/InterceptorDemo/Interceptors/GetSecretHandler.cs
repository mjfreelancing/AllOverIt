using AllOverIt.Aspects;
using System;
using System.Linq;
using System.Reflection;

namespace InterceptorDemo.Interceptors
{
    internal sealed class GetSecretIdHandler : InterceptorHandlerBase<int>
    {
        public override MethodInfo[] TargetMethods { get; } = [typeof(ISecretService).GetMethod(nameof(ISecretService.GetSecretId))];

        protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref int result)
        {
            return base.BeforeInvoke(targetMethod, ref args, ref result);
        }
    }

    internal sealed class GetSecretHandler : InterceptorHandlerBase<string>
    {
        private readonly long _minimimReportableMilliseconds;
        private readonly bool _useCache;

        public override MethodInfo[] TargetMethods { get; } = [typeof(ISecretService).GetMethod(nameof(ISecretService.GetSecret))];

        public GetSecretHandler(long minimimReportableMilliseconds, bool useCache)
        {
            _minimimReportableMilliseconds = minimimReportableMilliseconds;
            _useCache = useCache;
        }

        protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref string result)
        {
            var accessKey = (string) args[0];

            args[0] = accessKey.ToUpperInvariant();

            if (_useCache)
            {
                // Get the result from a cache here - just setting a value
                var cachedValue = ((string) args[0]).ToArray();
                Array.Reverse(cachedValue);

                result = new string(cachedValue).ToLowerInvariant();

                Console.WriteLine($"Before {targetMethod.Name}({accessKey}) - using a cache");
            }
            else
            {
                Console.WriteLine($"Before {targetMethod.Name}({accessKey}) - not using a cache");
            }

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