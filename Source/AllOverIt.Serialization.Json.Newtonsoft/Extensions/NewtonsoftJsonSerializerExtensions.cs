using AllOverIt.Assertion;
using AllOverIt.Serialization.Json.Newtonsoft.Converters;
using Newtonsoft.Json;

namespace AllOverIt.Serialization.Json.Newtonsoft.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="NewtonsoftJsonSerializer"/>.</summary>
    public static class NewtonsoftJsonSerializerExtensions
    {
        /// <summary>Adds a Json Converter to the serializer.</summary>
        /// <param name="serializer">The serializer to add the Json converter to.</param>
        /// <param name="converters">The Json converter to add to the serializer.</param>
        public static void AddConverters(this NewtonsoftJsonSerializer serializer, params JsonConverter[] converters)
        {
            foreach (var converter in converters)
            {
                serializer.Settings.Converters.Add(converter);
            }
        }

        /// <summary>Adds a converter that deserializes an interface type to a specified concrete type.</summary>
        /// <typeparam name="TInterface">The interface type to convert from.</typeparam>
        /// <typeparam name="TConcrete">The concrete type to convert to.</typeparam>
        /// <param name="serializer">The serializer performing the deserialization.</param>
        public static void AddInterfaceConverter<TInterface, TConcrete>(this NewtonsoftJsonSerializer serializer)
            where TConcrete : class, TInterface
        {
            _ = serializer.WhenNotNull();

            serializer.Settings.Converters.Add(new InterfaceConverter<TInterface, TConcrete>());
        }
    }
}