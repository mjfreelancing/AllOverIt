using BenchmarkDotNet.Running;

namespace CompiledReflectionBenchmarking
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<CompiledReflection>();
        }
    }
}