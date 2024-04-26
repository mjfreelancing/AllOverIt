using AllOverIt.Serialization.Json.Abstractions;
using AllOverIt.Serialization.Json.Newtonsoft.Converters;
using Newtonsoft.Json;

namespace AllOverIt.Serialization.Json.Newtonsoft
{
    /// <inheritdoc />
    /// <remarks>If serialization settings are not provided then a default constructed <see cref="NestedDictionaryConverter"/> will be added
    /// to the list of converters. You can change the behaviour of this converter type by adding a suitably configured converter to the
    /// <see cref="JsonSerializerSettings"/> instance.</remarks>
    public sealed class JsonHelper : JsonHelperBase
    {
        private static readonly Type ConverterType = typeof(NestedDictionaryConverter);

        /// <inheritdoc />
        public JsonHelper(object value, JsonSerializerSettings settings = default)
            : base(value, CreateJsonSerializer(settings))
        {
        }

        /// <inheritdoc />
        public JsonHelper(string value, JsonSerializerSettings settings = default)
            : base(value, CreateJsonSerializer(settings))
        {
        }

        private static NewtonsoftJsonSerializer CreateJsonSerializer(JsonSerializerSettings settings)
        {
            settings ??= new JsonSerializerSettings();

            if (settings.Converters.All(converter => converter.GetType() != ConverterType))
            {
                settings.Converters.Add(new NestedDictionaryConverter());
            }

            return new NewtonsoftJsonSerializer(settings);
        }
    }
}