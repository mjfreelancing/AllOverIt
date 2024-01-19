using AllOverIt.Caching;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
using GenericCacheDemo.Keys;
using GenericCacheDemo.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GenericCacheDemo
{
    public static class Program
    {
        static async Task Main()
        {
            await PrePopulateAndReadAsync();
            Console.WriteLine();

            await PopulateAndReadInParallelAsync();
            Console.WriteLine();

            PopulateUsingDifferentKeyTypes();
            Console.WriteLine();

            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static async Task PrePopulateAndReadAsync()
        {
            var cache = GenericCache.Default;

            Console.WriteLine("1 - Populate then read values");

            await Enumerable
                .Range(1, 10)
                .ForEachAsTaskAsync((value, cancellationToken) =>
                    {
                        var key = new IntCacheKey(value);
                        cache.TryAdd(key, value * value);

                        return Task.CompletedTask;
                    },
                    Environment.ProcessorCount);

            await Enumerable
                .Range(1, 10)
                .ForEachAsTaskAsync((value, cancellationToken) =>
                    {
                        var key = new IntCacheKey(value);

                        // Won't ever fail since reading is thread safe and all values are already populated
                        var result = cache[key];
                        Console.WriteLine($"  : {value} * {value} = {result}");

                        return Task.CompletedTask;
                    },
                    Environment.ProcessorCount);

            cache.Clear();
        }

        private static async Task PopulateAndReadInParallelAsync()
        {
            const int count = 1000;

            var cache = GenericCache.Default;
            var failedReads = new ConcurrentQueue<int>();

            Console.WriteLine("2 - Populate and read in parallel (aiming for some failures)");

            var readData = Enumerable
                .Range(1, count)
                .ForEachAsTaskAsync((value, cancellationToken) =>
                    {
                        var key = new IntCacheKey(value);

                        if (!cache.TryGetValue(key, out int _))
                        {
                            failedReads.Enqueue(value);
                        }

                        return Task.CompletedTask;
                    },
                    Environment.ProcessorCount);

            var writeData = Enumerable
                .Range(1, count)
                .ForEachAsTaskAsync((value, cancellationToken) =>
                    {
                        var key = new IntCacheKey(value);
                        cache.TryAdd(key, value * value);

                        return Task.CompletedTask;
                    },
                    Environment.ProcessorCount);

            await Task.WhenAll(writeData, readData);

            if (failedReads.IsEmpty)
            {
                Console.WriteLine($"    All data for values 1 - {count} was read without fail");
            }
            else
            {
                var values = failedReads.Select(item => item);
                Console.WriteLine($"    Failed to read values: {string.Join(", ", values)}");
            }

            cache.Clear();
        }

        private static void PopulateUsingDifferentKeyTypes()
        {
            var cache = GenericCache.Default;

            // Add PropertyInfo based on BindingOptions
            var binding1 = BindingOptions.Instance | BindingOptions.Internal | BindingOptions.Static;
            var byBindingKey1 = new PropKeyByBinding(binding1);
            var byBindingResult = typeof(InfoModel)
                .GetPropertyInfo(binding1)
                .AsReadOnlyCollection();

            cache.Add(byBindingKey1, byBindingResult);

            // Add PropertyInfo based on BindingOptions and a Name
            var binding2 = BindingOptions.All;
            var name = nameof(InfoModel.Value4);
            var byBindingKey2 = new PropKeyByBindingAndName(binding2, name);

            var byBindingAndNameResult = typeof(InfoModel)
                .GetPropertyInfo(binding2)
                .SingleOrDefault(prop => prop.Name == name);

            cache.Add(byBindingKey2, byBindingAndNameResult);

            // Read the data back - using index operator because we know the data exists - would otherwise use TryGetValue() or GetOrAdd().
            // Casting is safe when you know what to expected based on the key type.
            var result1 = (IReadOnlyCollection<PropertyInfo>) cache[byBindingKey1];
            OutputPropertyInfo($"Result via key type '{nameof(PropKeyByBinding)}'", result1);

            Console.WriteLine();

            var result2 = (PropertyInfo) cache[byBindingKey2];
            OutputPropertyInfo($"Result via key type '{nameof(PropKeyByBindingAndName)}'", [result2]);

            cache.Clear();
        }

        private static void OutputPropertyInfo(string caption, IEnumerable<PropertyInfo> propInfo)
        {
            Console.WriteLine(caption);

            foreach (var prop in propInfo)
            {
                Console.WriteLine($"  {prop.Name} is of type {prop.PropertyType.GetFriendlyName()}");
            }
        }
    }
}
