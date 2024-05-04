using AllOverIt.Assertion;
using System.IO.Compression;

namespace AllOverIt.Pagination.TokenEncoding
{
    // Decorates another IContinuationTokenStreamer to compress a stream containing the binary version of a IContinuationTokenStreamer
    internal sealed class ContinuationTokenCompressor : IContinuationTokenStreamer
    {
        private readonly IContinuationTokenStreamer _tokenStreamer;

        public ContinuationTokenCompressor(IContinuationTokenStreamer tokenStreamer)
        {
            _tokenStreamer = tokenStreamer.WhenNotNull(nameof(tokenStreamer));
        }

        public void SerializeToStream(IContinuationToken continuationToken, Stream stream)
        {
            _ = continuationToken.WhenNotNull(nameof(continuationToken));
            _ = stream.WhenNotNull(nameof(stream));

            using (var compressor = new DeflateStream(stream, CompressionMode.Compress, true))
            {
                _tokenStreamer.SerializeToStream(continuationToken, compressor);
            }
        }

        public IContinuationToken DeserializeFromStream(Stream stream)
        {
            _ = stream.WhenNotNull(nameof(stream));

            using (var decompressor = new DeflateStream(stream, CompressionMode.Decompress, true))
            {
                return _tokenStreamer.DeserializeFromStream(decompressor);
            }
        }
    }
}
