namespace BinarySerializationDemo.Models
{
    internal sealed class Classroom
    {
        public Guid RoomId { get; init; }
        public required Teacher Teacher { get; init; }
        public IEnumerable<Student> Students { get; init; } = [];
    }
}