using AllOverIt.Pipes.Named.Serialization;

namespace NamedPipeTypes
{
    public sealed class PipeMessageSerializer : NamedPipeSerializer<PipeMessage>
    {
        public PipeMessageSerializer()
        {
            // This could be greatly simplified by using a single reader/writer as the child object values
            // could be read/written and assigned to a PipeMessage.ChildClass - but this demo shows how to
            // use a custom reader / writer for each object type.
            Readers.Add(new PipeMessageReader());
            Readers.Add(new PipeMessageChildReader());

            Writers.Add(new PipeMessageWriter());
            Writers.Add(new PipeMessageChildWriter());
        }
    }
}