using AllOverIt.Assertion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Csv.Exporter
{
    public abstract class BufferedCsvExporterBase<TModel> : IBufferedCsvExporter<TModel>
    {
        private readonly BufferedCsvExporterConfiguration _configuration;
        private readonly IList<TModel> _data;

        private StreamWriter _writer;
        private ICsvSerializer<TModel> _csvSerializer;

        protected Stream Stream { get; private set; }       // The underlying stream passed to the StreamWriter

        protected BufferedCsvExporterBase()
            : this(new BufferedCsvExporterConfiguration())
        {
        }

        protected BufferedCsvExporterBase(BufferedCsvExporterConfiguration configuration)
        {
            _configuration = configuration.WhenNotNull(nameof(configuration));

            _data = new List<TModel>(_configuration.BufferSize);
        }

        public void Configure(IEnumerable<TModel> dynamicData = default)
        {
            Throw<InvalidOperationException>.WhenNotNull(_csvSerializer, "The CSV serializer is already configured.");

            _csvSerializer = CreateSerializer(dynamicData);
        }

        public Task AddDataAsync(TModel data, CancellationToken cancellationToken)
        {
            _data.Add(data);

            return ProcessBufferAsync(false, cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            GC.SuppressFinalize(this);
        }

        protected abstract Stream CreateStream();

        protected abstract ICsvSerializer<TModel> CreateSerializer(IEnumerable<TModel> dynamicData = default);

        protected async Task FlushAsync(CancellationToken cancellationToken)
        {
            await ProcessBufferAsync(true, cancellationToken);

            await _writer.FlushAsync();
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_writer is not null)
            {
                await FlushAsync(CancellationToken.None);

                await _writer.DisposeAsync();
                _writer = null;
            }
        }

        private async Task ProcessBufferAsync(bool force, CancellationToken cancellationToken)
        {
            Throw<InvalidOperationException>.WhenNull(_csvSerializer, "The CSV serializer is not configured.");

            if (force || _data.Count == _configuration.BufferSize)
            {
                var includeHeaders = _configuration.IncludeHeaders && _writer is null;

                if (_writer is null)
                {
                    Stream = CreateStream();

                    _writer = new StreamWriter(Stream, null, -1, false);
                }

                await _csvSerializer.SerializeAsync(_writer, _data, includeHeaders, true, cancellationToken);

                _data.Clear();
            }
        }
    }
}
