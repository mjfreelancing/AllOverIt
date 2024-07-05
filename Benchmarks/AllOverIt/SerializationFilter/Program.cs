using BenchmarkDotNet.Running;

namespace SerializationFilterBenchmark
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<BenchmarkTests>();
        }
    }
}
