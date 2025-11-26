using BenchmarkDotNet.Running;

namespace ThrowWhenBenchmark
{
    internal class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<ThrowWhenBenchmark>();
        }
    }
}