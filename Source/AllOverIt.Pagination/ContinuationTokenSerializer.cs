using AllOverIt.Extensions;
using AllOverIt.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace AllOverIt.Pagination
{
    internal static class ContinuationTokenSerializer
    {
        public static string Serialize(ContinuationToken continuationToken)
        {
            using(var stream = new MemoryStream())
            {
                using (var writer = new EnrichedBinaryWriter(stream))
                {
                    // Direction
                    writer.Write((byte) continuationToken.Direction);

                    // Number of values - surely only a handful of values
                    var valueCount = continuationToken.Values?.Count ?? 0;
                    writer.Write((byte) valueCount);

                    if (valueCount > 0)
                    {
                        // Each Value
                        foreach (var value in continuationToken.Values)
                        {
                            writer.WriteObject(value);

                            //// Not using TypeDescriptor.GetConverter() as it incorrectly formats DateTime values (there's no converter)
                            //// so As<string>() converts it to a string first => see if we can find a more efficient approach
                            //var strValue = value.As<string>();
                            //var valueBytes = Encoding.UTF8.GetBytes(strValue);

                            //// Store as individual bytes with a header length (avoid issues with delimiters and quotes in strings)
                            //writer.Write(valueBytes.Length);
                            //writer.Write(valueBytes);
                        }
                    }

                    stream.Position = 0;
                    var bytes = stream.ToArray();

                    return Convert.ToBase64String(bytes);
                }
            }
        }

        public static ContinuationToken Deserialize(string continuationToken, IReadOnlyList<IColumnDefinition> columnDefinitions)
        {
            if (continuationToken.IsNullOrEmpty())
            {
                return ContinuationToken.None;
            }

            var bytes = Convert.FromBase64String(continuationToken);

            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new EnrichedBinaryReader(stream))
                {
                    var direction = (PaginationDirection) reader.ReadByte();
                    var valueCount = (int) reader.ReadByte();

                    if (valueCount == 0)
                    {
                        return new ContinuationToken
                        {
                            Direction = direction
                        };
                    }

                    var values = new List<object>();

                    for (var i = 0; i < valueCount; i++)
                    {
                        //var length = reader.ReadInt32();
                        //var valueBytes = reader.ReadBytes(length);
                        //var strValue = Encoding.UTF8.GetString(valueBytes);

                        //var column = columnDefinitions[i];
                        //var value = TypeDescriptor.GetConverter(column.Property.PropertyType).ConvertFrom(strValue);

                        var value = reader.ReadObject();

                        values.Add(value);
                    }

                    return new ContinuationToken
                    {
                        Direction = direction,
                        Values = values
                    };
                }
            }
        }

        //private static string ValueToString(object value)
        //{
        //    return TypeDescriptor
        //        .GetConverter(value.GetType())
        //        .ConvertToInvariantString(value);
        //}

        //private static object ValueFromString(string value, Type type)
        //{
        //    return TypeDescriptor
        //        .GetConverter(type)
        //        .ConvertFrom(value);
        //}
    }
}
