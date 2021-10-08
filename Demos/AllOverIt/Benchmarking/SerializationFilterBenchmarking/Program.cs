using BenchmarkDotNet.Running;

namespace SerializationFilterBenchmarking
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<ObjectFilterTest>();
        }
    }
}
