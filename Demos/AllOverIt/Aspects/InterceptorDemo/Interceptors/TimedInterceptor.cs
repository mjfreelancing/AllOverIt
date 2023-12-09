using AllOverIt.Aspects;
using AllOverIt.Extensions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace InterceptorDemo.Interceptors
{
    // Note: Interceptors cannot be sealed as they are the base class for the generated proxy.
    internal class TimedInterceptor : InterceptorBase<ISecretService>
    {
        private static readonly MethodInfo[] FilteredMethods = [typeof(ISecretService).GetMethod(nameof(ISecretService.GetSecretId))];

        public long? MinimimReportableMilliseconds { get; set; }

        private sealed class TimedState : InterceptorState
        {
            public Stopwatch Stopwatch { get; } = Stopwatch.StartNew();
        }

        protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref object result)
        {
            if (FilteredMethods.Contains(targetMethod))
            {
                return InterceptorState.None;
            }

            var accessKey = (string) args[0];

            args[0] = accessKey.ToUpperInvariant();

            Console.WriteLine($"Before {targetMethod.Name}({accessKey})");

            // Can return InterceptorState.None if no state is required
            return new TimedState();
        }

        protected override void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, ref object result)
        {
            if (FilteredMethods.Contains(targetMethod))
            {
                return;
            }

            var accessKey = (string) args[0];

            var taskResult = result as Task<string>;

            // Cater for GetSecret() and GetSecretAsync()
            var value = taskResult is not null
                   ? taskResult.Result
                   : (string) result;

            Console.WriteLine($"After {targetMethod.Name}({accessKey}), result = {value}");

            value = value.ToLowerInvariant();

            // Cater for GetSecret() and GetSecretAsync()
            result = taskResult is not null
                ? Task.FromResult(value)
                : value;

            Console.WriteLine($"  => Result modified to {value}");

            CheckElapsedPeriod(state);
        }

        protected override void Faulted(MethodInfo targetMethod, object[] args, InterceptorState state, Exception exception)
        {
            var accessKey = (string) args[0];

            Console.WriteLine($"FAULTED {targetMethod.Name}({accessKey}) : {exception.GetType().GetFriendlyName()} - {exception.Message}");

            CheckElapsedPeriod(state);
        }

        private void CheckElapsedPeriod(InterceptorState state)
        {
            var timedState = state as TimedState;
            var elapsed = timedState.Stopwatch.ElapsedMilliseconds;

            if (!MinimimReportableMilliseconds.HasValue || elapsed >= MinimimReportableMilliseconds)
            {
                if (MinimimReportableMilliseconds.HasValue)
                {
                    Console.WriteLine($" >> WARNING: Elapsed exceeded minimum {MinimimReportableMilliseconds.Value}ms, actual = {elapsed}ms");
                }
                else
                {
                    Console.WriteLine($" >> Elapsed = {elapsed}ms");
                }
            }
        }
    }
}