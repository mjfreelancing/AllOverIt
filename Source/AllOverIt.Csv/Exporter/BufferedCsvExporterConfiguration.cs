namespace AllOverIt.Csv.Exporter
{
    public sealed class BufferedCsvExporterConfiguration
    {
        public int BufferSize { get; init; } = 16;
        public bool IncludeHeaders { get; init; } = true;
    }
}
