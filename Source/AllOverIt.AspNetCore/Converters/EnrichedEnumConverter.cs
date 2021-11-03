using AllOverIt.Patterns.Enumeration;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AllOverIt.AspNetCore.Converters
{
    /// <summary>A base class to convert JSON to and from an <see cref="EnrichedEnum{TEnum}"/> type.</summary>
    /// <typeparam name="TEnum">The concrete <see cref="EnrichedEnum{TEnum}"/> type.</typeparam>
    public abstract class EnrichedEnumConverter<TEnum> : JsonConverter<TEnum>
        where TEnum : EnrichedEnum<TEnum>
    {
        /// <summary>Reads a string from the current JSON reader and converts it to the required <typeparamref name="TEnum"/> type.</summary>
        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return EnrichedEnum<TEnum>.From(reader.GetString());
        }

        /// <summary>Writes the value of a <typeparamref name="TEnum"/> instance to the current JSON writer.</summary>
        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Name);
        }
    }
}