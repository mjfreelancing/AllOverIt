using BenchmarkDotNet.Running;

namespace ForEachAsyncBenchmarking
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<AsyncCalculations>();
        }
    }
}
