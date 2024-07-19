namespace InterfaceDeserializationDemo
{
    internal sealed class Parent : Person
    {
        public IChild[] ChildrenArray { get; set; } = [];
        public IEnumerable<IChild> ChildrenEnumerable { get; set; } = [];
    }
}