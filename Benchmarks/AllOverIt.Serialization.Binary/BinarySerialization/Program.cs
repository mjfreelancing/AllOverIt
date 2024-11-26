#if !DEBUG
using BenchmarkDotNet.Running;
#endif

namespace BinarySerializationBenchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            var benchmark = new BenchmarkTests();

            benchmark.Binary_Reader_Writer();
            benchmark.NewtonSoft();
            benchmark.SystemText();
#else
            BenchmarkRunner.Run<BenchmarkTests>();
#endif
        }
    }
}