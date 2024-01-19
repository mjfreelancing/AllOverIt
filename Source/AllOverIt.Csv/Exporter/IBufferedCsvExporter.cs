using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Csv.Exporter
{
    /// <summary>Represents a buffered CSV exporter that caches rows of data that are flushed when the buffer is full.</summary>
    /// <typeparam name="TModel">The model type representing the columns of each row to be exported.</typeparam>
    public interface IBufferedCsvExporter<TModel> : IAsyncDisposable where TModel : class
    {
        /// <summary>Configures the underlying <see cref="CsvSerializer{TCsvData}"/>. This method must be called before adding data.</summary>
        /// <param name="configData"><see cref="CsvSerializer{TCsvData}"/> supports the dynamic generation of columns based on the provided data.
        /// If all columns are fixed / well-known then this parameter will be <see langword="null"/>. When not <see langword="null"/>, the provided
        /// data will be used to establish the names of the dyanmic columns, through the use of calls to one of the <see cref="ICsvSerializer{TCsvData}"/>
        /// extension methods <c>AddDynamicFields()</c>.</param>
        void Configure(IEnumerable<TModel> configData = default);

        /// <summary>Adds a row of data to the CSV content. Each.</summary>
        /// <param name="data">The model containing the data for each column to be exported, as configured via <see cref="Configure(IEnumerable{TModel})"/>.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="Task"/> that completes when the data has been added to the buffer. The data will be flushed to the underlying
        /// writer each time the buffer is filled to capacity.</returns>
        Task AddDataAsync(TModel data, CancellationToken cancellationToken);

        /// <summary>Flushes the buffer to the underlying writer. This method is called internally as required so does not normally need to be explicitly
        /// called but is available for scenarios where the caller wants to force the buffer to be flushed.</summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="Task"/> that completes when the data has been fully flushed and emptied the internal buffer.</returns>
        Task FlushAsync(CancellationToken cancellationToken);

        /// <summary>Flushes the internal buffer to the underlying writer and completes the writing of data. This method is implicitly called when
        /// the exporter is disposed of but is available for scenarios where the caller wants to force this action.</summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="Task"/> that completes when the data has been fully flushed and written to the underlying writer.</returns>
        Task CloseAsync(CancellationToken cancellationToken);
    }
}
