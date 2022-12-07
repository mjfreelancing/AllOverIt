﻿using AllOverIt.Extensions;
using AllOverIt.Formatters.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllOverIt.Formatters.Objects.Extensions;
using AllOverIt.Helpers.PropertyNavigation;
using AllOverIt.Helpers.PropertyNavigation.Extensions;
using System.Text;

namespace SerializeObjectProperties
{
    class Program
    {
        static async Task Main()
        {
            try
            {
                var serializer = new ObjectPropertySerializer();

                SerializeObject(false);

                Console.WriteLine();
                SerializeObject(true);

                Console.WriteLine();
                SerializeFilteredObject();

                Console.WriteLine();
                SerializeFilteredItemDataViaRegistry();

                Console.WriteLine();
                await SerializeTaskedFilteredItemDataViaRegistryAsync();

                Console.WriteLine();
                SerializeDictionary1();

                Console.WriteLine();
                SerializeDictionary2();

                Console.WriteLine();
                SerializeList();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static void SerializeObject(bool collateArrayValues)
        {
            var serializer = new ObjectPropertySerializer();

            var dummy1 = new Dummy();
            var dummy2 = new Dummy { Prop11 = dummy1 };
            // un-comment to test a self-referencing exception
            //dummy1.Prop11 = dummy2;

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
                        Prop11 = dummy1,
                        Task = Task.FromResult(true)            // will be excluded
                    }
                },
                Prop12 = new List<int> { -1, -2, -3 },
                Prop13 = new[]
                {
                    new Dummy
                    {
                        Prop7 = "1",
                        Prop10 = true,
                        Prop8 = 2,
                        Prop9 = 3.3,
                        Prop12 = new[] { 10, 20, 30 }
                    },
                    new Dummy
                    {
                        Prop7 = "2",
                        Prop10 = false,
                        Prop8 = 3,
                        Prop9 = 4.4,
                        Prop12 = new[] { 11, 22, 33 }
                    },
                    new Dummy
                    {
                        Prop7 = "3",
                        Prop10 = true,
                        Prop8 = 4,
                        Prop9 = 5.5,
                        Prop12 = new[] { 1, 2, 3 }
                    }
                }
            };

            var metadataRoot = new
            {
                Prop13 = "Root",
                Prop14 = metadataChild.ToPropertyDictionary(),  // of type IDictionary<string, object>
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
                Prop21 = (bool?)null,
                Prop22 = new TypedDummy<int>
                {
                    Value1 = 23,
                    Dummy = 1
                },
                Prop23 = new TypedDummy<string>
                {
                    Value2 = 9,
                    Dummy = "one"
                },
                Prop24 = new TypedDummy<Task>
                {
                    Value1 = 3,
                    Value2 = -3,
                    Dummy = Task.CompletedTask                  // will be excluded
                },
                Prop25 = (Action<int, int, bool>)((_, _, _) => { }),
                Prop26 = (Func<bool, bool>)(_ => true),
                Prop27 = new Dictionary<string, Task>(),        // will be excluded
                Prop28 = new Dictionary<int, string>(),
                Prop29 = new Dictionary<Task, string>(),         // will be excluded
                Prop30 = metadataChild
            };

            Console.WriteLine($"Object serialization values (collate array values = {collateArrayValues}):");
            Console.WriteLine("===========================================================");

            serializer.Options.EnumerableOptions.CollateValues = collateArrayValues;

            if (!collateArrayValues)
            {
                serializer.Options.EnumerableOptions.AutoCollatedPaths = new[]
                {
                        "Prop30.Prop12",
                        "Prop30.Prop13.Prop12"
                    };
            }

            var items = serializer.SerializeToDictionary(metadataRoot).Select(kvp => $"{kvp.Key} = {kvp.Value}");

            foreach (var item in items)
            {
                Console.WriteLine($"  {item}");
            }
        }

        private static void SerializeFilteredObject()
        {
            var filter = new ComplexObjectFilter();
            var serializer = new ObjectPropertySerializer(null, filter);

            var complexObject = new ComplexObject
            {
                Items = new ComplexObject.ComplexItem[]
                {
                    new()
                    {
                        Name = "Name 1",
                        Factor = 1.1,
                        Data = new ComplexObject.ComplexItem.ComplexItemData
                        {
                            Timestamp = DateTime.Now,
                            Values = Enumerable.Range(1, 5).SelectAsReadOnlyCollection(value => value)
                        }
                    },
                    new()
                    {
                        Name = "Name 2",
                        Factor = 2.2,
                        Data = new ComplexObject.ComplexItem.ComplexItemData
                        {
                            Timestamp = DateTime.Now,
                            Values = Enumerable.Range(11, 5).SelectAsReadOnlyCollection(value => value)
                        }
                    },
                    new()
                    {
                        Name = "Name 3",
                        Factor = 3.3,
                        Data = new ComplexObject.ComplexItem.ComplexItemData
                        {
                            Timestamp = DateTime.Now,
                            Values = Enumerable.Range(21, 5).SelectAsReadOnlyCollection(value => value)
                        }
                    },
                }
            };

            Console.WriteLine("Filtered Object serialization values:");
            Console.WriteLine("====================================");

            var items = serializer.SerializeToDictionary(complexObject).Select(kvp => $"{kvp.Key} = {kvp.Value}");

            foreach (var item in items)
            {
                Console.WriteLine($"  {item}");
            }
        }

        private static void SerializeFilteredItemDataViaRegistry()
        {
            var complexObject = new ComplexObject
            {
                Items = new ComplexObject.ComplexItem[]
                {
                    new()
                    {
                        Name = "Name 1",
                        Factor = 1.1,
                        Data = new ComplexObject.ComplexItem.ComplexItemData
                        {
                            Timestamp = DateTime.Now,
                            Values = Enumerable.Range(1, 5).SelectAsReadOnlyCollection(value => value)
                        }
                    },
                    new()
                    {
                        Name = "Name 2",
                        Factor = 2.2,
                        Data = new ComplexObject.ComplexItem.ComplexItemData
                        {
                            Timestamp = DateTime.Now,
                            Values = Enumerable.Range(11, 5).SelectAsReadOnlyCollection(value => value)
                        }
                    },
                    new()
                    {
                        Name = "Name 3",
                        Factor = 3.3,
                        Data = new ComplexObject.ComplexItem.ComplexItemData
                        {
                            Timestamp = DateTime.Now,
                            Values = Enumerable.Range(21, 5).SelectAsReadOnlyCollection(value => value)
                        }
                    },
                }
            };

            var options = new ObjectPropertySerializerOptions
            {
                // Approach #1
                // See below for a type-safe way that also checks the leaf node is not a class type
                //
                // EnumerableOptions =
                // {
                //     AutoCollatedPaths = new[] {"Items.Data.Values"}
                // }
            };

            // Approach #2
            var propertyNodes = PropertyNavigator
                .For<ComplexObject>()
                .Navigate(item => item.Items)           // each call to Navigate can only go as far as an IEnumerable<T>
                .Navigate(item => item.Data.Values);

            // This would throw since item.Items is an enumerable class type
            // var propertyNodes = PropertyNavigator.For<ComplexObject>().Navigate(item => item.Items);

            options.EnumerableOptions.SetAutoCollatedPaths(propertyNodes);

            var registry = new ObjectPropertyFilterRegistry();
            registry.Register<ComplexObject, ComplexObjectItemDataFilter>(options);

            Console.WriteLine("Complex Object serialization values via a registry:");
            Console.WriteLine("===================================================");

            _ = registry.GetObjectPropertySerializer(complexObject, out var serializer);

            var items = serializer.SerializeToDictionary(complexObject).Select(kvp => $"{kvp.Key} = {kvp.Value}");

            foreach (var item in items)
            {
                Console.WriteLine($"  {item}");
            }

            Console.WriteLine();
            Console.WriteLine("  >> And do it again to ensure we can request the same filter type");
            Console.WriteLine();

            _ = registry.GetObjectPropertySerializer(complexObject, out serializer);

            items = serializer.SerializeToDictionary(complexObject).Select(kvp => $"{kvp.Key} = {kvp.Value}");

            foreach (var item in items)
            {
                Console.WriteLine($"  {item}");
            }
        }

        private static async Task SerializeTaskedFilteredItemDataViaRegistryAsync()
        {
            var registry = new ObjectPropertyFilterRegistry();

            var options = new ObjectPropertySerializerOptions();

            // See SerializeFilteredItemDataViaRegistry() for a type-safe way that also checks the leaf node is not a class type
            options.EnumerableOptions.AutoCollatedPaths = new[] { "Items.Data.Values" };

            // Options can be safely shared
            registry.Register<ComplexObject, ComplexObjectItemDataFilter>(options);

            var tasks = Enumerable
                .Range(0, 10)
                .Select(value =>
                {
                    return Task.Factory.StartNew<StringBuilder>(() =>
                    {
                        var sb = new StringBuilder();

                        var complexObject = new ComplexObject
                        {
                            Items = new ComplexObject.ComplexItem[]
                            {
                                new()
                                {
                                    Name = "Name 1",
                                    Factor = value + 1.1,
                                    Data = new ComplexObject.ComplexItem.ComplexItemData
                                    {
                                        Timestamp = DateTime.Now,
                                        Values = Enumerable.Range(1, 5).SelectAsReadOnlyCollection(value => value)
                                    }
                                },
                                new()
                                {
                                    Name = "Name 2",
                                    Factor = value + 2.2,
                                    Data = new ComplexObject.ComplexItem.ComplexItemData
                                    {
                                        Timestamp = DateTime.Now,
                                        Values = Enumerable.Range(11, 5).SelectAsReadOnlyCollection(value => value)
                                    }
                                },
                                new()
                                {
                                    Name = "Name 3",
                                    Factor = value + 3.3,
                                    Data = new ComplexObject.ComplexItem.ComplexItemData
                                    {
                                        Timestamp = DateTime.Now,
                                        Values = Enumerable.Range(21, 5).SelectAsReadOnlyCollection(value => value)
                                    }
                                },
                            }
                        };

                        _ = registry.GetObjectPropertySerializer(complexObject, out var serializer);

                        var items = serializer.SerializeToDictionary(complexObject).SelectAsReadOnlyCollection(kvp => $"{kvp.Key} = {kvp.Value}");

                        foreach (var item in items)
                        {
                            sb.AppendLine($"  {item}");
                        }

                        return sb;
                    });
                });

            var stringBuilders = await Task.WhenAll(tasks);

            foreach (var sb in stringBuilders)
            {
                Console.WriteLine("Complex Object serialization values via a registry (in a Task):");
                Console.WriteLine("===============================================================");

                Console.WriteLine(sb.ToString());
                Console.WriteLine();
            }
        }

        private static void SerializeDictionary1()
        {
            var serializer = new ObjectPropertySerializer();

            var dictionary = new Dictionary<string, int>
            {
                { "one", 1 },
                { "two", 2 },
                { "three", 3 },
                { "four", 4 }
            };

            Console.WriteLine("Dictionary #1 serialization values:");
            Console.WriteLine("===================================");

            var items = serializer.SerializeToDictionary(dictionary).Select(kvp => $"{kvp.Key} = {kvp.Value}");

            foreach (var item in items)
            {
                Console.WriteLine($"  {item}");
            }
        }

        private static void SerializeDictionary2()
        {
            var serializer = new ObjectPropertySerializer();

            var dictionary = new Dictionary<TypedDummy<bool>, int>
            {
                // only the class name is serialized because it is a key within the dictionary
                { new TypedDummy<bool>{Dummy = true}, 1 },
                { new TypedDummy<bool>{Dummy = false}, 2 },
                { new TypedDummy<bool>{Dummy = false}, 3 },
                { new TypedDummy<bool>{Dummy = true}, 4 },
            };

            Console.WriteLine("Dictionary #2 serialization values:");
            Console.WriteLine("===================================");

            var items = serializer.SerializeToDictionary(dictionary).Select(kvp => $"{kvp.Key} = {kvp.Value}");

            foreach (var item in items)
            {
                Console.WriteLine($"  {item}");
            }
        }

        private static void SerializeList()
        {
            var serializer = new ObjectPropertySerializer();

            var list = Enumerable.Range(1, 4).Select(value => $"Value {value}").ToList();

            Console.WriteLine("List serialization values:");
            Console.WriteLine("==========================");

            var items = serializer.SerializeToDictionary(list).Select(kvp => $"{kvp.Key} = {kvp.Value}");

            foreach (var item in items)
            {
                Console.WriteLine($"  {item}");
            }
        }
    }
}
