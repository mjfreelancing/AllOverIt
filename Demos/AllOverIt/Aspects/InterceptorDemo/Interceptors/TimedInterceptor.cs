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

        protected override InterceptorState BeforeInvoke(MethodInfo targetMethod, object[] args, ref object result)
        {
            Console.WriteLine($"Before {targetMethod.Name}()");

            // Can return InterceptorState.None if no state is required
            return new TimedState();
        }

        protected override void AfterInvoke(MethodInfo targetMethod, object[] args, InterceptorState state, ref object result)
        {
            Console.WriteLine($"After {targetMethod.Name}()");

            CheckElapsedPeriod(state);
        }

        protected override void Faulted(MethodInfo targetMethod, object[] args, InterceptorState state, Exception exception)
        {
            Console.WriteLine($"FAULTED {targetMethod.Name}() : {exception.GetType().GetFriendlyName()} - {exception.Message}");

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