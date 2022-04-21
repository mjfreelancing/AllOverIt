using AllOverIt.Serialization.Abstractions;
using AllOverIt.Serialization.JsonHelper;
using AllOverIt.Serialization.NewtonsoftJson.Converters;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace AllOverIt.Serialization.NewtonsoftJson
{
    public sealed class JsonHelper : JsonHelperBase
    {
        private static Type DictionaryConverterType = typeof(DictionaryConverter);

        public JsonHelper(object value, JsonSerializerSettings settings = null)
            : base(value, CreateJsonSerializer(settings))
        {
        }

        public JsonHelper(string value, JsonSerializerSettings settings = null)
            : base(value, CreateJsonSerializer(settings))
        {
        }

        private static IJsonSerializer CreateJsonSerializer(JsonSerializerSettings settings)
        {
            settings ??= new JsonSerializerSettings();

            if (!settings.Converters.Any(converter => converter.GetType() == DictionaryConverterType))
            {
                settings.Converters.Add(new DictionaryConverter());
            }

            return new NewtonsoftJsonSerializer(settings);
        }
    }
}