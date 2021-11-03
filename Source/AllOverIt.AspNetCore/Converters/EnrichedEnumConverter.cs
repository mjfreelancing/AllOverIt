using AllOverIt.Patterns.Enumeration;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AllOverIt.AspNetCore.Converters
{
    public abstract class EnrichedEnumConverter<TEnum> : JsonConverter<TEnum>
        where TEnum : EnrichedEnum<TEnum>
    {
        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return EnrichedEnum<TEnum>.From(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Name);
        }
    }
}