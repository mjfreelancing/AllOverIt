using BenchmarkDotNet.Running;
using System;

namespace ObjectMappingBenchmarking
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<MappingTests>();

            Console.WriteLine("");
            Console.WriteLine("All Over It.");
        }
    }
}
