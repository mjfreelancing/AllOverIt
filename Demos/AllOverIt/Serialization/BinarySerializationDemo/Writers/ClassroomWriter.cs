using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Extensions;

internal sealed class ClassroomWriter : EnrichedBinaryValueWriter<Classroom>
{
    public override void WriteValue(IEnrichedBinaryWriter writer, object value)
    {
        var classroom = (Classroom) value;

        writer.WriteGuid(classroom.RoomId);         // Writes the Guid without a type prefix
        writer.WriteObject(classroom.Teacher);
        writer.WriteObject(classroom.Students);     // This is an Enumerable
    }
}
