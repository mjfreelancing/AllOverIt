using AllOverIt.Patterns.Enumeration;
using Newtonsoft.Json;
using System;

namespace AllOverIt.Serialization.NewtonsoftJson.Converters
{
    /// <summary>Converts JSON to and from an <see cref="EnrichedEnum{TEnum}"/> type.</summary>
    /// <typeparam name="TEnum">The concrete <see cref="EnrichedEnum{TEnum}"/> type.</typeparam>
    public class EnrichedEnumJsonConverter<TEnum> : JsonConverter
        where TEnum : EnrichedEnum<TEnum>
    {
        /// <summary>Returns true if the value to be converted is a string.</summary>
        /// <param name="objectType">The object type</param>
        /// <returns>True if the value to be converted is a string.</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var strValue = reader.ReadAsString();

            return strValue == null
                ? null
                : EnrichedEnum<TEnum>.From(strValue);
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var enumValue = ((EnrichedEnum<TEnum>) value).Name;
            writer.WriteValue(enumValue);
        }
    }
}