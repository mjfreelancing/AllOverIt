using System.IO;

namespace AllOverIt.Pagination.TokenEncoding
{
    // Exists only to serve as a contract for continuation token binary / compression streamers
    internal interface IContinuationTokenStreamer
    {
        void SerializeToStream(IContinuationToken continuationToken, Stream stream);
        IContinuationToken DeserializeFromStream(Stream stream);
    }
}
