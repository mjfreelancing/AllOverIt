using System.Collections.Generic;

namespace InterfaceDeserialization
{
    public interface IParent : IPerson
    {
        public IEnumerable<IChild> Children { get; }
    }
}