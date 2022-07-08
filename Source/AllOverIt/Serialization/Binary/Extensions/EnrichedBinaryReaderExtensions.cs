using AllOverIt.Serialization.Binary.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Serialization.Binary.Extensions
{
    public static class EnrichedBinaryReaderExtensions
    {
        private static readonly Type ObjectType = typeof(object);

        public static string ReadSafeString(this IEnrichedBinaryReader reader)
        {
            var hasValue = reader.ReadBoolean();

            return hasValue
                ? reader.ReadString()
                : default;
        }

        public static Guid ReadGuid(this IEnrichedBinaryReader reader)
        {
            var bytes = reader.ReadBytes(16);
            return new Guid(bytes);
        }

        public static object ReadEnum(this IEnrichedBinaryReader reader)
        {
            var enumType = GetEnumType(reader);
            var value = reader.ReadString();

            return Enum.Parse(enumType, value);
        }

        public static TEnum ReadEnum<TEnum>(this IEnrichedBinaryReader reader)
        {
            var enumType = GetEnumType(reader);
            var value = reader.ReadString();

            return (TEnum) Enum.Parse(enumType, value);
        }

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
            var dictionary = reader.ReadDictionary();

            if (typeof(TKey) == ObjectType && typeof(TValue) == ObjectType)
            {
                return (IDictionary<TKey, TValue>) dictionary;
            }

            return dictionary.ToDictionary(kvp => (TKey) kvp.Key, kvp => (TValue) kvp.Value);
        }

        private static Type GetEnumType(IEnrichedBinaryReader reader)
        {
            var enumTypeName = reader.ReadString();
            var enumType = Type.GetType(enumTypeName);

            if (enumType is null)
            {
                throw new BinaryReaderException($"Unknown enum type '{enumTypeName}'.");
            }

            return enumType;
        }
    }
}
