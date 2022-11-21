using AllOverIt.Assertion;
using AllOverIt.Serialization.Binary;
using System.IO;
using System.Text;

namespace AllOverIt.Pagination.TokenEncoding
{
    // Uses an EnrichedBinaryWriter to serialize a IContinuationToken to a stream
    internal sealed class ContinuationTokenBinaryStreamer : IContinuationTokenStreamer
    {
        private static readonly IEnrichedBinaryValueReader Reader = new ContinuationTokenReader();
        private static readonly IEnrichedBinaryValueWriter Writer = new ContinuationTokenWriter();

        public void SerializeToStream(IContinuationToken continuationToken, Stream stream)
        {
            _ = continuationToken.WhenNotNull(nameof(continuationToken));
            _ = stream.WhenNotNull(nameof(stream));

            using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.Writers.Add(Writer);
                writer.WriteObject(continuationToken);
            }
        }

        public IContinuationToken DeserializeFromStream(Stream stream)
        {
            _ = stream.WhenNotNull(nameof(stream));

            using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
            {
                reader.Readers.Add(Reader);

                return (ContinuationToken) reader.ReadObject();
            }
        }
    }
}
