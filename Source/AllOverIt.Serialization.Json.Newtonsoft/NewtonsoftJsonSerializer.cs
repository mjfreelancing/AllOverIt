using AllOverIt.Assertion;
using AllOverIt.Serialization.Json.Abstractions;
using AllOverIt.Serialization.Json.Abstractions.Exceptions;
using AllOverIt.Serialization.Json.Newtonsoft.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace AllOverIt.Serialization.Json.Newtonsoft
{
    /// <summary>An implementation of <see cref="IJsonSerializer"/> using Newtonsoft.Json.</summary>
    public class NewtonsoftJsonSerializer : IJsonSerializer
    {
        /// <summary>The serialization options. If no settings are provided then a default set will be applied.</summary>
        public JsonSerializerSettings Settings { get; }

        /// <summary>Constructor. Uses a default constructed JsonSerializerSettings.</summary>
        public NewtonsoftJsonSerializer()
            : this(new JsonSerializerSettings())
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="settings">The Newtonsoft serialization settings to use..</param>
        public NewtonsoftJsonSerializer(JsonSerializerSettings settings = default)
        {
            Settings = settings ?? new JsonSerializerSettings();
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

        private void ApplyOptionUseCamelCase(JsonSerializerConfiguration configuration)
        {
            if (!configuration.UseCamelCase.HasValue)
            {
                return;
            }

            Settings.ContractResolver = configuration.UseCamelCase.Value
                ? new CamelCasePropertyNamesContractResolver()
                : null;     // default back to PascalCase
        }

        private static void ApplyOptionCaseSensitive(JsonSerializerConfiguration configuration)
        {
            if (!configuration.CaseSensitive.HasValue)
            {
                return;
            }

            if (configuration.CaseSensitive.Value)
            {
                throw new SerializerConfigurationException("Newtonsoft requires a custom converter to support case sensitivity.");
            }
        }

        private void ApplyOptionSupportEnrichedEnums(JsonSerializerConfiguration configuration)
        {
            if (!configuration.SupportEnrichedEnums.HasValue)
            {
                return;
            }

            var enrichedEnumConverterFactory = Settings.Converters.SingleOrDefault(item => item.GetType() == typeof(EnrichedEnumJsonConverterFactory));

            if (configuration.SupportEnrichedEnums.Value)
            {
                if (enrichedEnumConverterFactory == null)
                {
                    Settings.Converters.Add(new EnrichedEnumJsonConverterFactory());
                }
            }
            else
            {
                if (enrichedEnumConverterFactory != null)
                {
                    Settings.Converters.Remove(enrichedEnumConverterFactory);
                }
            }
        }
    }
}