using AllOverIt.Serialization.Binary.Writers;
using AllOverIt.Serialization.Binary.Writers.Extensions;

namespace NamedPipeTypes
{
    public sealed class PipeMessageChildWriter : EnrichedBinaryValueWriter<PipeMessage.ChildClass>
    {
        public override void WriteValue(IEnrichedBinaryWriter writer, object value)
        {
            var child = (PipeMessage.ChildClass) value;

            writer.WriteInt32(child.Value);
        }
    }
}