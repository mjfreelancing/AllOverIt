using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Csv.Exporter
{
    public interface IMemoryCsvExporter<TModel> : IBufferedCsvExporter<TModel>
    {
        Task<byte[]> GetContentAsync(CancellationToken cancellationToken);
    }
}
