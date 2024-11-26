using BenchmarkDotNet.Running;

namespace SpecificationBenchmark
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<BenchmarkTests>();
        }
    }
}
