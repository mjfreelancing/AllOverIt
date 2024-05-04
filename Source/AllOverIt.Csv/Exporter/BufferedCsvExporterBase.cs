using AllOverIt.Assertion;
using AllOverIt.Csv.Exceptions;

namespace AllOverIt.Csv.Exporter
{
    /// <summary>Abstract class providing support for buffering CSV data and writing it to the underlying CSV writer
    /// as the buffer fills.</summary>
    /// <typeparam name="TModel">The model type representing the columns of each row to be exported.</typeparam>
    public abstract class BufferedCsvExporterBase<TModel> : IBufferedCsvExporter<TModel> where TModel : class
    {
        private readonly BufferedCsvExporterConfiguration _configuration;
        private readonly List<TModel> _data;

        private StreamWriter? _writer;
        private ICsvSerializer<TModel>? _csvSerializer;

        /// <summary>The underlying stream provided to the CSV writer. This stream is created on demand when the buffer
        /// is first flushed, including during a call to <see cref="FlushAsync(CancellationToken)"/> when there is data
        /// available for writing.</summary>
        protected Stream? Stream { get; private set; }

        /// <summary>Constructor. Initialized with a default <see cref="BufferedCsvExporterConfiguration"/>.</summary>
        protected BufferedCsvExporterBase()
            : this(new BufferedCsvExporterConfiguration())
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="configuration">The configuration to use.</param>
        protected BufferedCsvExporterBase(BufferedCsvExporterConfiguration configuration)
        {
            _configuration = configuration.WhenNotNull(nameof(configuration));

            _data = new List<TModel>(_configuration.BufferSize);
        }

        /// <inheritdoc />
        public void Configure(IEnumerable<TModel>? configData = default)
        {
            Throw<CsvExporterException>.WhenNotNull(_csvSerializer, "The CSV serializer is already configured.");

            _csvSerializer = CreateSerializer(configData);
        }

        /// <inheritdoc />
        public Task AddDataAsync(TModel data, CancellationToken cancellationToken)
        {
            _ = data.WhenNotNull(nameof(data));

            _data.Add(data);

            return ProcessBufferAsync(false, cancellationToken);
        }

        /// <inheritdoc />
        public async Task FlushAsync(CancellationToken cancellationToken)
        {
            await ProcessBufferAsync(true, cancellationToken);

            // _writer will be null if there was no data to process
            if (_writer is not null)
            {
#if NETSTANDARD2_1
                await _writer.FlushAsync();
#else
                await _writer.FlushAsync(cancellationToken);
#endif
            }
        }

        /// <inheritdoc />
        public async Task CloseAsync(CancellationToken cancellationToken)
        {
            // Force the _writer to be created and data flushed if the buffer has not been exhausted
            await FlushAsync(cancellationToken);

            if (_writer is not null)
            {
                await _writer.DisposeAsync();
                _writer = null;

                _csvSerializer = null;
            }
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            GC.SuppressFinalize(this);
        }

        /// <summary>This method is called when the stream is required to initialize the underlying CSV writer. This occurs
        /// when the buffer is first flushed, including during a call to <see cref="FlushAsync(CancellationToken)"/> when there
        /// is data available for writing.</summary>
        /// <returns>The newly created stream. This will be disposed when the underlying writer is disposed of.</returns>
        protected abstract Stream CreateStream();

        /// <summary> This method is called at the time of calling <see cref="Configure(IEnumerable{TModel})"/>. The method
        /// must return a newly instantiated serializer that has been completely configured.</summary>
        /// <param name="configData"><see cref="CsvSerializer{TCsvData}"/> supports the dynamic generation of columns based on the provided data.
        /// If all columns are fixed / well-known then this parameter will be <see langword="null"/>. When not <see langword="null"/>, the provided
        /// data will be used to establish the names of the dyanmic columns, through the use of calls to one of the <see cref="ICsvSerializer{TCsvData}"/>
        /// extension methods <c>AddDynamicFields()</c>.</param>
        /// <returns>The newly instantiated, and configured, CSV serializer.</returns>
        protected abstract ICsvSerializer<TModel> CreateSerializer(IEnumerable<TModel>? configData = default);

        /// <summary>Disposed of internal resources.</summary>
        /// <returns>A <see cref="ValueTask"/> that completes when all resources have been disposed of.</returns>
        protected virtual async ValueTask DisposeAsyncCore()
        {
            await CloseAsync(CancellationToken.None);
        }

        private async Task ProcessBufferAsync(bool force, CancellationToken cancellationToken)
        {
            Throw<CsvExporterException>.WhenNull(_csvSerializer, "The CSV serializer is not configured.");

            cancellationToken.ThrowIfCancellationRequested();

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
