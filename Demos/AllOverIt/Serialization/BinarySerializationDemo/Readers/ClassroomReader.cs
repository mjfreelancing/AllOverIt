using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Extensions;
using System.Collections.Generic;

internal sealed class ClassroomReader : EnrichedBinaryTypeReader<Classroom>
{
    public override object ReadValue(EnrichedBinaryReader reader)
    {
        var roomId = reader.ReadGuid();
        var teacher = reader.ReadObject<Teacher>();
        var studentCount = reader.ReadInt32();

        var students = new List<Student>();

        for (var i = 0; i < studentCount; i++)
        {
            var student = reader.ReadObject<Student>();
            students.Add(student);
        }

        return new Classroom
        {
            RoomId = roomId,
            Teacher = teacher,
            Students = students
        };
    }
}
