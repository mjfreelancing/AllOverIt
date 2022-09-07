using BenchmarkDotNet.Running;

namespace ForEachAsyncBenchmarking
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<BenchmarkTests>();
        }
    }
}
