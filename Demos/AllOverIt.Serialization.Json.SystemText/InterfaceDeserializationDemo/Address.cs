namespace InterfaceDeserializationDemo
{
    internal sealed class Address : IAddress
    {
        public required string Street { get; set; }
        public required string City { get; set; }
    }
}
