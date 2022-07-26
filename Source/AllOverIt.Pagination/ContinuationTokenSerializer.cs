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
                    using (var compressor = new DeflateStream(stream, CompressionMode.Compress, true))
                    {
                        SerializeToStream(continuationToken, compressor);
                    }
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
                if (usingCompression)
                {
                    using (var decompressor = new DeflateStream(stream, CompressionMode.Decompress, true))
                    {
                        return DeserializeFromStream(decompressor);
                    }
                }
                else
                {
                    return DeserializeFromStream(stream);
                }
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

        public static ContinuationToken DeserializeFromStream(Stream stream)
        {
            using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
            {
                reader.Readers.Add(new ContinuationTokenReader());

                return (ContinuationToken) reader.ReadObject();
            }
        }
    }
}
