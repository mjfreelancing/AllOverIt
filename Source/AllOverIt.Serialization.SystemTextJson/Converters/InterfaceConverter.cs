using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AllOverIt.Serialization.SystemTextJson.Converters
{
    public class InterfaceConverter<TInterface, TConcrete> : JsonConverter<TInterface>
        where TConcrete : class, TInterface
    {
        private static readonly Type InterfaceType = typeof(TInterface);

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == InterfaceType;
        }

        public override TInterface Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<TConcrete>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, TInterface value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}