using BenchmarkDotNet.Running;

namespace SpecificationBenchmarking
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<SpecificationTest>();

            //var test = new SpecificationTest();
            //test.Using_LinqSpecification_IsSatisfied_Twice();
        }
    }
}
