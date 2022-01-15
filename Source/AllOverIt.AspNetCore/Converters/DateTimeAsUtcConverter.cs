using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AllOverIt.AspNetCore.Converters
{
    /// <summary>A JsonConverter that treats <seealso cref="DateTime"/> values as kind <seealso cref="DateTimeKind.Utc"/>.</summary>
    /// <remarks>The converter does not perform any DateTime conversions; it only sets the kind so it is treated as if it is a UTC DateTime.</remarks>
    public sealed class DateTimeAsUtcConverter : JsonConverter<DateTime>
    {
        /// <summary>Reads a <seealso cref="DateTime"/> value as a UTC based <seealso cref="DateTime"/>.</summary>
        /// <param name="reader">The Json reader.</param>
        /// <param name="typeToConvert">Ignored.</param>
        /// <param name="options">Ignored.</param>
        /// <returns>A <seealso cref="DateTime"/> value of kind <seealso cref="DateTimeKind.Utc"/>.</returns>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetDateTime();
            return DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        /// <summary>Writes a <seealso cref="DateTime"/> value as kind <seealso cref="DateTimeKind.Utc"/>.</summary>
        /// <param name="writer">The Json writer.</param>
        /// <param name="value">The value to be written.</param>
        /// <param name="options">Ignored.</param>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var utcDateTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            writer.WriteStringValue(utcDateTime);
        }
    }
}