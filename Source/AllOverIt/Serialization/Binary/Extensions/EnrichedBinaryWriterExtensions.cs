using AllOverIt.Assertion;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AllOverIt.Serialization.Binary.Extensions
{

    public static class EnrichedBinaryWriterExtensions
    {
        public static void WriteNullable<TValue>(this IEnrichedBinaryWriter writer, TValue? value) where TValue : struct
        {
            writer.WriteObject(typeof(TValue?), value);
        }

        // Writes the value type and the value
        public static void WriteObject<TType>(this IEnrichedBinaryWriter writer, TType value)     // required for nullable types (need the type information)
        {
            writer.WriteObject(typeof(TType), value);
        }

        public static void WriteEnumerable(this IEnrichedBinaryWriter writer, IEnumerable enumerable)
        {
            _ = enumerable.WhenNotNull(nameof(enumerable));

            // Enumerable.Range()                    => returns a RangeIterator - no generic arguments
            // IEnumerable<int>                      => contains one generic argument
            // int?[]{}.Select(item => (object)item) => returns SelectEnumerableIterator<int?, object> - two generic arguments
            //
            // Capturing the generic type if available, otherwise will get the type of each value
            var genericArguments = enumerable.GetType().GetGenericArguments();

            var argType = genericArguments.Length == 1
                ? genericArguments[0]
                : null;

            WriteEnumerable(writer, enumerable, argType ?? typeof(object));
        }

        public static void WriteEnumerable<TType>(this IEnrichedBinaryWriter writer, IEnumerable<TType> enumerable)
        {
            _ = enumerable.WhenNotNull(nameof(enumerable));

            WriteEnumerable(writer, enumerable, typeof(TType));
        }

        public static void WriteEnumerable(this IEnrichedBinaryWriter writer, IEnumerable enumerable, Type valueType)
        {
            _ = enumerable.WhenNotNull(nameof(enumerable));

            if (enumerable is not ICollection collection)
            {
                var values = new List<object>();

                foreach (var value in enumerable)
                {
                    values.Add(value);
                }

                collection = values;
            }

            writer.Write(collection.Count);

            foreach (var value in collection)
            {
                writer.WriteObject(valueType, value);
            }
        }




        // Not providing a <TKey, TValue> generic version as it will be ambigious with this overload. See WriteTypedDictionary()
        // This method exists so it supports methods such as Environment.GetEnvironmentVariables() which returns IDictionary.
        public static void WriteDictionary(this IEnrichedBinaryWriter writer, IDictionary dictionary)
        {
            _ = dictionary.WhenNotNull(nameof(dictionary));

            Type keyType;
            Type valueType;

            var genericArguments = dictionary.GetType().GetGenericArguments();

            if (genericArguments.Length == 2)
            {
                keyType = genericArguments[0];
                valueType = genericArguments[1];
            }
            else
            {
                keyType = typeof(object);
                valueType = typeof(object);
            }

            WriteDictionary(writer, dictionary, keyType, valueType);
        }

        // Convenience method as cannot provide a WriteDictionary<TKey, TValue>() without becoming ambigious with WriteDictionary(IDictionary)
        public static void WriteTypedDictionary<TKey, TValue>(this IEnrichedBinaryWriter writer, IDictionary<TKey, TValue> dictionary)
        {
            _ = dictionary.WhenNotNull(nameof(dictionary));

            WriteDictionary(writer, (IDictionary) dictionary, typeof(TKey), typeof(TValue));
        }

        public static void WriteDictionary(this IEnrichedBinaryWriter writer, IDictionary dictionary, Type keyType, Type valueType)
        {
            _ = dictionary.WhenNotNull(nameof(dictionary));

            writer.Write(dictionary.Count);

            foreach (DictionaryEntry entry in dictionary)
            {
                writer.WriteObject(keyType, entry.Key);
                writer.WriteObject(valueType, entry.Value);
            }
        }
    }
}
