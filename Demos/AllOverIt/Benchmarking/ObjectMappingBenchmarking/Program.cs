using AllOverIt.Reflection;
using BenchmarkDotNet.Running;
using System;
using AllOverIt.Mapping;

namespace ObjectMappingBenchmarking
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<MappingTests>();

            //var tests = new MappingTests();
            //tests.StaticMethod_SimpleSource_Create_SimpleTarget();
            //tests.ObjectMapper_SimpleSource_Create_SimpleTarget();
            //tests.ObjectMapper_SimpleSource_CopyTo_SimpleTarget();

            Console.WriteLine("");
            Console.WriteLine("All Over It.");
        }
    }
}
