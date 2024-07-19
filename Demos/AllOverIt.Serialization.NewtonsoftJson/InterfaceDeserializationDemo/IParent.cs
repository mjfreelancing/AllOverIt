namespace InterfaceDeserializationDemo
{
    public interface IParent : IPerson
    {
        IChild[] ChildrenArray { get; }
        IEnumerable<IChild> ChildrenEnumerable { get; }
    }
}