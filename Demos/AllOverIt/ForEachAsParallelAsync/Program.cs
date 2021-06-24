using BenchmarkDotNet.Running;
using System;

namespace ForEachAsParallelAsync
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<AsyncCalculations>();

            Console.WriteLine("");
            Console.WriteLine("All Over It.");
        }
    }
}
