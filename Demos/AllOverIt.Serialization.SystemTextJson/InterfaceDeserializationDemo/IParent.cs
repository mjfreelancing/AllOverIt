namespace InterfaceDeserializationDemo
{
    public interface IParent : IPerson
    {
        public IEnumerable<IChild> Children { get; }
    }
}