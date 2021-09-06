using AllOverIt.Serialization.Abstractions;
using System.IO;

namespace AllOverIt.Serialization.SystemTextJson
{
    public sealed class SystemTextJsonSerializer : IJsonSerializer
    {
        //public byte[] SerializeToBytes(GraphqlSubscriptionRequest request)
        //{
        //    var json = System.Text.JsonSerializer.SerializeToUtf8Bytes(request);
        //    return Encoding.UTF8.GetBytes(json);
        //}

        public string SerializeObject<TType>(TType request)
        {
            throw new System.NotImplementedException();
        }

        public byte[] SerializeToBytes<TType>(TType request)
        {
            throw new System.NotImplementedException();
        }

        public TType DeserializeObject<TType>(string value)
        {
            throw new System.NotImplementedException();
        }

        public TType DeserializeObject<TType>(Stream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}