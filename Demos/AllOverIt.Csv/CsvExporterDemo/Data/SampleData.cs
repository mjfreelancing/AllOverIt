namespace CsvExporterDemo.Data
{
    public sealed class SampleData
    {
        public string Name { get; set; }
        public int Count { get; set; }

        // The key is the field heading
        public IDictionary<string, int> Values { get; set; }

        // The serializer will be configured to identify each item by index and the header name will
        // be in the format "Latitude #" and "Longitude #" (both properties exported individually)
        public IReadOnlyCollection<Coordinates> Coordinates { get; set; }

        // Each item will be exported as 'Type-Name' for the header and the Value is what will be exported
        public IReadOnlyCollection<SampleMetadata> Metadata { get; set; }
    }
}
