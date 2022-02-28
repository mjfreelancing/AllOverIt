using BenchmarkDotNet.Running;
using System;

namespace ObjectMappingBenchmarking
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<MappingTests>();

            //var tests = new MappingTests();
            //tests.StaticMethod_SimpleSource_Create_SimpleTarget();

            Console.WriteLine("");
            Console.WriteLine("All Over It.");
        }
    }
}
