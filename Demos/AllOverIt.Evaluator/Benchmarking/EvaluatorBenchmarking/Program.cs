using BenchmarkDotNet.Running;

namespace EvaluatorBenchmarking
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
