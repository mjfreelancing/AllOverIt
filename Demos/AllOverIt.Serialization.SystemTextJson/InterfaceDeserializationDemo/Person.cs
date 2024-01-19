namespace InterfaceDeserializationDemo
{
    internal class Person : IPerson
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public IAddress Address { get; set; }
    }
}
