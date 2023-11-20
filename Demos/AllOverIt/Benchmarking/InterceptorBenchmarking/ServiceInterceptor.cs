using AllOverIt.Aspects;

namespace InterceptorBenchmarking
{
    internal class ServiceInterceptor : InterceptorBase<IService>
    {
        // Not including BeforeInvoke(), AfterInvoke(), Faulted() to keep it as close as possible to the regular service calls
    }
}