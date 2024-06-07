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

            tests.Existing_Mapper_Configured_New_Target_New();
#endif
        }
    }
}
