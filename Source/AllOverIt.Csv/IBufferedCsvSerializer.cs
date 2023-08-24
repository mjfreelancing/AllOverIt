using AllOverIt.Assertion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Csv
{
    public interface IBufferedCsvSerializer
        <TModel> : IAsyncDisposable
    {
        void Configure(IEnumerable<TModel> dynamicFields = default);
        Task AddDataAsync(TModel data, CancellationToken cancellationToken);
        Task<ReadOnlyMemory<byte>> GetContentAsync(CancellationToken cancellationToken);
    }



    public sealed class BufferedCsvSerializerConfiguration
    {
        public int BufferSize { get; init; } = 16;
        public bool IncludeHeaders { get; init; } = true;
    }



    public abstract class BufferedCsvSerializerBase<TModel> : IBufferedCsvSerializer<TModel>
    {
        private readonly BufferedCsvSerializerConfiguration _configuration;
        private readonly IList<TModel> _data;

        private StreamWriter _writer;
        private ICsvSerializer<TModel> _csvSerializer;

        protected BufferedCsvSerializerBase()
            : this(new BufferedCsvSerializerConfiguration())
        {
        }

        protected BufferedCsvSerializerBase(BufferedCsvSerializerConfiguration configuration)
        {
            _configuration = configuration.WhenNotNull(nameof(configuration));

            _data = new List<TModel>(_configuration.BufferSize);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            GC.SuppressFinalize(this);
        }

        public void Configure(IEnumerable<TModel> dynamicFields = default)
        {
            Throw<InvalidOperationException>.WhenNotNull(_csvSerializer, "The CSV serializer is already configured.");

            _csvSerializer = CreateSerializer(dynamicFields);
        }

        public Task AddDataAsync(TModel data, CancellationToken cancellationToken)
        {
            _data.Add(data);

            return ProcessBufferAsync(false, cancellationToken);
        }

        public async Task<ReadOnlyMemory<byte>> GetContentAsync(CancellationToken cancellationToken)
        {
            await ProcessBufferAsync(true, cancellationToken);

            await _writer.FlushAsync();

            return ((MemoryStream) _writer.BaseStream).ToArray();
        }

        protected abstract ICsvSerializer<TModel> CreateSerializer(IEnumerable<TModel> dynamicFields = default);

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_writer is not null)
            {
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

                _writer ??= new(new MemoryStream(), leaveOpen: false);

                await _csvSerializer.SerializeAsync(_writer, _data, includeHeaders, true, cancellationToken);

                _data.Clear();
            }
        }
    }
}
