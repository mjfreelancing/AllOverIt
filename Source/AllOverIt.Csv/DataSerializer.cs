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

        public void AddField(string headerName, Func<TCsvData, object> valueResolver)
        {
            _fieldResolvers.Add(new CsvFieldResolver(headerName, valueResolver));
        }

        public async Task SerializeAsync(TextWriter writer, IEnumerable<TCsvData> data, bool includeHeader = true)
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
                var value = item.ValueResolver.Invoke(data);
                csv.WriteField(value);
            }

            await csv.NextRecordAsync().ConfigureAwait(false);
        }
    }
}
