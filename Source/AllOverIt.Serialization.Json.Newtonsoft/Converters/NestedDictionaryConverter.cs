using AllOverIt.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AllOverIt.Serialization.Json.Newtonsoft.Converters
{
    /// <summary>Implements a JSON Converter that converts to and from a Dictionary&lt;string, object>. All object and array
    /// properties are also converted to and from a Dictionary&lt;string, object>.</summary>
    internal sealed class NestedDictionaryConverter : JsonConverter
    {
        private static readonly Type DictionaryType = typeof(Dictionary<string, object>);

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return DictionaryType.IsAssignableFrom(objectType);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return ReadValue(reader);
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            WriteValue(writer, value, serializer);
        }

        private object ReadValue(JsonReader reader)
        {
            return reader.TokenType switch
            {
                JsonToken.StartObject => ReadObject(reader),
                JsonToken.StartArray => ReadArray(reader),

                JsonToken.Integer or JsonToken.Float or JsonToken.String or JsonToken.Boolean or
                JsonToken.Undefined or JsonToken.Null or JsonToken.Date or JsonToken.Bytes => reader.Value,

                _ => throw CreateJsonSerializationException(reader.TokenType)
            };
        }

        private List<object> ReadArray(JsonReader reader)
        {
            var list = new List<object>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.EndArray:
                        return list;

                    default:
                        var value = ReadValue(reader);
                        list.Add(value);
                        break;
                }
            }

            throw CreateJsonSerializationException();
        }

        private Dictionary<string, object> ReadObject(JsonReader reader)
        {
            var dictionary = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        var propertyName = $"{reader.Value}";

                        if (!reader.Read())
                        {
                            throw CreateJsonSerializationException();
                        }

                        var value = ReadValue(reader);
                        dictionary.Add(propertyName, value);
                        break;

                    case JsonToken.EndObject:
                        return dictionary;
                }
            }

            throw CreateJsonSerializationException();
        }

        private void WriteValue(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // TODO: Check the SystemText serializer does this - also need to add more tests
            var converter = serializer.Converters.FirstOrDefault(converter => !ReferenceEquals(converter, this) && converter.CanWrite && converter.CanConvert(value.GetType()));

            if (converter != null)
            {
                converter.WriteJson(writer, value, serializer);
                return;
            }

            var token = JToken.FromObject(value);

            switch (token.Type)
            {
                case JTokenType.Object:
                    WriteObject(writer, value, serializer);
                    break;

                case JTokenType.Array:
                    WriteArray(writer, value, serializer);
                    break;

                default:
                    writer.WriteValue(value);
                    break;
            }
        }

        private void WriteObject(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            var element = value as Dictionary<string, object>;

            foreach (var kvp in element)
            {
                writer.WritePropertyName(kvp.Key);

                if (kvp.Value is not null)
                {
                    WriteValue(writer, kvp.Value, serializer);
                }
                else
                {
                    writer.WriteNull();
                }
            }

            writer.WriteEndObject();
        }

        private void WriteArray(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartArray();

            foreach (var element in value.GetObjectElements())
            {
                WriteValue(writer, element, serializer);
            }

            writer.WriteEndArray();
        }

        [ExcludeFromCodeCoverage]
        private static JsonSerializationException CreateJsonSerializationException(JsonToken? tokenType = default)
        {
            var message = tokenType.HasValue
                ? $"Unexpected token '{tokenType}' while {nameof(NestedDictionaryConverter)} was reading."
                : $"Unexpected error while {nameof(NestedDictionaryConverter)} was reading.";

            return new JsonSerializationException(message);
        }
    }
}