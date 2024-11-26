namespace CsvExporterDemo.Data
{
    public sealed class SampleMetadata
    {
        public MetadataType Type { get; set; }
        public required string Name { get; set; }
        public required string Value { get; set; }
    }
}