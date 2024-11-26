using BenchmarkDotNet.Running;

namespace CompiledReflectionBenchmark
{
    internal class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<BenchmarkTests>();
        }
    }
}