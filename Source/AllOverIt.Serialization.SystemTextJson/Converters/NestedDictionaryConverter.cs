using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using AllOverIt.Extensions;

namespace AllOverIt.Serialization.SystemTextJson.Converters
{
    /// <summary>Implements a JSON Converter that converts to and from a Dictionary&lt;string, object>. All object and array
    /// properties are also converted to and from a Dictionary&lt;string, object>.</summary>
    internal sealed class NestedDictionaryConverter : JsonConverter<Dictionary<string, object>>
    {
        private static readonly Type DictionaryType = typeof(Dictionary<string, object>);

        /// <inheritdoc />
        public override Dictionary<string, object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw CreateReadJsonSerializationException(reader.TokenType);
            }

            var dictionary = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.Comment:
                        break;

                    case JsonTokenType.PropertyName:
                        var propertyName = reader.GetString();

                        if (propertyName.IsNullOrEmpty() || !reader.Read())
                        {
                            throw CreateReadJsonSerializationException(reader.TokenType);
                        }

                        var value = ReadValue(ref reader, options);
                        dictionary.Add(propertyName!, value);
                        break;

                    case JsonTokenType.EndObject:
                        return dictionary;
                }
            }

            return dictionary;
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

        private object ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.Number => reader.TryGetInt32(out var intValue) ? intValue : reader.GetDouble(),
                JsonTokenType.StartObject => Read(ref reader, null, options),
                JsonTokenType.StartArray => ReadArray(ref reader, options),
                JsonTokenType.String => reader.TryGetDateTime(out var date) ? date : reader.GetString(),
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                JsonTokenType.Null => null,
                _ => throw CreateReadJsonSerializationException(reader.TokenType)
            };
        }

        private object ReadArray(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var list = new List<object>();

            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                var value = ReadValue(ref reader, options);
                list.Add(value);
            }

            return list;
        }

        private void WriteValue(Utf8JsonWriter writer, string key, object objectValue)
        {
            if (key != null)
            {
                writer.WritePropertyName(key);
            }

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

                case object[] array:
                    writer.WriteStartArray();

                    foreach (var item in array)
                    {
                        WriteValue(writer, null, item);
                    }

                    writer.WriteEndArray();
                    break;

                default:
                    writer.WriteNullValue();
                    break;
            }
        }

        private static Exception CreateReadJsonSerializationException(JsonTokenType? tokenType = default)
        {
            var message = tokenType.HasValue
                ? $"Unexpected token '{tokenType}' when converting {DictionaryType.GetFriendlyName()}."
                : $"Unexpected error when converting {DictionaryType.GetFriendlyName()}.";

            return new JsonException(message);
        }
    }
}