using BenchmarkDotNet.Running;

namespace ForEachAsyncBenchmark
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<BenchmarkTests>();
        }
    }
}
