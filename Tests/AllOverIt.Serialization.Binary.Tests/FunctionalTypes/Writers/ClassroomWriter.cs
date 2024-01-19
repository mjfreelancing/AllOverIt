using AllOverIt.Serialization.Binary.Tests.FunctionalTypes.Models;
using AllOverIt.Serialization.Binary.Writers;
using AllOverIt.Serialization.Binary.Writers.Extensions;

namespace AllOverIt.Serialization.Binary.Tests.FunctionalTypes.Writers
{
    internal sealed class ClassroomWriter : EnrichedBinaryValueWriter<Classroom>
    {
        public override void WriteValue(IEnrichedBinaryWriter writer, object value)
        {
            var classroom = (Classroom) value;

            writer.WriteGuid(classroom.RoomId);
            writer.WriteObject(classroom.Teacher);
            writer.WriteEnumerable(classroom.Students);
        }
    }
}
