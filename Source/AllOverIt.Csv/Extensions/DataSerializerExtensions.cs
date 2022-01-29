using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Csv.Extensions
{
    public static class DataSerializerExtensions
    {
        // Typically used with IDictionary<string, T> properties
        public static void AddDynamicFields<TCsvData, TField>(this IDataSerializer<TCsvData> serializer, IEnumerable<TCsvData> data,
            Func<TCsvData, TField> fieldSelector, Func<TField, IEnumerable<string>> headerNames, Func<TField, string, object> valueResolver)
        {
            var uniqueNames = data                          // From the source data
                .Select(fieldSelector)                      // Select the IEnumerable property to obtain header names for
                .SelectMany(headerNames.Invoke)             // Get all possible names for the current row
                .Distinct();                                // Reduce to a distinct list

            foreach (var valueName in uniqueNames)
            {
                // Get the value for a given Value (by name), or null if that name is not available for the row being processed
                serializer.AddField(valueName, item =>
                {
                    var field = fieldSelector.Invoke(item);

                    return valueResolver.Invoke(field, valueName);
                });
            }
        }

        // Typically used with IEnumerable<T> properties where TFieldId is 'int' (for the index)
        public static void AddDynamicFields<TCsvData, TField, TFieldId>(this IDataSerializer<TCsvData> serializer, IEnumerable<TCsvData> data,
            Func<TCsvData, TField> fieldSelector, Func<TField, IEnumerable<FieldIdentifier<TFieldId>>> fieldIdentifiers,
            Func<TField, FieldIdentifier<TFieldId>, IEnumerable<object>> valuesResolver)
        {
            var uniqueIdentifiers = data                        // From the source data
                .Select(fieldSelector)                          // Select the IEnumerable property to obtain header names for
                .SelectMany(fieldIdentifiers.Invoke)            // Get all possible identifier / names for the current row (such as the collection index and a name)
                .Distinct(FieldIdentifier<TFieldId>.Comparer);  // Reduce to a distinct list based on the 'Id' property (must be unique for each set of headers)

            foreach (var identifier in uniqueIdentifiers)
            {
                serializer.AddFields(identifier.Names, item =>
                {
                    var field = fieldSelector.Invoke(item);

                    var values = valuesResolver.Invoke(field, identifier);

                    return values ?? Enumerable.Repeat((object)null, identifier.Names.Count);
                });
            }
        }
    }
}
