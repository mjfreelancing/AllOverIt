namespace InterfaceDeserialization
{
    public interface IPerson
    {
        string FirstName { get; }
        string LastName { get; }
        int Age { get; }
        IAddress Address { get; }
    }
}