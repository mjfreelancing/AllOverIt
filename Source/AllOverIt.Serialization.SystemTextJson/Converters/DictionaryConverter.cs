using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AllOverIt.Serialization.SystemTextJson.Converters
{
    // TODO: review exception messages

    /// <summary>Implements a JSON Converter that converts to and from a Dictionary&lt;string, object>.</summary>
    public sealed class DictionaryConverter : JsonConverter<Dictionary<string, object>>
    {
        /// <inheritdoc />
        public override Dictionary<string, object> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");
            }

            var dictionary = new Dictionary<string, object>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return dictionary;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("JsonTokenType was not PropertyName");
                }

                var propertyName = reader.GetString();

                if (string.IsNullOrWhiteSpace(propertyName))
                {
                    throw new JsonException("Failed to get property name");
                }

                reader.Read();

                var value = ReadValue(ref reader, options);
                dictionary.Add(propertyName, value);
            }

            return dictionary;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, Dictionary<string, object> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var key in value.Keys)
            {
                HandleValue(writer, key, value[key]);
            }

            writer.WriteEndObject();
        }

        private static void HandleValue(Utf8JsonWriter writer, string key, object objectValue)
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

                case Dictionary<string, object> dict:
                    writer.WriteStartObject();

                    foreach (var item in dict)
                    {
                        HandleValue(writer, item.Key, item.Value);
                    }

                    writer.WriteEndObject();
                    break;

                case object[] array:
                    writer.WriteStartArray();

                    foreach (var item in array)
                    {
                        HandleValue(writer, null, item);
                    }

                    writer.WriteEndArray();
                    break;

                default:
                    writer.WriteNullValue();
                    break;
            }
        }

        private object ReadValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    // Newtonsoft includes a Date token, try and interpret similarly
                    if (reader.TryGetDateTime(out var date))
                    {
                        return date;
                    }

                    return reader.GetString();

                case JsonTokenType.False:
                    return false;

                case JsonTokenType.True:
                    return true;

                case JsonTokenType.Null:
                    return null;

                case JsonTokenType.Number:
                    // Newtonsoft includes a float / integer token, try and interpret similarly
                    if (reader.TryGetInt32(out var intValue))
                    {
                        return intValue;
                    }

                    return reader.GetDouble();

                case JsonTokenType.StartObject:
                    return Read(ref reader, null, options);

                case JsonTokenType.StartArray:
                    var list = new List<object>();

                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        var value = ReadValue(ref reader, options);
                        list.Add(value);
                    }

                    return list;

                default:
                    throw new JsonException($"'{reader.TokenType}' is not supported");
            }
        }
    }
}