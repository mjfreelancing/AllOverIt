using System.Collections.Generic;

namespace InterfaceDeserializationDemo
{
    public interface IParent : IPerson
    {
        public IEnumerable<IChild> Children { get; }
    }
}