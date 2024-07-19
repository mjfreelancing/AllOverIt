namespace InterfaceDeserializationDemo
{
    internal sealed class Child : IChild
    {
        public required string FirstName { get; set; }
        public required int Age { get; set; }
    }
}