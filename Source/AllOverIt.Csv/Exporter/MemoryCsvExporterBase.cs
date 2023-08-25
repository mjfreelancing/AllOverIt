using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Csv.Exporter
{
    /// <summary>Implements a buffered CSV exporter that writes the content to a memory stream.</summary>
    /// <typeparam name="TModel">The model type representing the columns of each row to be exported.</typeparam>
    public abstract class MemoryCsvExporterBase<TModel> : BufferedCsvExporterBase<TModel>, IMemoryCsvExporter<TModel> where TModel : class
    {
        /// <summary>Constructor. Initialized with a default <see cref="BufferedCsvExporterConfiguration"/>.</summary>
        public MemoryCsvExporterBase()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="configuration">The configuration to use.</param>
        public MemoryCsvExporterBase(BufferedCsvExporterConfiguration configuration)
            : base(configuration)
        {
        }

        /// <inheritdoc />
        public async Task<byte[]> GetContentAsync(CancellationToken cancellationToken)
        {
            await FlushAsync(cancellationToken);

            return Stream is null
                ? Array.Empty<byte>() 
                : ((MemoryStream) Stream).ToArray();
        }

        /// <inheritdoc />
        protected override Stream CreateStream()
        {
            return new MemoryStream();
        }
    }
}
