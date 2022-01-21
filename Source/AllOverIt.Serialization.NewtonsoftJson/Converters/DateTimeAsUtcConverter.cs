using Newtonsoft.Json;
using System;

namespace AllOverIt.Serialization.NewtonsoftJson.Converters
{
    /// <summary>Converts a <see cref="DateTime"/> to or from JSON while treating it's <seealso cref="DateTimeKind"/> as <see cref="DateTimeKind.Utc"/>.</summary>
    /// <remarks>The converter does not perform any DateTime conversions; it only sets the kind so it is treated as if it is a UTC DateTime.</remarks>
    public sealed class DateTimeAsUtcConverter : JsonConverter
    {
        /// <summary>Returns true if the object to be converted is a <see cref="DateTime"/>.</summary>
        /// <param name="objectType">The object type.</param>
        /// <returns>True if the object to be converted is a <see cref="DateTime"/>.</returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(DateTime) == objectType;
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                throw new FormatException("Expecting to read a DateTime value but found null");
            }

            var dateTime = (DateTime) reader.Value;

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