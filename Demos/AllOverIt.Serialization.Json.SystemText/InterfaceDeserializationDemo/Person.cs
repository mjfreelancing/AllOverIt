namespace InterfaceDeserializationDemo
{
    internal class Person : IPerson
    {
        public required string FirstName { get; set; }
        public string LastName { get; set; }
        public required int Age { get; set; }
        public required IAddress Address { get; set; }
    }
}
