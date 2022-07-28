using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Serialization.Binary;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace AllOverIt.Pagination
{
    internal static class ContinuationTokenSerializer
    {
        public static string Serialize(ContinuationToken continuationToken, bool usingCompression = false)
        {
            _ = continuationToken.WhenNotNull(nameof(continuationToken));

            using (var stream = new MemoryStream())
            {
                if (usingCompression)
                {
                    SerializeToStreamWithDeflate(continuationToken, stream);
                }
                else
                {
                    SerializeToStream(continuationToken, stream);
                }               

                var bytes = stream.ToArray();

                return Convert.ToBase64String(bytes);
            }
        }

        public static ContinuationToken Deserialize(string continuationToken, bool usingCompression = false)
        {
            if (continuationToken.IsNullOrEmpty())
            {
                return ContinuationToken.None;
            }

            var bytes = Convert.FromBase64String(continuationToken);

            using (var stream = new MemoryStream(bytes))
            {
                return usingCompression
                    ? DeserializeFromStreamWithInflate(stream) 
                    : DeserializeFromStream(stream);
            }
        }

        private static void SerializeToStream(ContinuationToken continuationToken, Stream stream)
        {
            using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.Writers.Add(new ContinuationTokenWriter());

                writer.WriteObject(continuationToken);
            }
        }

        private static ContinuationToken DeserializeFromStream(Stream stream)
        {
            using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
            {
                reader.Readers.Add(new ContinuationTokenReader());

                return (ContinuationToken) reader.ReadObject();
            }
        }

        private static void SerializeToStreamWithDeflate(ContinuationToken continuationToken, Stream stream)
        {
            using (var compressor = new DeflateStream(stream, CompressionMode.Compress, true))
            {
                SerializeToStream(continuationToken, compressor);
            }
        }

        private static ContinuationToken DeserializeFromStreamWithInflate(Stream stream)
        {
            using (var decompressor = new DeflateStream(stream, CompressionMode.Decompress, true))
            {
                return DeserializeFromStream(decompressor);
            }
        }
    }
}
