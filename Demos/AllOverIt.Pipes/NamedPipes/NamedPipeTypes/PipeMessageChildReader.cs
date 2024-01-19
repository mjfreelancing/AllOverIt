using AllOverIt.Serialization.Binary.Readers;

namespace NamedPipeTypes
{
    public sealed class PipeMessageChildReader : EnrichedBinaryValueReader<PipeMessage.ChildClass>
    {
        public override object ReadValue(IEnrichedBinaryReader reader)
        {
            var value = reader.ReadInt32();

            return new PipeMessage.ChildClass
            {
                Value = value
            };
        }
    }
}