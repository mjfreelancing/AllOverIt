using System.Collections.Generic;
using System.Text.Json.Serialization;
using AllOverIt.Serialization.SystemTextJson.Converters;

namespace AllOverIt.Serialization.SystemTextJson.Extensions
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
        /// <param name="includeEnumerable">If true, a second converter will be added that will convert <see cref="IEnumerable{T}"/>
        /// to a <see cref="List{TConcrete}"/>.</param>
        public static void AddInterfaceConverter<TInterface, TConcrete>(this SystemTextJsonSerializer serializer, bool includeEnumerable = false)
            where TConcrete : class, TInterface
        {
            serializer.Options.Converters.Add(new InterfaceConverter<TInterface, TConcrete>());

            if (includeEnumerable)
            {
                serializer.AddEnumerableInterfaceConverter<TInterface, TConcrete>();
            }
        }

        /// <summary>Adds a converter that deserializes an <see cref="IEnumerable{TInterface}"/> to a <see cref="List{TConcrete}"/>..</summary>
        /// <typeparam name="TInterface">The interface type to convert from.</typeparam>
        /// <typeparam name="TConcrete">The concrete type to convert to.</typeparam>
        /// <param name="serializer">The serializer performing the deserialization.</param>
        public static void AddEnumerableInterfaceConverter<TInterface, TConcrete>(this SystemTextJsonSerializer serializer)
            where TConcrete : class, TInterface
        {
            serializer.Options.Converters.Add(new EnumerableInterfaceConverter<TInterface, TConcrete>());
        }
    }
}