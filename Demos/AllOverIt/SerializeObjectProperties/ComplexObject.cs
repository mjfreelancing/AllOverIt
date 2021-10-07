using System.Collections.Generic;

namespace SerializeObjectProperties
{
    internal sealed class ComplexObject
    {
        public sealed class Item
        {
            public sealed class ItemData
            {
                public IEnumerable<int> Points { get; set; }
            }

            public string Name { get; set; }
            public double Factor { get; set; }
            public ItemData Data { get; set; }
        }

        public IEnumerable<Item> Items { get; set; }
    }
}