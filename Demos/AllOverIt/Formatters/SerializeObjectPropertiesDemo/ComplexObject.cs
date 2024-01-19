using System;
using System.Collections.Generic;

namespace SerializeObjectPropertiesDemo
{
    internal sealed class ComplexObject
    {
        public sealed class ComplexItem
        {
            public sealed class ComplexItemData
            {
                public DateTime Timestamp { get; set; }
                public IEnumerable<int> Values { get; set; }
            }

            public string Name { get; set; }
            public double Factor { get; set; }
            public ComplexItemData Data { get; set; }
        }

        public IEnumerable<ComplexItem> Items { get; set; }
    }
}