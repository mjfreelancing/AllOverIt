using BenchmarkDotNet.Running;

namespace InterceptorBenchmark
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<BenchmarkTests>();
        }
    }
}