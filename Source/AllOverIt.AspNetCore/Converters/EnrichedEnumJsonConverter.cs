using AllOverIt.Patterns.Enumeration;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AllOverIt.AspNetCore.Converters
{
    /// <summary>Converts JSON to and from an <see cref="EnrichedEnum{TEnum}"/> type.</summary>
    /// <typeparam name="TEnum">The concrete <see cref="EnrichedEnum{TEnum}"/> type.</typeparam>
    public class EnrichedEnumJsonConverter<TEnum> : JsonConverter<TEnum>
        where TEnum : EnrichedEnum<TEnum>
    {
        private static readonly Type EnrichedEnumJsonConverterType = typeof(EnrichedEnumJsonConverter<TEnum>);

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

        /// <summary>Creates a <see cref="JsonConverter"/> for an <see cref="EnrichedEnum{TEnum}"/> type.</summary>
        /// <returns>A new JsonConverter instance.</returns>
        public static JsonConverter Create()
        {
            return (JsonConverter) Activator.CreateInstance(EnrichedEnumJsonConverterType);
        }
    }
}