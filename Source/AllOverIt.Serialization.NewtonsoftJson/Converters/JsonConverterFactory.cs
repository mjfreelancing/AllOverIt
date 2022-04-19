using Newtonsoft.Json;
using System;

namespace AllOverIt.Serialization.NewtonsoftJson.Converters
{
    /// <summary>Supports converting different types using the factory pattern.</summary>
    public abstract class JsonConverterFactory : JsonConverter
    {
        /// <summary>Creates a JsonConverter for a specified type.</summary>
        /// <param name="objectType">The type to create a JsonConverter for.</param>
        /// <returns>A JsonConverter specific to the specified type.</returns>
        public abstract JsonConverter CreateConverter(Type objectType);

        /// <inheritdoc />
        public sealed override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var converter = CreateConverter(objectType);
            return converter.ReadJson(reader, objectType, existingValue, serializer);
        }

        /// <inheritdoc />
        public sealed override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var objectType = value.GetType();
            var converter = CreateConverter(objectType);
            converter.WriteJson(writer, value, serializer);
        }
    }
}