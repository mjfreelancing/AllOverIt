#define BENCHMARK

#if BENCHMARK
using BenchmarkDotNet.Running;
#endif

namespace ObjectMappingBenchmarking
{
    internal class Program
    {
        static void Main(string[] args)
        {
#if BENCHMARK
            BenchmarkRunner.Run<MappingTests>();
#else
            var tests = new MappingTests();
            tests.StaticMethod_SimpleSource_Create_SimpleTarget();
            tests.ObjectMapper_SimpleSource_Create_SimpleTarget();
            tests.ObjectMapper_SimpleSource_CopyTo_SimpleTarget();
#endif
        }
    }
}
