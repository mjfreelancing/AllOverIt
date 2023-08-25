namespace AllOverIt.Csv.Exporter
{
    /// <summary>Provides configuration options for buffered CSV exporters such as <see cref="MemoryCsvExporterBase{TModel}"/>
    /// and <see cref="FileCsvExporterBase{TModel}"/>.</summary>
    public sealed class BufferedCsvExporterConfiguration
    {
        /// <summary>The number of CSV rows to buffer before flushing to the underlying writer. The recommended buffer
        /// size is highly dependent on the size of the model representing each row to be exported.</summary>
        public int BufferSize { get; init; } = 16;

        /// <summary>Indicates if the CSV export is to include the header row.</summary>
        public bool IncludeHeaders { get; init; } = true;
    }
}
