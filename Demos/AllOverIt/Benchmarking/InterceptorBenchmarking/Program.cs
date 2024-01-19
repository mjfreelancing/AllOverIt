using BenchmarkDotNet.Running;

namespace InterceptorBenchmarking
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<BenchmarkTests>();
        }
    }
}