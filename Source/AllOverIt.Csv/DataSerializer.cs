using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AllOverIt.Csv
{
    public class DataSerializer<TCsvData> : IDataSerializer<TCsvData>
    {
        private sealed class CsvFieldResolver : IExportFieldResolver<TCsvData>
        {
            public string HeaderName { get; }
            public Func<TCsvData, object> ValueResolver { get; }

            public CsvFieldResolver(string headerName, Func<TCsvData, object> valueResolver)
            {
                HeaderName = headerName;
                ValueResolver = valueResolver;
            }
        }

        private readonly IList<IExportFieldResolver<TCsvData>> _fieldResolvers = new List<IExportFieldResolver<TCsvData>>();

        public void AddFixedField(string headerName, Func<TCsvData, object> valueResolver)
        {
            _fieldResolvers.Add(new CsvFieldResolver(headerName, valueResolver));
        }

        // Typically used with IDictionary<string, T> properties
        public void AddDynamicFields<TField>(IEnumerable<TCsvData> data, Func<TCsvData, TField> fieldSelector,
            Func<TField, IEnumerable<string>> headerName, Func<TField, string, object> valueResolver)
        {
            var uniqueNames = data                                      // From the source data
                .Select(fieldSelector)                                  // Select the IEnumerable property to obtain header names for
                .SelectMany(item => headerName.Invoke(item))            // Get all possible names for the current row
                .Distinct();                                            // Reduce to a distinct list

            foreach (var valueName in uniqueNames)
            {
                // Get the value for a given Value (by name), or null if that name is not available for the row being processed
                AddFixedField(valueName, item =>
                {
                    var field = fieldSelector.Invoke(item);

                    return valueResolver.Invoke(field, valueName);
                });
            }
        }

        // Typically used with IEnumerable<T> properties where THeaderId is 'int' (for the index)
        public void AddDynamicFields<TField, THeaderId>(IEnumerable<TCsvData> data, Func<TCsvData, TField> fieldSelector,
            Func<TField, IEnumerable<HeaderIdentifier<THeaderId>>> headerIdentifier,
            Func<TField, HeaderIdentifier<THeaderId>, object> valueResolver)
        {
            var uniqueIdentifiers = data                                // From the source data
                .Select(fieldSelector)                                  // Select the IEnumerable property to obtain header names for
                .SelectMany(item => headerIdentifier.Invoke(item))      // Get all possible identifier / names for the current row (such as the collection index and a name)
                .Distinct();                                            // Reduce to a distinct list

            foreach (var identifier in uniqueIdentifiers)
            {
                // Get the value for a given Value (by name), or null if that name is not available for the row being processed
                AddFixedField(identifier.Name, item =>
                {
                    var field = fieldSelector.Invoke(item);

                    return valueResolver.Invoke(field, identifier);
                });
            }
        }

        public async Task Serialize(TextWriter writer, IEnumerable<TCsvData> data, bool includeHeader = true)
        {
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                if (includeHeader)
                {
                    await WriteHeaderAsync(csv);
                }

                foreach (var row in data)
                {
                    await WriteRowAsync(row, csv);
                }
            }
        }

        private Task WriteHeaderAsync(IWriter csv)
        {
            foreach (var item in _fieldResolvers)
            {
                csv.WriteField(item.HeaderName);
            }

            return csv.NextRecordAsync();
        }

        private async Task WriteRowAsync(TCsvData data, IWriter csv)
        {
            foreach (var item in _fieldResolvers)
            {
                csv.WriteField(item.ValueResolver.Invoke(data));
            }

            await csv.NextRecordAsync().ConfigureAwait(false);
        }
    }
}
