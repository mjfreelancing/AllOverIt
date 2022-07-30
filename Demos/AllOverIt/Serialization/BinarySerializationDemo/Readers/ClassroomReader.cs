using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Extensions;

internal sealed class ClassroomReader : EnrichedBinaryValueReader<Classroom>
{
    public override object ReadValue(IEnrichedBinaryReader reader)
    {
        var roomId = reader.ReadGuid();
        var teacher = reader.ReadObject<Teacher>();
        var students = reader.ReadEnumerable<Student>();

        return new Classroom
        {
            RoomId = roomId,
            Teacher = teacher,
            Students = students
        };
    }
}
