using CsvExporterDemo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CsvExporterDemo
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
        static async Task Main()
        {
            var sampleData = CreateSampleData();

            var exporter = new SampleDataExporter();

            // Providing all data for configuring the serializer only because it's needed for configuration of the dynamic headers.
            //
            // Other scenarios:
            //
            // 1 - If there were no dynamic columns 'SampleDataExporter' could simply call the base class Configure() in its constructor.
            //
            // 2 - If there's a need to export large amounts of data where dynamic columns are involved, try to collect all unique headers
            //     in advance and construct a collection containing this data for the purpose of configuration. The real data can then be
            //     passed to AddDataAsync() for exporting.
            exporter.Configure(sampleData);

            foreach (var data in sampleData)
            {
                await exporter.AddDataAsync(data, CancellationToken.None);
            }

            var content = await exporter.GetContentAsync(CancellationToken.None);

            var str = Encoding.UTF8.GetString(content);

            Console.WriteLine(str);

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static List<SampleData> CreateSampleData()
        {
            return Enumerable
                .Range(0, 10)
                .SelectMany(number =>
                {
                    var start = number * 4 + 1;

                    return new[]
                    {
                        new SampleData
                        {
                            Name = $"Sample {start}",
                            Count = 0,
                            Values = new Dictionary<string, int>(),
                            Coordinates = new List<Coordinates>(),
                            Metadata = new List<SampleMetadata>()
                        },

                        new SampleData
                        {
                            Name = $"Sample {start + 1}",
                            Count = 1,
                            Values = new Dictionary<string, int>
                            {
                                { "Value 1", 1 }
                            },
                            Coordinates = new List<Coordinates>
                            {
                                new(100.1, 120.2)
                            },
                            Metadata = new List<SampleMetadata>
                            {
                                new()
                                {
                                    Type = MetadataType.Environment,
                                    Name = "Temperature",
                                    Value = "30"
                                },
                                new()
                                {
                                    Type = MetadataType.Quality,
                                    Name = "Colour",
                                    Value = "8"
                                },
                                new()
                                {
                                    Type = MetadataType.Quality,
                                    Name = "Clarity",
                                    Value = "3"
                                },
                                new()
                                {
                                    Type = MetadataType.Environment,
                                    Name = "pH",
                                    Value = "7.0"
                                }
                            }
                        },

                        new SampleData
                        {
                            Name = $"Sample {start + 2}",
                            Count = 3,
                            Values = new Dictionary<string, int>
                            {
                                { "Value 1", 1 },
                                { "Value 2", 2 },
                                { "Value 3", 3 }
                            },
                            Coordinates = new List<Coordinates>
                            {
                                new(100.4, 119.8),
                                new(100.7, 120.0),
                                new(100.3, 119.2),
                            },
                            Metadata = new List<SampleMetadata>
                            {
                                new()
                                {
                                    Type = MetadataType.Environment,
                                    Name = "Temperature",
                                    Value = "28"
                                },
                                new()
                                {
                                    Type = MetadataType.Environment,
                                    Name = "pH",
                                    Value = "6.9"
                                }
                            }
                        },

                        new SampleData
                        {
                            Name = $"Sample {start + 3}",
                            Count = 2,
                            Values = new Dictionary<string, int>
                            {
                                { "Value 1", 1 },
                                { "Value 2", 2 }
                            },
                            Coordinates = new List<Coordinates>
                            {
                                new(100.1, 121.3),
                                new(100.8, 120.5),
                            },
                            Metadata = new List<SampleMetadata>
                            {
                                new()
                                {
                                    Type = MetadataType.Quality,
                                    Name = "Colour",
                                    Value = "9"
                                },
                                new()
                                {
                                    Type = MetadataType.Quality,
                                    Name = "Clarity",
                                    Value = "2"
                                },
                                new()
                                {
                                    Type = MetadataType.Environment,
                                    Name = "pH",
                                    Value = "7.1"
                                }
                            }
                        }
                    };
                })
                .ToList();
        }
    }
}
