using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.Serialization.Json.Newtonsoft.Converters
{
    /// <summary>Deserializes an interface to a concrete type.</summary>
    /// <typeparam name="TInterface">The interface type to convert from.</typeparam>
    /// <typeparam name="TConcrete">The concrete type to convert to.</typeparam>
    public class InterfaceConverter<TInterface, TConcrete> : JsonConverter
        where TConcrete : class, TInterface
    {
        private static readonly Type InterfaceType = typeof(TInterface);

        /// <summary>This converter cannot write JSON.</summary>
        public override bool CanWrite => false;

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == InterfaceType;
        }

        /// <inheritdoc />
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<TConcrete>(reader);
        }

        /// <inheritdoc />
        /// <remarks>The converter only supports deserialization so this method will throw <exception cref="NotImplementedException" /> if called.</remarks>
        [ExcludeFromCodeCoverage]
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            // This method will never be called as CanConvert() doesn't allow it
            throw new NotImplementedException();
        }
    }
}
