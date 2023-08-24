using System.Collections.Generic;
using System.Linq;
using AllOverIt.Csv;
using AllOverIt.Csv.Extensions;
using CsvSerializerDemo.Data;

namespace CsvSerializerDemo.Extensions
{
    public static class SampleDataExtensions
    {
        public static void ConfigureCsvExport(this IReadOnlyCollection<SampleData> sampleData, ICsvSerializer<SampleData> serializer)
        {
            // Add fixed, known, columns
            serializer.AddField(nameof(SampleData.Name), item => item.Name);
            serializer.AddField(nameof(SampleData.Count), item => item.Count);

            // Add the Values field. This is a dictionary where the Key is the header name and the value is the value to be exported
            serializer.AddDynamicFields(
                sampleData,                                                             // The source data to be processed
                item => item.Values,                                                    // The property to be export across one or more CSV columns
                item => item.Keys,                                                      // Anything that uniquely identifies each row - this will be the name in the next Func
                (item, name) => item.TryGetValue(name, out var value) ? value : null    // Get the value to be exported for the column with the provided name
            );

            // Add the Coordinates field. Each Latitude and Longitude will be exported in columns based on their index, such as 'Latitude 1' and 'Longitude 1'
            serializer.AddDynamicFields(
                sampleData,
                item => item.Coordinates,
                item =>
                {
                    return Enumerable
                        .Range(0, item.Count)
                        .Select(idx =>
                        {
                            // Using an 'int' to uniquely identify each set of headers
                            return new FieldIdentifier<int>
                            {
                                Id = idx,
                                Names = new[]
                                {
                                    // Two columns to be exported per index
                                    $"{nameof(Coordinates.Latitude)} {idx + 1}",
                                    $"{nameof(Coordinates.Longitude)} {idx + 1}"
                                }
                            };
                        });
                },
                (item, headerId) =>
                {
                    // The 'Id' indicates the element index being exported
                    if (headerId.Id < item.Count)
                    {
                        var coordinate = item.ElementAt(headerId.Id);

                        // Since two headers were exported, there is an expectation that two values will be returned
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
                    var (dataType, typeName) = headerId.Id;

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