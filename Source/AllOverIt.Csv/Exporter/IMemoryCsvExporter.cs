namespace AllOverIt.Csv.Exporter
{
    /// <summary>Represents a buffered CSV exporter that caches rows of data that are flushed to memory when the buffer is full.</summary>
    /// <typeparam name="TModel">The model type representing the columns of each row to be exported.</typeparam>
    public interface IMemoryCsvExporter<TModel> : IBufferedCsvExporter<TModel> where TModel : class
    {
        /// <summary>Flushes any pending writes and gets the content currently stored in memory.</summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task that completes with the CSV content currently stored in memory.</returns>
        Task<byte[]> GetContentAsync(CancellationToken cancellationToken);
    }
}
