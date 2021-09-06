using AllOverIt.Serialization.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Text;

namespace AllOverIt.Serialization.Newtonsoft
{
    public sealed class NewtonsoftJsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings _defaultSerializerSettings = new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };

        private readonly JsonSerializerSettings _serializerSettings;

        public NewtonsoftJsonSerializer(JsonSerializerSettings serializerSettings = default)
        {
            _serializerSettings = serializerSettings ?? _defaultSerializerSettings;
        }

        public string SerializeObject<TMessage>(TMessage message)
        {
            return JsonConvert.SerializeObject(message, _serializerSettings);
        }

        public byte[] SerializeToBytes<TMessage>(TMessage message)
        {
            var json = SerializeObject(message);
            return Encoding.UTF8.GetBytes(json);
        }

        public TType DeserializeObject<TType>(string value)
        {
            return JsonConvert.DeserializeObject<TType>(value, _serializerSettings);
        }

        public TType DeserializeObject<TType>(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    var serializer = JsonSerializer.Create(_serializerSettings);
                    var result = serializer.Deserialize<TType>(reader);
                    return result;
                }
            }
        }
    }
}