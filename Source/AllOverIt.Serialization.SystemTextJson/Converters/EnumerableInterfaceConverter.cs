using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AllOverIt.Serialization.SystemTextJson.Converters
{  
    public class EnumerableInterfaceConverter<TInterface, TConcrete> : JsonConverter<IEnumerable<TConcrete>>
        where TConcrete : class, TInterface
    {
        private static readonly Type InterfaceType = typeof(TInterface);
        private static readonly Type EnumerableInterfaceType = typeof(IEnumerable<>).MakeGenericType(typeof(TInterface));

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == EnumerableInterfaceType && typeToConvert.GenericTypeArguments[0] == InterfaceType;
        }

        public override IEnumerable<TConcrete> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<List<TConcrete>>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, IEnumerable<TConcrete> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}