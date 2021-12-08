using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AllOverIt.Serialization.NewtonsoftJson.Converters
{
    public class EnumerableInterfaceConverter<TInterface, TConcrete> : JsonConverter
        where TConcrete : class, TInterface
    {
        private static readonly Type InterfaceType = typeof(TInterface);
        private static readonly Type EnumerableInterfaceType = typeof(IEnumerable<>).MakeGenericType(typeof(TInterface));

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == EnumerableInterfaceType && typeToConvert.GenericTypeArguments[0] == InterfaceType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<List<TConcrete>>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}