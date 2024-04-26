namespace InterfaceDeserializationDemo
{
    internal sealed class Parent : Person
    {
        public IEnumerable<IChild> Children { get; set; }
    }
}