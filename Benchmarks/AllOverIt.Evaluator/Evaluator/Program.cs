using BenchmarkDotNet.Running;

namespace EvaluatorBenchmark
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<EvaluatorBenchmarks>();
            //BenchmarkRunner.Run<LookupBenchmarks>();
        }
    }
}
