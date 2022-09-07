using BenchmarkDotNet.Running;

namespace SpecificationBenchmarking
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<BenchmarkTests>();
        }
    }
}
