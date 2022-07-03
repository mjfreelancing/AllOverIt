using AllOverIt.Assertion;
using AllOverIt.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

            // Will return null if the enumerable comes from something like Enumerable.Range()
            // The action calls WriteObject() which attempts to deal with this by using the value's runtime type (if not null)
            var valueType = enumerable.GetType().GetGenericArguments().SingleOrDefault();

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

        public static void WriteDictionary(this IEnrichedBinaryWriter writer, IDictionary dictionary)
        {
            _ = dictionary.WhenNotNull(nameof(dictionary));

            var args = dictionary.GetType().GetGenericArguments();

            if (!args.Any())
            {
                // Assume IDictionary, such as from Environment.GetEnvironmentVariables(), contains values that can be converted to strings
                dictionary = dictionary.Cast<DictionaryEntry>().ToDictionary(entry => $"{entry.Key}", entry => $"{entry.Value}");
                args = dictionary.GetType().GetGenericArguments();
            }

            // The action calls WriteObject() which deals with 'object' types and null values, where possible.
            var keyType = args[0];
            var valueType = args[1];

            var keyEnumerator = dictionary.Keys.GetEnumerator();
            var valueEnumerator = dictionary.Values.GetEnumerator();

            writer.Write(dictionary.Count);

            while (keyEnumerator.MoveNext())
            {
                valueEnumerator.MoveNext();

                var key = keyEnumerator.Current;
                writer.WriteObject(keyType, key);

                var value = valueEnumerator.Current;
                writer.WriteObject(valueType, value);
            }
        }
    }
}
