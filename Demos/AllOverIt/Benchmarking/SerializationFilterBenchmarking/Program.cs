using BenchmarkDotNet.Running;

namespace SerializationFilterBenchmarking
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<ObjectFilterTest>();
        }
    }
}
