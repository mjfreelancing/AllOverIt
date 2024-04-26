namespace SerializationFilterBenchmarking
{
    internal sealed class ComplexObject
    {
        public sealed class Item
        {
            public sealed class ItemData
            {
                public DateTime Timestamp { get; set; }
                public IEnumerable<int> Values { get; set; }
            }

            public string Name { get; set; }
            public double Factor { get; set; }
            public ItemData Data { get; set; }
        }

        public IEnumerable<Item> Items { get; set; }
    }
}
