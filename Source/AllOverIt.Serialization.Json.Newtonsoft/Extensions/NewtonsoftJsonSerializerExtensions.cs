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
        /// <param name="includeEnumerable">If true, a second converter will be added that will convert <see cref="IEnumerable{TInterface}"/>
        /// to a <see cref="List{TConcrete}"/>.</param>
        public static void AddInterfaceConverter<TInterface, TConcrete>(this NewtonsoftJsonSerializer serializer, bool includeEnumerable = false)
            where TConcrete : class, TInterface
        {
            serializer.Settings.Converters.Add(new InterfaceConverter<TInterface, TConcrete>());

            if (includeEnumerable)
            {
                serializer.AddEnumerableInterfaceConverter<TInterface, TConcrete>();
            }
        }

        /// <summary>Adds a converter that deserializes an <see cref="IEnumerable{TInterface}"/> to a <see cref="List{TConcrete}"/>..</summary>
        /// <typeparam name="TInterface">The interface type to convert from.</typeparam>
        /// <typeparam name="TConcrete">The concrete type to convert to.</typeparam>
        /// <param name="serializer">The serializer performing the deserialization.</param>
        public static void AddEnumerableInterfaceConverter<TInterface, TConcrete>(this NewtonsoftJsonSerializer serializer)
            where TConcrete : class, TInterface
        {
            serializer.Settings.Converters.Add(new EnumerableInterfaceConverter<TInterface, TConcrete>());
        }
    }
}