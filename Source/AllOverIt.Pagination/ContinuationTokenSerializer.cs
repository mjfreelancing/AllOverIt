using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Pagination.Exceptions;
using AllOverIt.Serialization.Binary;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AllOverIt.Pagination
{
    internal static class ContinuationTokenSerializer
    {
        private static IDictionary<ContinuationTokenHashMode, int> _hashAlgorithmBytes = new Dictionary<ContinuationTokenHashMode, int>
        {
            { ContinuationTokenHashMode.Sha1, 160 / 8 },
            { ContinuationTokenHashMode.Sha256, 256 / 8 },
            { ContinuationTokenHashMode.Sha384, 384 / 8 },
            { ContinuationTokenHashMode.Sha512, 512 / 8 },
        };

        private static IDictionary<ContinuationTokenHashMode, Lazy<HashAlgorithm>> _hashAlgorithms = new Dictionary<ContinuationTokenHashMode, Lazy<HashAlgorithm>>
        {
            { ContinuationTokenHashMode.Sha1, new Lazy<HashAlgorithm>(() => SHA1.Create()) },
            { ContinuationTokenHashMode.Sha256, new Lazy<HashAlgorithm>(() => SHA256.Create()) },
            { ContinuationTokenHashMode.Sha384, new Lazy<HashAlgorithm>(() => SHA384.Create()) },
            { ContinuationTokenHashMode.Sha512, new Lazy<HashAlgorithm>(() => SHA512.Create()) },
        };

        public static bool TryDecodeTokenBytes(string continuationToken, IContinuationTokenOptions options, out byte[] continuationTokenBytes)
        {
            continuationTokenBytes = default;

            var buffer = new Span<byte>(new byte[continuationToken.Length]);

            if (!Convert.TryFromBase64String(continuationToken, buffer, out var byteCount))
            {
                return false;
            }

            var bytes = buffer[..byteCount].ToArray();

            if (options.HashMode != ContinuationTokenHashMode.None)
            {
                var hashByteLength = _hashAlgorithmBytes[options.HashMode];

                var hashBytes = bytes[..hashByteLength];
                var contentBytes = bytes[hashByteLength..];

                var hashAlgorithm = _hashAlgorithms[options.HashMode].Value;
                var contentHash = hashAlgorithm.ComputeHash(contentBytes);

                if (!contentHash.SequenceEqual(hashBytes))
                {
                    return false;
                }

                bytes = contentBytes;
            }

            continuationTokenBytes = bytes;
            return true;
        }

        public static string Serialize(ContinuationToken continuationToken, IContinuationTokenOptions options)
        {
            _ = continuationToken.WhenNotNull(nameof(continuationToken));
            _ = options.WhenNotNull(nameof(options));

            using (var stream = new MemoryStream())
            {
                if (options.UseCompression)
                {
                    SerializeToStreamWithDeflate(continuationToken, stream);
                }
                else
                {
                    SerializeToStream(continuationToken, stream);
                }               

                var bytes = stream.ToArray();

                if (options.HashMode != ContinuationTokenHashMode.None)
                {
                    var hashAlgorithm = _hashAlgorithms[options.HashMode].Value;
                    var hash = hashAlgorithm.ComputeHash(bytes);

                    bytes = hash.Concat(bytes).ToArray();
                }

                return Convert.ToBase64String(bytes);
            }
        }

        public static ContinuationToken Deserialize(string continuationToken, IContinuationTokenOptions options)
        {
            if (continuationToken.IsNullOrEmpty())
            {
                return ContinuationToken.None;
            }

            _ = options.WhenNotNull(nameof(options));

            if (!TryDecodeTokenBytes(continuationToken, options, out var bytes))
            {
                throw new PaginationException("Invalid continuation token. Hash value mismatch.");
            }

            using (var stream = new MemoryStream(bytes))
            {
                return options.UseCompression
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
