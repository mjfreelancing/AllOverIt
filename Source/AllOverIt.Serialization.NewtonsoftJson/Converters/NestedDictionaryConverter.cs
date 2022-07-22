using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using AllOverIt.Extensions;
using System.Linq;

namespace AllOverIt.Serialization.NewtonsoftJson.Converters
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
            while (reader.TokenType == JsonToken.Comment)
            {
                if (!reader.Read())
                {
                    throw CreateReadJsonSerializationException();
                }
            }

            return reader.TokenType switch
            {
                JsonToken.StartObject => ReadObject(reader),
                JsonToken.StartArray => ReadArray(reader),

                JsonToken.Integer or JsonToken.Float or JsonToken.String or JsonToken.Boolean or
                JsonToken.Undefined or JsonToken.Null or JsonToken.Date or JsonToken.Bytes => reader.Value,

                _ => throw CreateReadJsonSerializationException(reader.TokenType)
            };
        }

        private object ReadArray(JsonReader reader)
        {
            var list = new List<object>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Comment:
                        break;

                    case JsonToken.EndArray:
                        return list;

                    default:
                        var value = ReadValue(reader);
                        list.Add(value);
                        break;
                }
            }

            throw CreateReadJsonSerializationException();
        }

        private object ReadObject(JsonReader reader)
        {
            var dictionary = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Comment:
                        break;

                    case JsonToken.PropertyName:
                        var propertyName = $"{reader.Value}";

                        if (propertyName.IsNullOrEmpty() || !reader.Read())
                        {
                            throw CreateReadJsonSerializationException();
                        }

                        var value = ReadValue(reader);
                        dictionary.Add(propertyName, value); 
                        break;

                    case JsonToken.EndObject:
                        return dictionary;
                }
            }

            throw CreateReadJsonSerializationException();
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

            var element = value as IDictionary<string, object>;

            foreach (var kvp in element!)
            {
                writer.WritePropertyName(kvp.Key);
                WriteValue(writer, kvp.Value, serializer);
            }

            writer.WriteEndObject();
        }

        private void WriteArray(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartArray();

            var array = value as IEnumerable<object>;

            foreach (var element in array!)
            {
                WriteValue(writer, element, serializer);
            }

            writer.WriteEndArray();
        }

        private static Exception CreateReadJsonSerializationException(JsonToken? tokenType = default)
        {
            var message = tokenType.HasValue
                ? $"Unexpected token '{tokenType}' when converting {DictionaryType.GetFriendlyName()}."
                : $"Unexpected error when converting {DictionaryType.GetFriendlyName()}.";

            return new JsonSerializationException(message);
        }
    }
}