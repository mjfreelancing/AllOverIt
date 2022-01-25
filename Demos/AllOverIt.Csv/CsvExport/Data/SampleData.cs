using System.Collections.Generic;

namespace CsvExport.Data
{
    internal sealed class SampleData
    {
        public string Name { get; set; }
        public int Count { get; set; }

        // The key is the field heading
        public IDictionary<string, int> Values { get; set; }

        // The serializer will be configured to identify each value by index and the header name will be in the format "Coordinate #"
        public IReadOnlyCollection<Coordinates> Coordinates { get; set; }
    }
}
