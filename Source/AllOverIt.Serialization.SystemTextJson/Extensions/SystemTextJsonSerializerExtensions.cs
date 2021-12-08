using System.Text.Json.Serialization;
using AllOverIt.Serialization.SystemTextJson.Converters;

namespace AllOverIt.Serialization.SystemTextJson.Extensions
{
    public static class SystemTextJsonSerializerExtensions
    {
        public static void AddConverters(this SystemTextJsonSerializer serializer, params JsonConverter[] converters)
        {
            foreach (var converter in converters)
            {
                serializer.Options.Converters.Add(converter);
            }
        }

        public static void AddInterfaceConverter<TInterface, TConcrete>(this SystemTextJsonSerializer serializer, bool includeEnumerable)
            where TConcrete : class, TInterface
        {
            serializer.Options.Converters.Add(new InterfaceConverter<TInterface, TConcrete>());

            if (includeEnumerable)
            {
                serializer.AddEnumerableInterfaceConverter<TInterface, TConcrete>();
            }
        }

        public static void AddEnumerableInterfaceConverter<TInterface, TConcrete>(this SystemTextJsonSerializer serializer)
            where TConcrete : class, TInterface
        {
            serializer.Options.Converters.Add(new EnumerableInterfaceConverter<TInterface, TConcrete>());
        }
    }
}