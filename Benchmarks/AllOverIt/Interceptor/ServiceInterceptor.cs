using AllOverIt.Aspects;

namespace InterceptorBenchmark
{
    internal class ServiceInterceptor : InterceptorBase<IService>
    {
        // Not including BeforeInvoke(), AfterInvoke(), Faulted() to keep it as close as possible to the regular service calls
    }
}