using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using AllOverIt.Extensions;

namespace AllOverIt.Csv
{
    /// <summary>Exports complex objects to CSV format.</summary>
    /// <typeparam name="TCsvData">The complex object type to be exported.</typeparam>
    public class CsvSerializer<TCsvData> : ICsvSerializer<TCsvData>
    {
        private sealed class CsvFieldResolver : IFieldResolver<TCsvData>
        {
            private readonly Func<TCsvData, IEnumerable<object>> _valuesResolver;

            public IReadOnlyCollection<string> HeaderNames { get; }

            public CsvFieldResolver(string headerName, Func<TCsvData, object> valueResolver)
            {
                HeaderNames = new []{ headerName };
                _valuesResolver = item => new[]{ valueResolver.Invoke(item)};
            }

            public CsvFieldResolver(IEnumerable<string> headerNames, Func<TCsvData, IEnumerable<object>> valuesResolver)
            {
                HeaderNames = headerNames.AsReadOnlyCollection();
                _valuesResolver = valuesResolver;
            }

            public IReadOnlyCollection<object> GetValues(TCsvData data)
            {
                return _valuesResolver
                    .Invoke(data)
                    .AsReadOnlyCollection();
            }
        }

        private readonly IList<IFieldResolver<TCsvData>> _fieldResolvers = new List<IFieldResolver<TCsvData>>();

        /// <inheritdoc />
        public void AddField(string headerName, Func<TCsvData, object> valueResolver)
        {
            _fieldResolvers.Add(new CsvFieldResolver(headerName, valueResolver));
        }

        /// <inheritdoc />
        public void AddFields(IEnumerable<string> headerNames, Func<TCsvData, IEnumerable<object>> valuesResolver)
        {
            _fieldResolvers.Add(new CsvFieldResolver(headerNames, valuesResolver));
        }

        /// <inheritdoc />
        public async Task SerializeAsync(TextWriter writer, IEnumerable<TCsvData> data, bool includeHeader = true)
        {
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                if (includeHeader)
                {
                    await WriteHeaderAsync(csv).ConfigureAwait(false);
                }

                foreach (var row in data)
                {
                    await WriteRowAsync(row, csv).ConfigureAwait(false);
                }
            }
        }

        private Task WriteHeaderAsync(IWriter csv)
        {
            foreach (var item in _fieldResolvers)
            {
                foreach (var headerName in item.HeaderNames)
                {
                    csv.WriteField(headerName);
                }
            }

            return csv.NextRecordAsync();
        }

        private async Task WriteRowAsync(TCsvData data, IWriter csv)
        {
            foreach (var item in _fieldResolvers)
            {
                var values = item.GetValues(data);
                
                foreach (var value in values)
                {
                    csv.WriteField(value);
                }
            }

            await csv.NextRecordAsync().ConfigureAwait(false);
        }
    }
}
