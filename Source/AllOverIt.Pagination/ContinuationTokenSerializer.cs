using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Pagination.Exceptions;
using AllOverIt.Serialization.Binary;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AllOverIt.Pagination
{
    internal sealed class ContinuationTokenSerializer : IContinuationTokenSerializer
    {
        private const int HashByteLength = 128 / 8;
        private static Func<HashAlgorithm> HashFactory = () => MD5.Create();

        private readonly IObjectStreamer<IContinuationToken> _tokenStreamer;
        private readonly IObjectStreamer<IContinuationToken> _tokenCompressor;
        private readonly IContinuationTokenOptions _tokenOptions;

        public ContinuationTokenSerializer(IContinuationTokenOptions tokenOptions)
            : this(new ContinuationTokenStreamer(), tokenOptions)
        {

        }

        internal ContinuationTokenSerializer(IObjectStreamer<IContinuationToken> tokenStreamer, IContinuationTokenOptions tokenOptions)
            : this(tokenStreamer, new ContinuationTokenCompressor(tokenStreamer), tokenOptions)
        {
        }

        internal ContinuationTokenSerializer(IObjectStreamer<IContinuationToken> tokenStreamer, IObjectStreamer<IContinuationToken> tokenCompressor,
            IContinuationTokenOptions tokenOptions)
        {
            _tokenStreamer = tokenStreamer.WhenNotNull(nameof(tokenStreamer));
            _tokenCompressor = tokenCompressor.WhenNotNull(nameof(tokenCompressor));
            _tokenOptions = tokenOptions.WhenNotNull(nameof(tokenOptions));
        }

        public bool IsValidToken(string continuationToken)
        {
            _ = continuationToken.WhenNotNull(nameof(continuationToken));

            return TryDeserialize(continuationToken, out _);
        }

        public string Serialize(IContinuationToken continuationToken)
        {
            _ = continuationToken.WhenNotNull(nameof(continuationToken));

            using (var stream = new MemoryStream())
            {
                if (_tokenOptions.UseCompression)
                {
                    _tokenCompressor.SerializeToStream(continuationToken, stream);
                }
                else
                {
                    _tokenStreamer.SerializeToStream(continuationToken, stream);
                }               

                var bytes = stream.ToArray();

                if (_tokenOptions.IncludeHash)
                {
                    using (var hashAlgorithm = HashFactory.Invoke())
                    {
                        var hash = hashAlgorithm.ComputeHash(bytes);
                        bytes = hash.Concat(bytes).ToArray();
                    }
                }

                return Convert.ToBase64String(bytes);
            }
        }

        public IContinuationToken Deserialize(string continuationToken)
        {
            if (continuationToken.IsNullOrEmpty())
            {
                return ContinuationToken.None;
            }

            // Decompresses the stream if compression was used
            if (!TryDeserialize(continuationToken, out var token))
            {
                throw new PaginationException("Invalid continuation token. Hash value mismatch.");
            }

            return token;
        }

        internal bool TryDeserialize(string continuationToken, out IContinuationToken token)
        {
            _ = continuationToken.WhenNotNull(nameof(continuationToken));

            token = default;

            var buffer = new Span<byte>(new byte[continuationToken.Length]);

            if (!Convert.TryFromBase64String(continuationToken, buffer, out var byteCount))
            {
                return false;
            }

            var bytes = buffer[..byteCount].ToArray();

            if (_tokenOptions.IncludeHash)
            {
                // Must have at least the hash value bytes + 1 byte of content
                if (bytes.Length < HashByteLength + 1)
                {
                    return false;
                }

                var hashBytes = bytes[..HashByteLength];
                var contentBytes = bytes[HashByteLength..];

                using (var hashAlgorithm = HashFactory.Invoke())
                {
                    var contentHash = hashAlgorithm.ComputeHash(contentBytes);

                    if (!contentHash.SequenceEqual(hashBytes))
                    {
                        return false;
                    }
                }

                bytes = contentBytes;
            }

            using (var stream = new MemoryStream(bytes))
            {
                token = _tokenOptions.UseCompression
                    ? _tokenCompressor.DeserializeFromStream(stream)
                    : _tokenStreamer.DeserializeFromStream(stream);
            }

            return true;
        }
    }





    public interface IObjectStreamer<TType>
    {
        void SerializeToStream(TType @object, Stream stream);
        TType DeserializeFromStream(Stream stream);
    }


    internal sealed class ContinuationTokenStreamer : IObjectStreamer<IContinuationToken>
    {
        public void SerializeToStream(IContinuationToken continuationToken, Stream stream)
        {
            using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.Writers.Add(new ContinuationTokenWriter());

                writer.WriteObject(continuationToken);
            }
        }

        public IContinuationToken DeserializeFromStream(Stream stream)
        {
            using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
            {
                reader.Readers.Add(new ContinuationTokenReader());

                return (ContinuationToken) reader.ReadObject();
            }
        }
    }



    internal sealed class ContinuationTokenCompressor : IObjectStreamer<IContinuationToken>
    {
        private readonly IObjectStreamer<IContinuationToken> _tokenStreamer;

        public ContinuationTokenCompressor(IObjectStreamer<IContinuationToken> tokenStreamer)
        {
            _tokenStreamer = tokenStreamer.WhenNotNull(nameof(tokenStreamer));
        }

        public void SerializeToStream(IContinuationToken continuationToken, Stream stream)
        {
            using (var compressor = new DeflateStream(stream, CompressionMode.Compress, true))
            {
                _tokenStreamer.SerializeToStream(continuationToken, compressor);
            }
        }

        public IContinuationToken DeserializeFromStream(Stream stream)
        {
            using (var decompressor = new DeflateStream(stream, CompressionMode.Decompress, true))
            {
                return _tokenStreamer.DeserializeFromStream(decompressor);
            }
        }
    }
}
