using AllOverIt.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AllOverIt.Serialization.Json.SystemText.Converters
{
    /// <summary>Implements a JSON Converter that converts to and from a Dictionary&lt;string, object>. All object and array
    /// properties are also converted to and from a Dictionary&lt;string, object>.</summary>
    internal sealed class NestedDictionaryConverter : JsonConverter<Dictionary<string, object>>
    {
        private readonly NestedDictionaryConverterOptions _options;

        public NestedDictionaryConverter()
            : this(new NestedDictionaryConverterOptions())
        {
        }

        public NestedDictionaryConverter(NestedDictionaryConverterOptions options)
        {
            _options = options;
        }


        /// <inheritdoc />
        public override Dictionary<string, object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw CreateJsonSerializationException(reader.TokenType);
            }

            var dictionary = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        var propertyName = reader.GetString();

                        if (!reader.Read())
                        {
                            throw CreateJsonSerializationException(reader.TokenType);
                        }

                        var value = ReadValue(ref reader, options);
                        dictionary.Add(propertyName!, value);
                        break;

                    case JsonTokenType.EndObject:
                        return dictionary;
                }
            }

            throw CreateJsonSerializationException();
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Dictionary<string, object> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var key in value.Keys)
            {
                WriteValue(writer, key, value[key]);
            }

            writer.WriteEndObject();
        }

        private object ReadFloatingValue(ref Utf8JsonReader reader)
        {
            return _options.ReadFloatingAsDecimals
                ? reader.GetDecimal()
                : reader.GetDouble();
        }

        private object ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Number => reader.TryGetInt32(out var intValue) ? intValue : ReadFloatingValue(ref reader),
                JsonTokenType.StartObject => Read(ref reader, null, options),
                JsonTokenType.StartArray => ReadArray(ref reader, options),
                JsonTokenType.String => reader.TryGetDateTime(out var date) ? date : reader.GetString(),
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                JsonTokenType.Null => null,
                _ => throw CreateJsonSerializationException(reader.TokenType)
            };
        }

        private List<object> ReadArray(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var list = new List<object>();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                var value = ReadValue(ref reader, options);
                list.Add(value);
            }

            return list;
        }

        private static void WriteValue(Utf8JsonWriter writer, string key, object objectValue)
        {
            if (key != null)
            {
                writer.WritePropertyName(key);
            }

            if (objectValue is not null && objectValue.GetType().IsArray)
            {
                WriteArray(writer, objectValue);
            }
            else
            {
                WriteValue(writer, objectValue);
            }
        }

        private static void WriteArray(Utf8JsonWriter writer, object objectValue)
        {
            writer.WriteStartArray();

            foreach (var item in objectValue.GetObjectElements())
            {
                WriteValue(writer, null, item);
            }

            writer.WriteEndArray();
        }

        private static void WriteValue(Utf8JsonWriter writer, object objectValue)
        {
            switch (objectValue)
            {
                case string stringValue:
                    writer.WriteStringValue(stringValue);
                    break;

                case DateTime dateTime:
                    writer.WriteStringValue(dateTime);
                    break;

                case long longValue:
                    writer.WriteNumberValue(longValue);
                    break;

                case int intValue:
                    writer.WriteNumberValue(intValue);
                    break;

                case float floatValue:
                    writer.WriteNumberValue(floatValue);
                    break;

                case double doubleValue:
                    writer.WriteNumberValue(doubleValue);
                    break;

                case decimal decimalValue:
                    writer.WriteNumberValue(decimalValue);
                    break;

                case bool boolValue:
                    writer.WriteBooleanValue(boolValue);
                    break;

                case Dictionary<string, object> dictionary:
                    writer.WriteStartObject();

                    foreach (var item in dictionary)
                    {
                        WriteValue(writer, item.Key, item.Value);
                    }

                    writer.WriteEndObject();
                    break;

                case Enum enumValue:
                    writer.WriteStringValue(enumValue.ToString());
                    break;

                case null:
                    writer.WriteNullValue();
                    break;

                default:
                    throw new InvalidOperationException($"Unhandled object type '{objectValue.GetType().GetFriendlyName()}' while writing a nested dictionary.");
            }
        }

        [ExcludeFromCodeCoverage]
        private static JsonException CreateJsonSerializationException(JsonTokenType? tokenType = default)
        {
            var message = tokenType.HasValue
                ? $"Unexpected token '{tokenType}' while {nameof(NestedDictionaryConverter)} was reading."
                : $"Unexpected error while {nameof(NestedDictionaryConverter)} was reading.";

            return new JsonException(message);
        }
    }
}