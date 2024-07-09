namespace BinarySerializationBenchmark.Models
{
    internal sealed class Classroom
    {
        public required Guid RoomId { get; init; }
        public required Teacher Teacher { get; init; }
        public IEnumerable<Student> Students { get; init; } = [];
    }
}