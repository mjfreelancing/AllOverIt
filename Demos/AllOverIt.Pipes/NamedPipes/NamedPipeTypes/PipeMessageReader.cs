using AllOverIt.Serialization.Binary.Readers;
using AllOverIt.Serialization.Binary.Readers.Extensions;

namespace NamedPipeTypes
{
    public sealed class PipeMessageReader : EnrichedBinaryValueReader<PipeMessage>
    {
        public override object ReadValue(IEnrichedBinaryReader reader)
        {
            var id = reader.ReadGuid();
            var text = reader.ReadSafeString();
            var pingBack = reader.ReadBoolean();
            var child = reader.ReadObject<PipeMessage.ChildClass>()!;    // not doing anything with this, just demonstrating objects can be serialized

            return new PipeMessage
            {
                Id = id,
                Text = text,
                PingBack = pingBack,
                Child = child
            };
        }
    }
}