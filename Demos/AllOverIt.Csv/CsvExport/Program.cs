using AllOverIt.Csv;
using AllOverIt.Csv.Extensions;
using CsvExport.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CsvExport
{
    /*
        Will be exported as:

        Name     | Count | Value 1 | Value 2 | Value 3 | Latitude 1 | Longitude 1 | Latitude 2 | Longitude 2 | Latitude 3 | Longitude 3 | Environment-Temperature | Quality-Colour | Quality-Clarity | Environment-pH
        Sample 1 | 0     |         |         |         |            |             |            |             |            |             |                         |                |                 | 
        Sample 2 | 1     | 1       |         |         | 100.1      | 120.2       |            |             |                          | 30                      | 8              | 3               | 7
        Sample 3 | 3     | 1       | 2       | 3       | 100.4      | 119.8       | 100.7      | 120         | 100.3       119.2        | 28                      |                |                 | 6.9
        Sample 4 | 2     | 1       | 2       |         | 100.1      | 121.3       | 100.8      | 120.5       |                          |                         | 9              | 2               | 7.1
     */

    internal class Program
    {      
        static async Task Main(string[] args)
        {
            var sampleData = CreateSampleData();

            var serializer = new DataSerializer<SampleData>();

            ConfigureSerializer(serializer, sampleData);

            // Write to the console
            using (var writer = new StringWriter())
            {
                await serializer.SerializeAsync(writer, sampleData);

                Console.WriteLine(writer.ToString());
            }

            // And write to a file
            using (var writer = new StreamWriter("data-export.csv"))
            {
                await serializer.SerializeAsync(writer, sampleData);
            }

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static IReadOnlyCollection<SampleData> CreateSampleData()
        {
            return new List<SampleData>
            {
                new SampleData
                {
                    Name = "Sample 1",
                    Count = 0,
                    Values = new Dictionary<string, int>(),
                    Coordinates = new List<Coordinates>(),
                    Metadata = new List<SampleMetadata>()
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
                    },
                    Metadata = new List<SampleMetadata>
                    {
                        new SampleMetadata
                        {
                            Type = MetadataType.Environment,
                            Name = "Temperature",
                            Value = "30"
                        },
                        new SampleMetadata
                        {
                            Type = MetadataType.Quality,
                            Name = "Colour",
                            Value = "8"
                        },
                        new SampleMetadata
                        {
                            Type = MetadataType.Quality,
                            Name = "Clarity",
                            Value = "3"
                        },
                        new SampleMetadata
                        {
                            Type = MetadataType.Environment,
                            Name = "pH",
                            Value = "7.0"
                        }
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
                    },
                    Metadata = new List<SampleMetadata>
                    {
                        new SampleMetadata
                        {
                            Type = MetadataType.Environment,
                            Name = "Temperature",
                            Value = "28"
                        },
                        new SampleMetadata
                        {
                            Type = MetadataType.Environment,
                            Name = "pH",
                            Value = "6.9"
                        }
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
                    },
                    Metadata = new List<SampleMetadata>
                    {
                        new SampleMetadata
                        {
                            Type = MetadataType.Quality,
                            Name = "Colour",
                            Value = "9"
                        },
                        new SampleMetadata
                        {
                            Type = MetadataType.Quality,
                            Name = "Clarity",
                            Value = "2"
                        },
                        new SampleMetadata
                        {
                            Type = MetadataType.Environment,
                            Name = "pH",
                            Value = "7.1"
                        }
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
                item =>
                {
                    return Enumerable
                        .Range(0, item.Count)
                        .Select(idx =>
                        {
                            return new FieldIdentifier<int>
                            {
                                Id = idx,
                                Names = new[]
                                {
                                    $"{nameof(Coordinates.Latitude)} {idx + 1}",
                                    $"{nameof(Coordinates.Longitude)} {idx + 1}"
                                }
                            };
                        });
                },
                (item, headerId) =>
                {
                    if (headerId.Id < item.Count)
                    {
                        var coordinate = item.ElementAt(headerId.Id);

                        return new object[]
                        {
                            coordinate.Latitude,
                            coordinate.Longitude
                        };
                    }

                    return null;
                });

            serializer.AddDynamicFields(
                sampleData,
                item => item.Metadata,
                item =>
                {
                    return item
                        .Select(metadata =>
                        {
                            return new FieldIdentifier<KeyValuePair<MetadataType, string>>
                            {
                                Id = new KeyValuePair<MetadataType, string>(metadata.Type, metadata.Name),
                                Names = new[]
                                {
                                    $"{metadata.Type}-{metadata.Name}"
                                }
                            };
                        });
                },
                (item, headerId) =>
                {
                    var id = headerId.Id;

                    var dataType = id.Key;
                    var typeName = id.Value;

                    var metadata = item.SingleOrDefault(data => data.Type == dataType && data.Name == typeName);

                    return metadata == null
                        ? null
                        : new object[]
                        {
                            metadata.Value
                        };
                });
        }
    }
}
