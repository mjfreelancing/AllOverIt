namespace BinarySerializationBenchmark.Models
{
    internal abstract class Person
    {
        public required string FirstName { get; init; }
        public string? LastName { get; init; }
        public Gender Gender { get; init; }
        public int? Age { get; init; }
    }
}