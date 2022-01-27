using AllOverIt.Csv;
using AllOverIt.Csv.Extensions;
using CsvExport.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;

namespace CsvExport
{
    internal class Program
    {      
        static async Task Main(string[] args)
        {
            var sampleData = CreateSampledata();

            var serializer = new DataSerializer<SampleData>();

            ConfigureSerializer(serializer, sampleData);

            // Replace StringWriter() with StreamWriter("filename.csv") to write to a file instead
            using (var writer = new StringWriter())
            {
                await serializer.SerializeAsync(writer, sampleData);

                Console.WriteLine(writer.ToString());
            }

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static IReadOnlyCollection<SampleData> CreateSampledata()
        {
            return new List<SampleData>
            {
                new SampleData
                {
                    Name = "Sample 1",
                    Count = 0,
                    Values = new Dictionary<string, int>(),
                    Coordinates = new List<Coordinates>()
                },

                new SampleData
                {
                    Name = "Sample 2",
                    Count = 1,
                    Values = new Dictionary<string, int>
                    {
                        { "Value 1", 1 }
                    },
                    Coordinates = new List<Coordinates>
                    {
                        new Coordinates(100.1, 120.2)
                    }
                },

                new SampleData
                {
                    Name = "Sample 3",
                    Count = 3,
                    Values = new Dictionary<string, int>
                    {
                        { "Value 1", 1 },
                        { "Value 2", 2 },
                        { "Value 3", 3 }
                    },
                    Coordinates = new List<Coordinates>
                    {
                        new Coordinates(100.4, 119.8),
                        new Coordinates(100.7, 120.0),
                        new Coordinates(100.3, 119.2),
                    }
                },

                new SampleData
                {
                    Name = "Sample 4",
                    Count = 2,
                    Values = new Dictionary<string, int>
                    {
                        { "Value 1", 1 },
                        { "Value 2", 2 }
                    },
                    Coordinates = new List<Coordinates>
                    {
                        new Coordinates(100.1, 121.3),
                        new Coordinates(100.8, 120.5),
                    }
                }
            };
        }

        private static void ConfigureSerializer(IDataSerializer<SampleData> serializer, IReadOnlyCollection<SampleData> sampleData)
        {
            // Add fixed, known, columns
            serializer.AddField(nameof(SampleData.Name), item => item.Name);
            serializer.AddField(nameof(SampleData.Count), item => item.Count);

            serializer.AddDynamicFields(
                sampleData,                                                             // The source data to be processed
                item => item.Values,                                                    // The property to be export across one or more columns
                item => item.Keys,                                                      // Anything that uniquely identifies each row - this will be the name in the next Func
                (item, name) => item.TryGetValue(name, out var value) ? value : null    // Get the value to be exported for the column with the provided name
            );
               
            serializer.AddDynamicFields(
                sampleData,
                item => item.Coordinates,
                item => Enumerable
                            .Range(0, item.Count * 2)       // *2 because we need to export the latitude and longitude
                            .Select(idx => 
                            {
                                var itemOrdinal = (int)Math.Floor(idx / 2.0d) + 1;

                                var name = idx % 2 == 0
                                    ? $"Latitude {itemOrdinal}"
                                    : $"Longitude {itemOrdinal}";

                                // The int generic means the 'Id' can be used to identify the index being processed
                                return new HeaderIdentifier<int>
                                {
                                    Id = idx,
                                    Name = name
                                };
                            }),        // using the index to identify the header
                (item, headerId) =>
                {
                    if (headerId.Id < item.Count * 2)
                    {
                        var itemOrdinal = (int) Math.Floor(headerId.Id / 2.0d);
                        var coordinates = item.ElementAt(itemOrdinal);

                        return headerId.Id % 2 == 0
                            ? $"{coordinates.Latitude}"
                            : $"{coordinates.Longitude}";
                    }

                    return null;
                });
        }
    }
}
