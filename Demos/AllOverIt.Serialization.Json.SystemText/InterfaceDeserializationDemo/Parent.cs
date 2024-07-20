namespace InterfaceDeserializationDemo
{
    internal sealed class Parent : Person
    {
        public IChild[] ChildrenArray { get; set; } = [];
        public IEnumerable<IChild> ChildrenEnumerable { get; set; } = [];
        public IList<IChild> ChildrenList { get; set; } = [];
    }
}