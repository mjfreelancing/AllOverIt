using AllOverIt.Aspects.Interceptor;
using System.Diagnostics;

namespace InterceptorDemo.Interceptors
{
    internal sealed class TimedInterceptorState : InterceptorState
    {
        public Stopwatch Stopwatch { get; } = Stopwatch.StartNew();
    }
}