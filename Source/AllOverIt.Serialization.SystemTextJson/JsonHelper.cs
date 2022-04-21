using AllOverIt.Serialization.Abstractions;
using AllOverIt.Serialization.JsonHelper;
using AllOverIt.Serialization.SystemTextJson.Converters;
using System;
using System.Linq;
using System.Text.Json;

namespace AllOverIt.Serialization.SystemTextJson
{
    public sealed class JsonHelper : JsonHelperBase
    {
        private static Type DictionaryConverterType = typeof(DictionaryConverter);

        public JsonHelper(object value, JsonSerializerOptions options = null)
            : base(value, CreateJsonSerializer(options))
        {
        }

        public JsonHelper(string value, JsonSerializerOptions options = null)
            : base(value, CreateJsonSerializer(options))
        {
        }

        private static IJsonSerializer CreateJsonSerializer(JsonSerializerOptions options)
        {
            options ??= new JsonSerializerOptions();

            if (!options.Converters.Any(converter => converter.GetType() == DictionaryConverterType))
            {
                options.Converters.Add(new DictionaryConverter());
            }

            return new SystemTextJsonSerializer(options);
        }
    }
}