using Newtonsoft.Json;

namespace AllOverIt.Serialization.Json.Newtonsoft.Converters
{
    /// <summary>Supports converting different types using the factory pattern.</summary>
    public abstract class JsonConverterFactory : JsonConverter
    {
        /// <summary>Creates a JsonConverter for a specified type.</summary>
        /// <param name="objectType">The type to create a JsonConverter for.</param>
        /// <returns>A JsonConverter specific to the specified type.</returns>
        public abstract JsonConverter CreateConverter(Type objectType);

        /// <inheritdoc />
        public sealed override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var converter = CreateConverter(objectType);

            return converter.ReadJson(reader, objectType, existingValue, serializer);
        }

        /// <inheritdoc />
        public sealed override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            var objectType = value.GetType();
            var converter = CreateConverter(objectType);
            converter.WriteJson(writer, value, serializer);
        }
    }
}