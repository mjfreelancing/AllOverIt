#define BENCHMARK

#if BENCHMARK
using BenchmarkDotNet.Running;
#endif

namespace ObjectMappingBenchmarking
{
    internal class Program
    {
        static void Main()
        {
#if BENCHMARK
            BenchmarkRunner.Run<MappingTests>();
#else
            var tests = new MappingTests();
            tests.ObjectMapper_PreConfigured_CopyTo_Target();
#endif
        }
    }
}
