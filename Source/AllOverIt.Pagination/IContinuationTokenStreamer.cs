using System.IO;

namespace AllOverIt.Pagination
{
    internal interface IContinuationTokenStreamer
    {
        void SerializeToStream(IContinuationToken continuationToken, Stream stream);
        IContinuationToken DeserializeFromStream(Stream stream);
    }
}
