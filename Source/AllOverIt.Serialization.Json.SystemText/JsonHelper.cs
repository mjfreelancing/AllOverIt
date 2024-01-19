using AllOverIt.Serialization.Json.Abstractions;
using AllOverIt.Serialization.Json.SystemText.Converters;
using System;
using System.Linq;
using System.Text.Json;

namespace AllOverIt.Serialization.Json.SystemText
{
    /// <inheritdoc />
    /// <remarks>If serialization settings are not provided then a default constructed <see cref="NestedDictionaryConverter"/> will be added
    /// to the list of converters. You can change the behaviour of this converter type by adding a suitably configured converter to the
    /// <see cref="JsonSerializerOptions"/> instance.</remarks>
    public sealed class JsonHelper : JsonHelperBase
    {
        private static readonly Type ConverterType = typeof(NestedDictionaryConverter);

        /// <inheritdoc />
        public JsonHelper(object value, JsonSerializerOptions options = default)
            : base(value, CreateJsonSerializer(options))
        {
        }

        /// <inheritdoc />
        public JsonHelper(string value, JsonSerializerOptions options = default)
            : base(value, CreateJsonSerializer(options))
        {
        }

        private static SystemTextJsonSerializer CreateJsonSerializer(JsonSerializerOptions options)
        {
            options ??= new JsonSerializerOptions();

            if (options.Converters.All(converter => converter.GetType() != ConverterType))
            {
                // Uses default options (deserialize floating values as double, rather than decimal)
                options.Converters.Add(new NestedDictionaryConverter());
            }

            return new SystemTextJsonSerializer(options);
        }
    }
}