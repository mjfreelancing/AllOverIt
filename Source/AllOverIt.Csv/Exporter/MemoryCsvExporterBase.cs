using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Csv.Exporter
{
    public abstract class MemoryCsvExporterBase<TModel> : BufferedCsvExporterBase<TModel>, IMemoryCsvExporter<TModel>
    {
        public async Task<byte[]> GetContentAsync(CancellationToken cancellationToken)
        {
            await FlushAsync(cancellationToken);

            return ((MemoryStream) Stream).ToArray();
        }

        protected override Stream CreateStream()
        {
            return new MemoryStream();
        }
    }
}
