using System.Collections.Generic;

namespace InterfaceDeserializationDemo
{
    internal sealed class Parent : Person
    {
        public IEnumerable<IChild> Children { get; set; }
    }
}