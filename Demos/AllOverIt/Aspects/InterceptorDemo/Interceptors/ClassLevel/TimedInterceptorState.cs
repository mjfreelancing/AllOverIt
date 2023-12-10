using AllOverIt.Aspects;
using System.Diagnostics;

namespace InterceptorDemo.Interceptors.ClassLevel
{
    // Only using this with methods that return a string or Task<string> (not a good practive, just for testing and demo)
    internal sealed class TimedInterceptorState<TResult> : InterceptorState<TResult>
    {
        public Stopwatch Stopwatch { get; } = Stopwatch.StartNew();
    }
}