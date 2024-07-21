using System.Text.Json;
using System.Text.Json.Serialization;

namespace AllOverIt.Serialization.Json.SystemText.Converters
{
    /// <summary>Converts a <see cref="DateTime"/> to or from JSON while treating it's <seealso cref="DateTimeKind"/> as <see cref="DateTimeKind.Utc"/>.</summary>
    /// <remarks>The converter does not perform any DateTime conversions; it only sets the kind so it is treated as if it is a UTC DateTime.</remarks>
    public sealed class DateTimeAsUtcConverter : JsonConverter<DateTime>
    {
        /// <summary>Reads a <see cref="DateTime"/> value as a UTC based <see cref="DateTime"/>.</summary>
        /// <param name="reader">The Json reader.</param>
        /// <param name="typeToConvert">Ignored.</param>
        /// <param name="options">Ignored.</param>
        /// <returns>A <see cref="DateTime"/> value of kind <see cref="DateTimeKind.Utc"/>.</returns>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetDateTime();
            return DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        /// <summary>Writes a <see cref="DateTime"/> value as kind <see cref="DateTimeKind.Utc"/>.</summary>
        /// <param name="writer">The Json writer.</param>
        /// <param name="value">The value to be written.</param>
        /// <param name="options">Ignored.</param>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // Null values do not pass through here
            var utcDateTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            writer.WriteStringValue(utcDateTime);
        }
    }
}