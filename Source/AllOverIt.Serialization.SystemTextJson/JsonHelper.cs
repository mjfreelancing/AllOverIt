using AllOverIt.Serialization.Abstractions;
using AllOverIt.Serialization.JsonHelper;
using AllOverIt.Serialization.SystemTextJson.Converters;
using System;
using System.Linq;
using System.Text.Json;

namespace AllOverIt.Serialization.SystemTextJson
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

        private static IJsonSerializer CreateJsonSerializer(JsonSerializerOptions options)
        {
            options ??= new JsonSerializerOptions();

            if (options.Converters.All(converter => converter.GetType() != ConverterType))
            {
                options.Converters.Add(new NestedDictionaryConverter());
            }

            return new SystemTextJsonSerializer(options);
        }
    }
}