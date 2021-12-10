using AllOverIt.Serialization.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Serialization.NewtonsoftJson
{
    /// <summary>An implementation of <see cref="IJsonSerializer"/> using Newtonsoft.Json.</summary>
    public sealed class NewtonsoftJsonSerializer : IJsonSerializer
    {
        /// <summary>The serialization options. If no settings are provided then a default set will be applied.</summary>
        public JsonSerializerSettings Settings { get; }

        /// <summary>Constructor.</summary>
        /// <param name="settings">The serialization settings to use. If no settings are provided then a default set will be applied.</param>
        public NewtonsoftJsonSerializer(JsonSerializerSettings settings = default)
        {
            Settings = settings ?? CreateDefaultSettings();
        }

        /// <inheritdoc />
        public string SerializeObject<TType>(TType value)
        {
            return JsonConvert.SerializeObject(value, Settings);
        }

        /// <inheritdoc />
        public byte[] SerializeToUtf8Bytes<TType>(TType value)
        {
            var json = SerializeObject(value);
            return Encoding.UTF8.GetBytes(json);
        }

        /// <inheritdoc />
        public TType DeserializeObject<TType>(string value)
        {
            return JsonConvert.DeserializeObject<TType>(value, Settings);
        }

        /// <inheritdoc />
        public Task<TType> DeserializeObjectAsync<TType>(Stream stream, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var sr = new StreamReader(stream))
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    var serializer = JsonSerializer.Create(Settings);
                    var result = serializer.Deserialize<TType>(reader);
                    return Task.FromResult(result);
                }
            }
        }

        private static JsonSerializerSettings CreateDefaultSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
        }
    }
}