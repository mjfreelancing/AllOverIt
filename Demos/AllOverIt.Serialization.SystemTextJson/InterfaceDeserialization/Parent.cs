using System.Collections.Generic;

namespace InterfaceDeserialization
{
    internal sealed class Parent : Person
    {
        public IEnumerable<IChild> Children { get; set; }
    }
}