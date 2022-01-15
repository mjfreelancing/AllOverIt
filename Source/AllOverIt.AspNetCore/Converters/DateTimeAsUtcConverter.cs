using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AllOverIt.AspNetCore.Converters
{
    public sealed class DateTimeAsUtcConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetDateTime();
            return DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var utcDateTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            writer.WriteStringValue(utcDateTime);
        }
    }
}