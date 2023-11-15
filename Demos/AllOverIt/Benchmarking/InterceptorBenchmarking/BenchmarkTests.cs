using AllOverIt.Aspects.Interceptor;
using BenchmarkDotNet.Attributes;
using System.Threading.Tasks;

namespace InterceptorBenchmarking
{
    /*
    | Method                          | Mean          | Error       | StdDev      | Gen0   | Allocated |
    |-------------------------------- |--------------:|------------:|------------:|-------:|----------:|
    | Call_Service_GetSecret          |      2.063 ns |   0.0700 ns |   0.0749 ns |      - |         - |
    | Call_Service_GetSecretAsync     |     17.383 ns |   0.2721 ns |   0.2272 ns | 0.0172 |      72 B |
    | Call_Interceptor_GetSecret      |     31.480 ns |   0.3878 ns |   0.3628 ns | 0.0114 |      48 B |
    | Call_Interceptor_GetSecretAsync | 15,725.459 ns | 264.3216 ns | 247.2466 ns | 2.1057 |    8890 B |
     */

    [MemoryDiagnoser]
    public class BenchmarkTests
    {
        private readonly Service _service = new();
        private readonly IService _serviceInterceptor = InterceptorFactory.CreateInterceptor<IService, ServiceInterceptor>(new Service(), null, (_, interceptor) => { });

        [Benchmark]
        public void Call_Service_GetSecret()
        {
            _service.GetSecret();
        }

        [Benchmark]
        public async Task Call_Service_GetSecretAsync()
        {
            await _service.GetSecretAsync(true);
        }

        [Benchmark]
        public void Call_Interceptor_GetSecret()
        {
            _serviceInterceptor.GetSecret();
        }

        [Benchmark]
        public async Task Call_Interceptor_GetSecretAsync()
        {
            await _serviceInterceptor.GetSecretAsync(true);
        }
    }
}