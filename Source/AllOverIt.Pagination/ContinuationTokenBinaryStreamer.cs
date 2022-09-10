using AllOverIt.Assertion;
using AllOverIt.Serialization.Binary;
using System.IO;
using System.Text;

namespace AllOverIt.Pagination
{
    internal sealed class ContinuationTokenBinaryStreamer : IContinuationTokenStreamer
    {
        private static readonly IEnrichedBinaryValueReader _reader = new ContinuationTokenReader();
        private static readonly IEnrichedBinaryValueWriter _writer = new ContinuationTokenWriter();

        public void SerializeToStream(IContinuationToken continuationToken, Stream stream)
        {
            _ = continuationToken.WhenNotNull(nameof(continuationToken));
            _ = stream.WhenNotNull(nameof(stream));

            using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.Writers.Add(_writer);
                writer.WriteObject(continuationToken);
            }
        }

        public IContinuationToken DeserializeFromStream(Stream stream)
        {
            _ = stream.WhenNotNull(nameof(stream));

            using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
            {
                reader.Readers.Add(_reader);

                return (ContinuationToken) reader.ReadObject();
            }
        }
    }
}
