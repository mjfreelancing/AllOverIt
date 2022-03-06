using AllOverIt.Reflection;
using BenchmarkDotNet.Running;
using System;

namespace ObjectMappingBenchmarking
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var cache = new ObjectTestCache();


            //cache.GetOrAddInt(1, key => 10);
            //var v1 = cache.GetOrAddInt(1, key => 10);


            //cache.GetOrAddString("2", key => "20");
            //var v2 = cache.GetOrAddString("2", key => "20");


            //cache.GetOrAddObjectTest(1, "2", typeof(string), key =>
            //{
            //    return new ObjectTest
            //    {
            //        Prop1 = 20,
            //        Prop2 = "30",
            //        Prop3 = typeof(int)
            //    };
            //});

            //var v3 = cache.GetOrAddObjectTest(1, "2", typeof(string), key =>
            //{
            //    return new ObjectTest
            //    {
            //        Prop1 = 20,
            //        Prop2 = "30",
            //        Prop3 = typeof(int)
            //    };
            //});


            //cache.GetOrAddObjectTest(11, "22", typeof(int), key =>
            //{
            //    return new ObjectTest
            //    {
            //        Prop1 = 20,
            //        Prop2 = "30",
            //        Prop3 = typeof(int)
            //    };
            //});

            //var v4 = cache.GetOrAddObjectTest(1, "22", typeof(string), key =>
            //{
            //    return new ObjectTest
            //    {
            //        Prop1 = 200,
            //        Prop2 = "300",
            //        Prop3 = typeof(string)
            //    };
            //});


            //BenchmarkRunner.Run<MappingTests>();

            var tests = new MappingTests();
            //tests.StaticMethod_SimpleSource_Create_SimpleTarget();
            //tests.ObjectMapper_SimpleSource_Create_SimpleTarget();
            tests.ObjectMapper_SimpleSource_CopyTo_SimpleTarget();

            Console.WriteLine("");
            Console.WriteLine("All Over It.");
        }
    }
}
