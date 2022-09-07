using BenchmarkDotNet.Running;

namespace CompiledReflectionBenchmarking
{
    internal class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<BenchmarkTests>();
        }
    }
}