using AllOverIt.Aspects;
using System;
using System.Diagnostics;
using System.Reflection;

namespace InterceptorDemo.Interceptors.ClassLevel
{
    // Note: Interceptors cannot be sealed as they are the base class for the generated proxy.
    // Time all method executions
    internal class TimeAllMethodExecutionsInterceptor : InterceptorBase<ISecretService>
    {
        public long? MinimimReportableMilliseconds { get; set; }

        private sealed class TimedState : InterceptorState
        {
            public Stopwatch Stopwatch { get; } = Stopwatch.StartNew();
        }

        protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, ref object[] args)
        {
            Console.WriteLine($"Before {targetMethod.Name}");

            return new TimedState();
        }

        protected override void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state)
        {
            Console.WriteLine($"After {targetMethod.Name}");

            CheckElapsedPeriod(state);
        }

        protected override void Faulted(MethodInfo targetMethod, object[] args, InterceptorState state, Exception exception)
        {
            Console.WriteLine($"FAULTED: {targetMethod.Name} - {exception.Message}");

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
                    Console.WriteLine($" >> WARNING: Minimum {MinimimReportableMilliseconds.Value}ms, actual = {elapsed}ms");
                }
                else
                {
                    Console.WriteLine($" >> Elapsed = {elapsed}ms");
                }
            }
        }
    }
}