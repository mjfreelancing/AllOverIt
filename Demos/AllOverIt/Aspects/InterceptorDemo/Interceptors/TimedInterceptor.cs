using AllOverIt.Aspects.Interceptor;
using AllOverIt.Extensions;
using System;
using System.Diagnostics;
using System.Reflection;

namespace InterceptorDemo.Interceptors
{
    // Note: Interceptors cannot be sealed as they are the base class for the generated proxy.
    internal class TimedInterceptor : InterceptorBase<ISecretService>
    {
        public long? MinimimReportableMilliseconds { get; set; }

        private class TimedState : InterceptorState
        {
            public Stopwatch Stopwatch { get; } = Stopwatch.StartNew();
        }

        protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args, ref object result)
        {
            var accessKey = (string) args[0];

            args[0] = accessKey.ToUpperInvariant();

            Console.WriteLine($"Before {targetMethod.Name}({accessKey})");

            // Can return InterceptorState.None if no state is required
            return new TimedState();
        }

        protected override void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, ref object result)
        {
            var accessKey = (string) args[0];

            Console.WriteLine($"After {targetMethod.Name}({accessKey})");

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