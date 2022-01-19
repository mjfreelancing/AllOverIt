using System;
using Newtonsoft.Json;

namespace AllOverIt.Serialization.NewtonsoftJson.Converters
{
    /// <summary>A JsonConverter that treats <see cref="DateTime"/> values as kind <see cref="DateTimeKind.Utc"/>.</summary>
    /// <remarks>The converter does not perform any DateTime conversions; it only sets the kind so it is treated as if it is a UTC DateTime.</remarks>
    public sealed class DateTimeAsUtcConverter : JsonConverter
    {
        /// <summary>Returns true if the value to be converted is a <see cref="DateTime"/>.</summary>
        /// <param name="objectType">The object type</param>
        /// <returns>True if the value to be converted is a <see cref="DateTime"/>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(DateTime) == objectType;
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var dateTime = serializer.Deserialize<DateTime>(reader);
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var utcDateTime = DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
            writer.WriteValue(utcDateTime);
        }
    }
}