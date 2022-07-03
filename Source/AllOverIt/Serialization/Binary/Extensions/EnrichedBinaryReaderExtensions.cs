using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Serialization.Binary.Extensions
{
    public static class EnrichedBinaryReaderExtensions
    {
        private static readonly Type ObjectType = typeof(object);

        public static TValue ReadObject<TValue>(this IEnrichedBinaryReader reader)      // Enumerables are returned where TValue = List<object>. See ReadEnumerable().
        {
            return (TValue) reader.ReadObject();
        }

        public static TValue? ReadNullable<TValue>(this IEnrichedBinaryReader reader) where TValue : struct
        {
            return (TValue?) reader.ReadObject();
        }

        public static IEnumerable<object> ReadEnumerable(this IEnrichedBinaryReader reader)
        {
            var count = reader.ReadInt32();

            var values = new List<object>();

            for (var i = 0; i < count; i++)
            {
                var value = reader.ReadObject();
                values.Add(value);
            }

            return values;
        }

        public static IEnumerable<TValue> ReadEnumerable<TValue>(this IEnrichedBinaryReader reader)      // Enumerables are returned where TValue = List<Type>
        {
            return reader.ReadObject<List<object>>().Cast<TValue>();
        }

        public static IDictionary<object, object> ReadDictionary(this IEnrichedBinaryReader reader)
        {
            var count = reader.ReadInt32();

            var values = new Dictionary<object, object>();

            for (var i = 0; i < count; i++)
            {
                var key = reader.ReadObject();
                var value = reader.ReadObject();

                values.Add(key, value);
            }

            return values;
        }

        public static IDictionary<TKey, TValue> ReadDictionary<TKey, TValue>(this IEnrichedBinaryReader reader)
        {
            var dictionary = reader.ReadObject<Dictionary<object, object>>();

            if (typeof(TKey) == ObjectType && typeof(TValue) == ObjectType)
            {
                return (IDictionary<TKey, TValue>) dictionary;
            }

            return dictionary.ToDictionary(kvp => (TKey) kvp.Key, kvp => (TValue) kvp.Value);
        }
    }
}
