using AllOverIt.Serialization.Abstractions;
using AllOverIt.Serialization.JsonHelper;
using AllOverIt.Serialization.NewtonsoftJson.Converters;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace AllOverIt.Serialization.NewtonsoftJson
{
    /// <inheritdoc />
    public sealed class JsonHelper : JsonHelperBase
    {
        private static readonly Type ConverterType = typeof(NestedDictionaryConverter);

        /// <inheritdoc />
        public JsonHelper(object value, JsonSerializerSettings settings = null)
            : base(value, CreateJsonSerializer(settings))
        {
        }

        /// <inheritdoc />
        public JsonHelper(string value, JsonSerializerSettings settings = null)
            : base(value, CreateJsonSerializer(settings))
        {
        }

        private static IJsonSerializer CreateJsonSerializer(JsonSerializerSettings settings)
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