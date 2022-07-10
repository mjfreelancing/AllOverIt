using BenchmarkDotNet.Running;

namespace ThrowWhenBenchmarking
{
    internal class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<ThrowWhenBenchmark>();
        }
    }
}