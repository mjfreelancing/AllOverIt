using AllOverIt.Extensions;
using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace AllOverIt.Pagination
{
    internal static class ContinuationTokenSerializer
    {
        private abstract class TokenCompression : IDisposable
        {
            private readonly bool _disposeStream;
            public Stream Stream { get; }

            public TokenCompression(Stream stream, CompressionMode compressionMode, bool usingCompression)
            {
                Stream = usingCompression
                    ? new DeflateStream(stream, compressionMode, true)
                    : stream;

                _disposeStream = usingCompression;
            }

            public void Dispose()
            {
                if (_disposeStream)
                {
                    Stream.Dispose();
                }

                GC.SuppressFinalize(this);
            }
        }

        private sealed class TokenCompressor : TokenCompression
        {
            public TokenCompressor(Stream stream, bool usingCompression)
                : base(stream, CompressionMode.Compress, usingCompression)
            {
            }
        }

        private sealed class TokenDecompressor : TokenCompression
        {
            public TokenDecompressor(Stream stream, bool usingCompression)
                : base(stream, CompressionMode.Decompress, usingCompression)
            {
            }
        }

        public static string Serialize(ContinuationToken continuationToken, bool usingCompression = false)
        {
            using (var stream = new MemoryStream())
            {
                using (var compressor = new TokenCompressor(stream, usingCompression))
                {
                    SerializeToStream(continuationToken, compressor.Stream);
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
                using (var compressor = new TokenDecompressor(stream, usingCompression))
                {
                    using (var reader = new EnrichedBinaryReader(compressor.Stream))
                    {
                        return DeserializeFromStream(compressor.Stream);
                    }
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




    internal sealed class ContinuationTokenWriter : EnrichedBinaryValueWriter<ContinuationToken>
    {
        public override void WriteValue(EnrichedBinaryWriter writer, object value)
        {
            var continuationToken = (ContinuationToken) value;

            writer.WriteByte((byte) continuationToken.Direction);

            // Number of values - surely only a handful of values
            var valueCount = continuationToken.Values?.Count ?? 0;
            writer.WriteByte((byte) valueCount);

            if (valueCount > 0)
            {
                foreach (var tokenValue in continuationToken.Values)
                {
                    // Write the type and value for the token so they can be read back appropriately
                    writer.WriteObject(tokenValue);
                }
            }
        }
    }



    internal sealed class ContinuationTokenReader : EnrichedBinaryValueReader<ContinuationToken>
    {
        public override object ReadValue(EnrichedBinaryReader reader)
        {
            var direction = (PaginationDirection) reader.ReadByte();
            var valueCount = (int) reader.ReadByte();

            if (valueCount == 0)
            {
                return new ContinuationToken
                {
                    Direction = direction
                };
            }

            var values = new List<object>();

            for (var i = 0; i < valueCount; i++)
            {
                var value = reader.ReadObject();
                values.Add(value);
            }

            return new ContinuationToken
            {
                Direction = direction,
                Values = values
            };
        }
    }

}
