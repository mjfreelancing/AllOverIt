﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Csv.Extensions
{
    public static class DataSerializerExtensions
    {
        // Typically used with IDictionary<string, T> properties
        public static void AddDynamicFields<TCsvData, TField>(this IDataSerializer<TCsvData> serializer, IEnumerable<TCsvData> data,
            Func<TCsvData, TField> fieldSelector, Func<TField, IEnumerable<string>> headerName, Func<TField, string, object> valueResolver)
        {
            var uniqueNames = data                                      // From the source data
                .Select(fieldSelector)                                  // Select the IEnumerable property to obtain header names for
                .SelectMany(item => headerName.Invoke(item))            // Get all possible names for the current row
                .Distinct();                                            // Reduce to a distinct list

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

        // Typically used with IEnumerable<T> properties where THeaderId is 'int' (for the index)
        public static void AddDynamicFields<TCsvData, TField, THeaderId>(this IDataSerializer<TCsvData> serializer, IEnumerable<TCsvData> data,
            Func<TCsvData, TField> fieldSelector, Func<TField, IEnumerable<HeaderIdentifier<THeaderId>>> headerIdentifier,
            Func<TField, HeaderIdentifier<THeaderId>, object> valueResolver)
        {
            var uniqueIdentifiers = data                                // From the source data
                .Select(fieldSelector)                                  // Select the IEnumerable property to obtain header names for
                .SelectMany(item => headerIdentifier.Invoke(item))      // Get all possible identifier / names for the current row (such as the collection index and a name)
                .Distinct();                                            // Reduce to a distinct list

            foreach (var identifier in uniqueIdentifiers)
            {
                // Get the value for a given Value (by name), or null if that name is not available for the row being processed
                serializer.AddField(identifier.Name, item =>
                {
                    var field = fieldSelector.Invoke(item);

                    return valueResolver.Invoke(field, identifier);
                });
            }
        }
    }
}
