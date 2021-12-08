using Newtonsoft.Json;
using System;

namespace AllOverIt.Serialization.NewtonsoftJson.Converters
{
    public class InterfaceConverter<TInterface, TConcrete> : JsonConverter
        where TConcrete : class, TInterface
    {
        private static readonly Type InterfaceType = typeof(TInterface);

        public override bool CanConvert(Type objectType)
        {
            return objectType == InterfaceType;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<TConcrete>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}