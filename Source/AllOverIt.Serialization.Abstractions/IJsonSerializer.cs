using System.IO;

namespace AllOverIt.Serialization.Abstractions
{
    public interface IJsonSerializer
    {
        string SerializeObject<TType>(TType request);
        byte[] SerializeToBytes<TType>(TType request);
        TType DeserializeObject<TType>(string value);
        TType DeserializeObject<TType>(Stream stream);
    }
}