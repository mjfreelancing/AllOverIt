using System.IO;

namespace AllOverIt.Pagination.TokenEncoding
{
    internal interface IContinuationTokenStreamer
    {
        void SerializeToStream(IContinuationToken continuationToken, Stream stream);
        IContinuationToken DeserializeFromStream(Stream stream);
    }
}
