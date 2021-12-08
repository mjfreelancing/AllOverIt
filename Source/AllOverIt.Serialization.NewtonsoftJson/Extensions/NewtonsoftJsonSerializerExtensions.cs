using AllOverIt.Serialization.NewtonsoftJson.Converters;
using Newtonsoft.Json;

namespace AllOverIt.Serialization.NewtonsoftJson.Extensions
{
    public static class NewtonsoftJsonSerializerExtensions
    {
        public static void AddConverters(this NewtonsoftJsonSerializer serializer, params JsonConverter[] converters)
        {
            foreach (var converter in converters)
            {
                serializer.Settings.Converters.Add(converter);
            }
        }

        public static void AddInterfaceConverter<TInterface, TConcrete>(this NewtonsoftJsonSerializer serializer, bool includeEnumerable)
            where TConcrete : class, TInterface
        {
            serializer.Settings.Converters.Add(new InterfaceConverter<TInterface, TConcrete>());

            if (includeEnumerable)
            {
                serializer.AddEnumerableInterfaceConverter<TInterface, TConcrete>();
            }
        }

        public static void AddEnumerableInterfaceConverter<TInterface, TConcrete>(this NewtonsoftJsonSerializer serializer)
            where TConcrete : class, TInterface
        {
            serializer.Settings.Converters.Add(new EnumerableInterfaceConverter<TInterface, TConcrete>());
        }
    }
}