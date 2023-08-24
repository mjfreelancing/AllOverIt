using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Csv.Exporter
{
    public interface IBufferedCsvExporter<TModel> : IAsyncDisposable
    {
        void Configure(IEnumerable<TModel> dynamicData = default);
        Task AddDataAsync(TModel data, CancellationToken cancellationToken);
    }
}
