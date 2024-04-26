namespace BinarySerializationBenchmarking.Models
{
    internal sealed class Classroom
    {
        public Guid RoomId { get; init; }
        public Teacher Teacher { get; init; }
        public IEnumerable<Student> Students { get; init; }
    }
}