using AllOverIt.Extensions;
using AllOverIt.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SerializeObjectProperties
{
    class Program
    {
        static void Main(string[] args)
        {
            var serializer = new ObjectPropertySerializationHelper { IncludeEmptyCollections = true, IncludeNulls = true };

            SerializeObject(serializer);

            Console.WriteLine();
            SerializeDictionary(serializer);

            Console.WriteLine();
            SerializeList(serializer);

            Console.WriteLine("");
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static void SerializeObject(ObjectPropertySerializationHelper serializer)
        {
            var metadataChild = new
            {
                Prop1 = 1,
                Prop2 = false,
                Prop3 = new
                {
                    Prop4 = "Hello",
                    Prop5 = new List<int>
                    {
                        1, 2, 3, 4
                    },
                    Prop6 = new Dummy
                    {
                        Prop7 = "World",
                        Prop8 = 11,
                        Prop9 = 2.3d,
                        Prop10 = true,
                        Prop11 = new Dummy
                        {
                            //
                        }
                    }
                },
                Prop12 = new List<int> { -1, -2, -3 }
            };

            var metadataRoot = new
            {
                Prop13 = "Root",
                Prop14 = metadataChild.ToPropertyDictionary(), // of type IDictionary<string, object>
                Prop15 = new Dictionary<int, string>
                {
                    { 1, "one" }, { 2, "two" }, { 3, "three" }
                },
                Prop16 = new Dictionary<Dummy, int>
                {
                    { new Dummy(), 1 },
                    { new Dummy(), 2 }
                },
                Prop17 = new List<int>(),
                Prop18 = new Dictionary<string, object>
                {
                    {"Some Key", new Dictionary<int, string>
                    {
                        { 1, "one" }, { 2, "two" }, { 3, "three" }
                    }}
                },
                Prop19 = new List<Action<int>>
                {
                    _ => { }
                },
                Prop20 = new List<Func<int, string, int>>
                {
                    (_, _) => 1
                },
                prop21 = (bool?)null
            };

            Console.WriteLine("Object serialization values:");
            Console.WriteLine("============================");

            var items = serializer.SerializeToDictionary(metadataRoot).Select(kvp => $"{kvp.Key} = {kvp.Value}");

            foreach (var item in items)
            {
                Console.WriteLine($"  {item}");
            }

        }

        private static void SerializeDictionary(ObjectPropertySerializationHelper serializer)
        {
            var dictionary = new Dictionary<string, int>
            {
                { "one", 1 },
                { "two", 2 },
                { "three", 3 },
                { "four", 4 }
            };

            Console.WriteLine("Dictionary serialization values:");
            Console.WriteLine("================================");

            var items2 = serializer.SerializeToDictionary(dictionary).Select(kvp => $"{kvp.Key} = {kvp.Value}");
            foreach (var item in items2)
            {
                Console.WriteLine($"  {item}");
            }
        }

        private static void SerializeList(ObjectPropertySerializationHelper serializer)
        {
            var list = Enumerable.Range(1, 4).Select(value => $"Value {value}").ToList();

            Console.WriteLine("List serialization values:");
            Console.WriteLine("==========================");

            var items3 = serializer.SerializeToDictionary(list).Select(kvp => $"{kvp.Key} = {kvp.Value}");
            foreach (var item in items3)
            {
                Console.WriteLine($"  {item}");
            }
        }
    }
}
