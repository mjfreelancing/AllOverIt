using AllOverIt.Reflection;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AllOverIt.Serialization.SystemTextJson.Converters
{
    /// <summary>Deserializes an <see cref="IEnumerable{TInterface}"/> to an <see cref="IList{TConcrete}"/>.</summary>
    /// <typeparam name="TInterface">The interface type to convert from.</typeparam>
    /// <typeparam name="TConcrete">The concrete type to convert to.</typeparam>
    public class EnumerableInterfaceConverter<TInterface, TConcrete> : JsonConverter<IList<TConcrete>>
        where TConcrete : class, TInterface
    {
        private static readonly Type InterfaceType = typeof(TInterface);
        private static readonly Type EnumerableInterfaceType = CommonTypes.IEnumerableGenericType.MakeGenericType(typeof(TInterface));

        /// <inheritdoc />
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == EnumerableInterfaceType && typeToConvert.GenericTypeArguments[0] == InterfaceType;
        }

        /// <inheritdoc />
        public override IList<TConcrete> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<List<TConcrete>>(ref reader, options);
        }

        /// <inheritdoc />
        /// <remarks>The converter only supports deserialization so this method will throw <exception cref="NotImplementedException" /> if called.</remarks>
        public override void Write(Utf8JsonWriter writer, IList<TConcrete> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}