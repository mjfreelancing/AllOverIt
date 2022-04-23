using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using AllOverIt.Extensions;

namespace AllOverIt.Serialization.NewtonsoftJson.Converters
{
    /// <summary>Implements a JSON Converter that converts to and from a Dictionary&lt;string, object>. All object and array properties are
    /// expected to also be of type Dictionary&lt;string, object>.</summary>
    public sealed class StringObjectDictionaryConverter : JsonConverter
    {
        private static readonly Type DictionaryType = typeof(IDictionary<string, object>);

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
            WriteValue(writer, value);
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
            var dictionary = new Dictionary<string, object>();

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

        private void WriteValue(JsonWriter writer, object value)
        {
            var token = JToken.FromObject(value);

            switch (token.Type)
            {
                case JTokenType.Object:
                    WriteObject(writer, value);
                    break;

                case JTokenType.Array:
                    WriteArray(writer, value);
                    break;

                default:
                    writer.WriteValue(value);
                    break;
            }
        }

        private void WriteObject(JsonWriter writer, object value)
        {
            writer.WriteStartObject();

            var element = value as IDictionary<string, object>;

            foreach (var kvp in element!)
            {
                writer.WritePropertyName(kvp.Key);
                WriteValue(writer, kvp.Value);
            }

            writer.WriteEndObject();
        }

        private void WriteArray(JsonWriter writer, object value)
        {
            writer.WriteStartArray();

            var array = value as IEnumerable<object>;

            foreach (var element in array!)
            {
                WriteValue(writer, element);
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