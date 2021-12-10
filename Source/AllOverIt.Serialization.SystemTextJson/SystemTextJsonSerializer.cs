using AllOverIt.Serialization.Abstractions;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Serialization.SystemTextJson
{
    /// <summary>An implementation of <see cref="IJsonSerializer"/> using System.Text.Json.</summary>
    public sealed class SystemTextJsonSerializer : IJsonSerializer
    {
        /// <summary>The serialization options. If no options are provided then a default set will be applied.</summary>
        public JsonSerializerOptions Options { get; }

        /// <summary>Constructor.</summary>
        /// <param name="options">The serialization options to use. If no options are provided then a default set will be applied.</param>
        public SystemTextJsonSerializer(JsonSerializerOptions options = default)
        {
            Options = options ?? CreateDefaultOptions();
        }

        /// <inheritdoc />
        public string SerializeObject<TType>(TType value)
        {
            return JsonSerializer.Serialize(value, Options);
        }

        /// <inheritdoc />
        public byte[] SerializeToUtf8Bytes<TType>(TType value)
        {
            return JsonSerializer.SerializeToUtf8Bytes(value, Options);
        }

        /// <inheritdoc />
        public TType DeserializeObject<TType>(string value)
        {
            return JsonSerializer.Deserialize<TType>(value, Options);
        }

        /// <inheritdoc />
        public Task<TType> DeserializeObjectAsync<TType>(Stream stream, CancellationToken cancellationToken)
        {
            return JsonSerializer
                .DeserializeAsync<TType>(stream, Options, cancellationToken)
                .AsTask();
        }

        private static JsonSerializerOptions CreateDefaultOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
        }
    }
}