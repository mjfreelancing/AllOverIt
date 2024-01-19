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
            var child = reader.ReadObject<PipeMessage.ChildClass>();

            return new PipeMessage
            {
                Id = id,
                Text = text,
                Child = child
            };
        }
    }
}