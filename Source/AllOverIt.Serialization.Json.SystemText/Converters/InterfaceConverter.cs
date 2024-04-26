using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AllOverIt.Serialization.Json.SystemText.Converters
{
    /// <summary>Deserializes an interface to a concrete type.</summary>
    /// <typeparam name="TInterface">The interface type to convert from.</typeparam>
    /// <typeparam name="TConcrete">The concrete type to convert to.</typeparam>
    public class InterfaceConverter<TInterface, TConcrete> : JsonConverter<TInterface>
        where TConcrete : class, TInterface
    {
        private static readonly Type InterfaceType = typeof(TInterface);

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == InterfaceType;
        }

        /// <summary>Reads and converts the JSON to an instance of type <typeparamref name="TConcrete"/>,
        /// which is a <typeparamref name="TInterface"/>.</summary>
        /// <param name="reader">The JSON reader.</param>
        /// <param name="typeToConvert">The type to be converted.</param>
        /// <param name="options">The JSON serializer options.</param>
        /// <returns>An instance of type <typeparamref name="TConcrete"/>, which is a <typeparamref name="TInterface"/>.</returns>
        public override TInterface Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<TConcrete>(ref reader, options);
        }

        /// <inheritdoc />
        /// <remarks>The converter only supports deserialization so this method will throw <exception cref="NotImplementedException" /> if called.</remarks>
        [ExcludeFromCodeCoverage]
        public override void Write(Utf8JsonWriter writer, TInterface value, JsonSerializerOptions options)
        {
            // This method will never be called as CanConvert() doesn't allow it
            throw new NotImplementedException();
        }
    }
}