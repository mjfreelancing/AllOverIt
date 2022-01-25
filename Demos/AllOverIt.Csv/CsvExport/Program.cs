using AllOverIt.Csv;
using CsvExport.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
                await serializer.Serialize(writer, sampleData);

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
            serializer.AddFixedField(nameof(SampleData.Name), item => item.Name);
            serializer.AddFixedField(nameof(SampleData.Count), item => item.Count);

            serializer.AddDynamicFields(
                sampleData,                         // The source data to be processed
                item => item.Values,                // The property to be export across one or more columns
                item => item.Keys,                  // Anything that uniquely identifies each row - this will be the name in the next Func
                (item, name) =>
                {
                    // Get the value to be exported for the column with the provided name
                    return item.TryGetValue(name, out var value) ? value : (int?) null;
                });

            serializer.AddDynamicFields(
                sampleData,
                item => item.Coordinates,
                item => Enumerable.Range(0, item.Count).Select(idx => (idx, $"Coordinate {idx + 1}")),        // using the index to identify the header
                (item, headerId) =>
                {
                    if (headerId.Item1 < item.Count)
                    {
                        var coordinates = item.ElementAt(headerId.Item1);

                        return $"({coordinates.Latitude} {coordinates.Longitude})";
                    }

                    return null;
                });
        }
    }
}
