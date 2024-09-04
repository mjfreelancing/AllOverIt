namespace CsvExporterDemo.Data
{
    public sealed class SampleData
    {
        public required string Name { get; set; }
        public required int Count { get; set; }

        // The key is the field heading
        public required IDictionary<string, int> Values { get; set; }

        // The serializer will be configured to identify each item by index and the header name will
        // be in the format "Latitude #" and "Longitude #" (both properties exported individually)
        public required Coordinates[] Coordinates { get; set; }

        // Each item will be exported as 'Type-Name' for the header and the Value is what will be exported
        public required SampleMetadata[] Metadata { get; set; }
    }
}
