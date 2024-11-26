using AllOverIt.Assertion;
using CsvHelper;
using System.Globalization;

namespace AllOverIt.Csv
{
    /// <summary>Exports complex objects to CSV format.</summary>
    /// <typeparam name="TCsvData">The complex object type to be exported.</typeparam>
    public class CsvSerializer<TCsvData> : ICsvSerializer<TCsvData>
    {
        private sealed class CsvFieldResolver : IFieldResolver<TCsvData>
        {
            private readonly Func<TCsvData, object?[]> _valuesResolver;

            public string[] HeaderNames { get; }

            public CsvFieldResolver(string headerName, Func<TCsvData, object?> valueResolver)
            {
                HeaderNames = [headerName];
                _valuesResolver = item => [valueResolver.Invoke(item)];
            }

            public CsvFieldResolver(string[] headerNames, Func<TCsvData, object?[]> valuesResolver)
            {
                HeaderNames = headerNames;
                _valuesResolver = valuesResolver;
            }

            public object?[] GetValues(TCsvData data)
            {
                return _valuesResolver.Invoke(data);
            }
        }

        private readonly List<IFieldResolver<TCsvData>> _fieldResolvers = [];

        /// <inheritdoc />
        public void AddField(string headerName, Func<TCsvData, object?> valueResolver)
        {
            _ = headerName.WhenNotNullOrEmpty();
            _ = valueResolver.WhenNotNull();

            _fieldResolvers.Add(new CsvFieldResolver(headerName, valueResolver));
        }

        /// <inheritdoc />
        public void AddFields(string[] headerNames, Func<TCsvData, object?[]> valuesResolver)
        {
            _ = headerNames.WhenNotNull();
            _ = valuesResolver.WhenNotNull();

            _fieldResolvers.Add(new CsvFieldResolver(headerNames, valuesResolver));
        }

        /// <inheritdoc />
        public async Task SerializeAsync(TextWriter writer, IEnumerable<TCsvData> data, bool includeHeader = true, bool leaveOpen = false,
            CancellationToken cancellationToken = default)
        {
            _ = writer.WhenNotNull();
            _ = data.WhenNotNull();

            // Prepares the writer, streams the header (if required), and then calls back to stream the data
            await SerializeDataAsync(
                writer,
                async csv =>
                {
                    foreach (var row in data)
                    {
                        await WriteRowAsync(row, csv, cancellationToken).ConfigureAwait(false);
                    }
                },
                includeHeader,
                leaveOpen,
                cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task SerializeAsync(TextWriter writer, IAsyncEnumerable<TCsvData> data, bool includeHeader = true, bool leaveOpen = false,
            CancellationToken cancellationToken = default)
        {
            _ = writer.WhenNotNull();
            _ = data.WhenNotNull();

            // Prepares the writer, streams the header (if required), and then calls back to stream the data
            await SerializeDataAsync(
                writer, async csv =>
                {
                    await foreach (var row in data.WithCancellation(cancellationToken).ConfigureAwait(false))
                    {
                        await WriteRowAsync(row, csv, cancellationToken).ConfigureAwait(false);
                    }
                },
                includeHeader,
                leaveOpen,
                cancellationToken).ConfigureAwait(false);
        }

        private async Task SerializeDataAsync(TextWriter writer, Func<CsvWriter, Task> action, bool includeHeader, bool leaveOpen, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture, leaveOpen))
            {
                if (includeHeader)
                {
                    await WriteHeaderAsync(csv, cancellationToken).ConfigureAwait(false);
                }

                await action.Invoke(csv).ConfigureAwait(false);
            }
        }

        private Task WriteHeaderAsync(CsvWriter csv, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var item in _fieldResolvers)
            {
                foreach (var headerName in item.HeaderNames)
                {
                    csv.WriteField(headerName);
                }
            }

            return csv.NextRecordAsync();
        }

        private async Task WriteRowAsync(TCsvData data, CsvWriter csv, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

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
