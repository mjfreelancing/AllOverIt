using AllOverIt.Extensions;
using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Extensions;

internal sealed class ClassroomWriter : EnrichedBinaryTypeWriter<Classroom>
{
    public override void WriteValue(EnrichedBinaryWriter writer, object value)
    {
        var classroom = (Classroom) value;

        writer.WriteGuid(classroom.RoomId);
        writer.WriteObject(classroom.Teacher);

        var students = classroom.Students.AsReadOnlyCollection();

        writer.WriteInt32(students.Count);

        if (students.Count > 0)
        {
            foreach (var student in students)
            {
                writer.WriteObject(student);
            }
        }
    }
}
