using AllOverIt.Assertion;
using AllOverIt.Csv.Exceptions;
using AllOverIt.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Csv.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="ICsvSerializer{TCsvData}"/>.</summary>
    public static class CsvSerializerExtensions
    {
        /// <summary>Adds configuration for fields dynamically determined from a complex field on the <typeparamref name="TCsvData"/> type.</summary>
        /// <typeparam name="TCsvData">The data type to be exported to CSV.</typeparam>
        /// <typeparam name="TField">The field type on <typeparamref name="TCsvData"/> to be configured.</typeparam>
        /// <param name="serializer">The serializer performing the CSV export.</param>
        /// <param name="data">The collection of data to be exported.</param>
        /// <param name="fieldSelector">Gets the field instance to be configured and exported during serialization.</param>
        /// <param name="headerNameResolver">Based on the field instance, returns a list of header names to be exported. These names are usually derived
        /// from other properties on the field. A typical use-case when <typeparamref name="TField"/> is an IDictionary&lt;U, V&gt; and the
        /// dictionary's Keys are used as the header names.</param>
        /// <param name="valueResolver">This resolver is called during the export process for each field instance and its associated header name.
        /// If <typeparamref name="TField"/> is an IDictionary&lt;U, V&gt; then the header name is typically used to look up the value based on this key.</param>
        public static void AddDynamicFields<TCsvData, TField>(this ICsvSerializer<TCsvData> serializer, IEnumerable<TCsvData> data,
            Func<TCsvData, TField> fieldSelector, Func<TField, IEnumerable<string>> headerNameResolver, Func<TField, string, object> valueResolver)
        {
            _ = serializer.WhenNotNull(nameof(serializer));
            var csvData = data.WhenNotNull(nameof(data));
            _ = fieldSelector.WhenNotNull(nameof(fieldSelector));
            _ = headerNameResolver.WhenNotNull(nameof(headerNameResolver));
            _ = valueResolver.WhenNotNull(nameof(valueResolver));

            var uniqueNames = csvData                          // From the source data
                .Select(fieldSelector)                      // Select the IEnumerable property to obtain header names for
                .SelectMany(headerNameResolver.Invoke)             // Get all possible names for the current row
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

        /// <summary>Adds configuration for fields dynamically determined from a complex field on the <typeparamref name="TCsvData"/> type.</summary>
        /// <typeparam name="TCsvData">The data type to be exported to CSV.</typeparam>
        /// <typeparam name="TField">The field type on <typeparamref name="TCsvData"/> to be configured.</typeparam>
        /// <typeparam name="TFieldId">A field identifier type. The value of this field (The Id property of in <see cref="FieldIdentifier{TFieldId}"/>) is used to
        /// find unique sets of column headers.</typeparam>
        /// <param name="serializer">The serializer performing the CSV export.</param>
        /// <param name="data">The collection of data to be exported.</param>
        /// <param name="fieldSelector">Gets the field instance to be configured and exported during serialization.</param>
        /// <param name="fieldIdentifiers">Returns a list of unique identifiers for the field instance to be exported. This identifier contains an 'Id' property
        /// that must be unique for each set of header names to be exported. As an example, if the field is an array/list then the Id could be the index and
        /// the 'Names' could be 'PropA 1' and 'PropB 1', assuming 'PropA' and 'PropB' were being exported.</param>
        /// <param name="valuesResolver">This resolver is called during the export process for each field instance and its associated field identifier. The 'Id'
        /// property of the field identifier is typically used to find the element to be exported. The serializer expects the same number of values to be returned
        /// as there were header names. If there's no data to be exported for this field instance then return null.</param>
        /// <exception cref="CsvSerializerException">When the number of values returned does not match the number of header names.</exception>
        public static void AddDynamicFields<TCsvData, TField, TFieldId>(this ICsvSerializer<TCsvData> serializer, IEnumerable<TCsvData> data,
            Func<TCsvData, TField> fieldSelector, Func<TField, IEnumerable<FieldIdentifier<TFieldId>>> fieldIdentifiers,
            Func<TField, FieldIdentifier<TFieldId>, IEnumerable<object>> valuesResolver)
        {
            _ = serializer.WhenNotNull(nameof(serializer));
            var csvData = data.WhenNotNull(nameof(data));
            _ = fieldSelector.WhenNotNull(nameof(fieldSelector));
            _ = fieldIdentifiers.WhenNotNull(nameof(fieldIdentifiers));
            _ = valuesResolver.WhenNotNull(nameof(valuesResolver));

            var uniqueIdentifiers = csvData                     // From the source data
                .Select(fieldSelector)                          // Select the IEnumerable property to obtain header names for
                .SelectMany(fieldIdentifiers.Invoke)            // Get all possible identifier / names for the current row (such as the collection index and a name)
                .Distinct(FieldIdentifier<TFieldId>.Comparer);  // Reduce to a distinct list based on the 'Id' property (must be unique for each set of headers)

            foreach (var identifier in uniqueIdentifiers)
            {
                var columnCount = identifier.Names.Count;

                serializer.AddFields(identifier.Names, row =>
                {
                    var field = fieldSelector.Invoke(row);

                    var values = valuesResolver
                        .Invoke(field, identifier)
                        ?.AsReadOnlyCollection();

                    values ??= Enumerable.Repeat((object) null, identifier.Names.Count).ToList();

                    if (values.Count != columnCount)
                    {
                        throw new CsvSerializerException($"Column count mismatch. Expected {columnCount}, found {values.Count}.");
                    }

                    return values;
                });
            }
        }
    }
}
