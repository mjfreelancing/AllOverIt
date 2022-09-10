using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Pagination.Exceptions;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace AllOverIt.Pagination.TokenEncoding
{
    internal sealed class ContinuationTokenSerializer : IContinuationTokenSerializer
    {
        private const int HashByteLength = 128 / 8;
        private static readonly Func<HashAlgorithm> HashFactory = () => MD5.Create();

        private readonly IContinuationTokenStreamer _tokenStreamer;
        private readonly IContinuationTokenOptions _tokenOptions;

        public ContinuationTokenSerializer(IContinuationTokenOptions tokenOptions)
            : this(new ContinuationTokenBinaryStreamer(), tokenOptions)
        {
        }

        internal ContinuationTokenSerializer(IContinuationTokenStreamer tokenStreamer, IContinuationTokenOptions tokenOptions)
        {
            _tokenStreamer = tokenStreamer.WhenNotNull(nameof(tokenStreamer));
            _tokenOptions = tokenOptions.WhenNotNull(nameof(tokenOptions));

            if (_tokenOptions.UseCompression)
            {
                // Decorate the binary streamer with compression
                _tokenStreamer = new ContinuationTokenCompressor(tokenStreamer);
            }
        }       

        public string Serialize(IContinuationToken continuationToken)
        {
            _ = continuationToken.WhenNotNull(nameof(continuationToken));

            using (var stream = new MemoryStream())
            {
                _tokenStreamer.SerializeToStream(continuationToken, stream);

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

        public bool TryDeserialize(string continuationToken, out IContinuationToken token)
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
                token = _tokenStreamer.DeserializeFromStream(stream);
            }

            return true;
        }
    }
}
