using AllOverIt.Assertion;
using AllOverIt.Serialization.Json.Abstractions;
using AllOverIt.Serialization.Json.SystemText.Converters;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Serialization.Json.SystemText
{
    /// <summary>An implementation of <see cref="IJsonSerializer"/> using System.Text.Json.</summary>
    public class SystemTextJsonSerializer : IJsonSerializer
    {
        /// <summary>The serialization options. If no options are provided then a default set will be applied.</summary>
        public JsonSerializerOptions Options { get; }

        /// <summary>Constructor.</summary>
        /// <param name="options">The System.Text serialization options to use. If no options are provided then a default set will be applied.</param>
        public SystemTextJsonSerializer(JsonSerializerOptions options = default)
        {
            Options = options ?? new JsonSerializerOptions();
        }

        /// <inheritdoc />
        public void Configure(JsonSerializerConfiguration configuration)
        {
            _ = configuration.WhenNotNull(nameof(configuration));

            ApplyOptionUseCamelCase(configuration);
            ApplyOptionCaseSensitive(configuration);
            ApplyOptionSupportEnrichedEnums(configuration);
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

        private void ApplyOptionUseCamelCase(JsonSerializerConfiguration configuration)
        {
            if (!configuration.UseCamelCase.HasValue)
            {
                return;
            }

            Options.PropertyNamingPolicy = configuration.UseCamelCase.Value
                ? JsonNamingPolicy.CamelCase
                : null;     // default back to PascalCase
        }

        private void ApplyOptionCaseSensitive(JsonSerializerConfiguration configuration)
        {
            if (!configuration.CaseSensitive.HasValue)
            {
                return;
            }

            Options.PropertyNameCaseInsensitive = !configuration.CaseSensitive.Value;
        }

        private void ApplyOptionSupportEnrichedEnums(JsonSerializerConfiguration configuration)
        {
            if (!configuration.SupportEnrichedEnums.HasValue)
            {
                return;
            }

            var enrichedEnumConverterFactory = Options.Converters.SingleOrDefault(item => item.GetType() == typeof(EnrichedEnumJsonConverterFactory));

            if (configuration.SupportEnrichedEnums.Value)
            {
                if (enrichedEnumConverterFactory == null)
                {
                    Options.Converters.Add(new EnrichedEnumJsonConverterFactory());
                }
            }
            else
            {
                if (enrichedEnumConverterFactory != null)
                {
                    Options.Converters.Remove(enrichedEnumConverterFactory);
                }
            }
        }
    }
}