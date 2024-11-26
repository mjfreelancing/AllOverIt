using AllOverIt.Assertion;
using AllOverIt.Serialization.Json.SystemText.Converters;
using System.Text.Json.Serialization;

namespace AllOverIt.Serialization.Json.SystemText.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="SystemTextJsonSerializer"/>.</summary>
    public static class SystemTextJsonSerializerExtensions
    {
        /// <summary>Adds a Json Converter to the serializer.</summary>
        /// <param name="serializer">The serializer to add the Json converter to.</param>
        /// <param name="converters">The Json converter to add to the serializer.</param>
        public static void AddConverters(this SystemTextJsonSerializer serializer, params JsonConverter[] converters)
        {
            foreach (var converter in converters)
            {
                serializer.Options.Converters.Add(converter);
            }
        }

        /// <summary>Adds a converter that deserializes an interface type to a specified concrete type.</summary>
        /// <typeparam name="TInterface">The interface type to convert from.</typeparam>
        /// <typeparam name="TConcrete">The concrete type to convert to.</typeparam>
        /// <param name="serializer">The serializer performing the deserialization.</param>
        public static void AddInterfaceConverter<TInterface, TConcrete>(this SystemTextJsonSerializer serializer)
            where TConcrete : class, TInterface
        {
            _ = serializer.WhenNotNull();

            serializer.Options.Converters.Add(new InterfaceConverter<TInterface, TConcrete>());
        }
    }
}