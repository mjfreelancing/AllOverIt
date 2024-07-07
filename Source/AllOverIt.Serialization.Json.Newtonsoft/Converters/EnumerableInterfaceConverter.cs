using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.Serialization.Json.Newtonsoft.Converters
{
    /// <summary>Deserializes an <see cref="IEnumerable{TInterface}"/> to a <see cref="List{TConcrete}"/>.</summary>
    /// <typeparam name="TInterface">The interface type to convert from.</typeparam>
    /// <typeparam name="TConcrete">The concrete type to convert to.</typeparam>
    public class EnumerableInterfaceConverter<TInterface, TConcrete> : JsonConverter
        where TConcrete : class, TInterface
    {
        private static readonly Type InterfaceType = typeof(TInterface);
        private static readonly Type EnumerableInterfaceType = typeof(IEnumerable<>).MakeGenericType(typeof(TInterface));

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == EnumerableInterfaceType && typeToConvert.GenericTypeArguments[0] == InterfaceType;
        }

        /// <inheritdoc />
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<List<TConcrete>>(reader);
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