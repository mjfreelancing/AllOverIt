namespace SerializeObjectPropertiesDemo
{
    internal sealed class ComplexObject
    {
        public sealed class ComplexItem
        {
            public sealed class ComplexItemData
            {
                public DateTime Timestamp { get; set; }
                public IEnumerable<int> Values { get; set; } = [];
            }

            public string Name { get; set; } = string.Empty;
            public double Factor { get; set; }
            public ComplexItemData Data { get; set; } = new();
        }

        public IEnumerable<ComplexItem> Items { get; set; } = [];
    }
}