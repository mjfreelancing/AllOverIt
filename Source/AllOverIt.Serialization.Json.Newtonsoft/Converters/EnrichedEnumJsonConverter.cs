using AllOverIt.Patterns.Enumeration;
using Newtonsoft.Json;

namespace AllOverIt.Serialization.Json.Newtonsoft.Converters
{
    /// <summary>Converts JSON to and from an <see cref="EnrichedEnum{TEnum}"/> type.</summary>
    /// <typeparam name="TEnum">The concrete <see cref="EnrichedEnum{TEnum}"/> type.</typeparam>
    public class EnrichedEnumJsonConverter<TEnum> : JsonConverter
        where TEnum : EnrichedEnum<TEnum>
    {
        private static readonly Type EnrichedEnumJsonConverterType = typeof(EnrichedEnumJsonConverter<TEnum>);

        /// <summary>Returns true if the object to be converted is a <see cref="EnrichedEnum{TEnum}"/>.</summary>
        /// <param name="objectType">The object type.</param>
        /// <returns><see langword="true" /> if the object to be converted is a <see cref="EnrichedEnum{TEnum}"/>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TEnum);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var strValue = (string) reader.Value;

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

        /// <summary>Creates a <see cref="JsonConverter"/> for an <see cref="EnrichedEnum{TEnum}"/> type.</summary>
        /// <returns>A new JsonConverter instance.</returns>
        public static JsonConverter Create()
        {
            return (JsonConverter) Activator.CreateInstance(EnrichedEnumJsonConverterType);
        }
    }
}