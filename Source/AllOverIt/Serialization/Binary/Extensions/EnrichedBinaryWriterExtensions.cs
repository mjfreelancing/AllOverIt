using AllOverIt.Assertion;
using AllOverIt.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AllOverIt.Serialization.Binary.Extensions
{
    public static class EnrichedBinaryWriterExtensions
    {
        /// <inheritdoc cref="BinaryWriter.Write(ulong)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteUInt64(this IEnrichedBinaryWriter writer, ulong value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(uint)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteUInt32(this IEnrichedBinaryWriter writer, uint value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(ushort)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteUInt16(this IEnrichedBinaryWriter writer, ushort value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(string)"/>
        /// <param name="writer">The binary writer.</param>
        /// <remarks>The value can null.</remarks>
        public static void WriteSafeString(this IEnrichedBinaryWriter writer, string value)
        {
            var hasValue = value.IsNotNullOrEmpty();
            writer.WriteBoolean(hasValue);

            if (hasValue)
            {
                writer.Write(value);
            }
        }

        /// <inheritdoc cref="BinaryWriter.Write(float)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteSingle(this IEnrichedBinaryWriter writer, float value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(sbyte)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteSByte(this IEnrichedBinaryWriter writer, sbyte value) => writer.Write(value);

#if NETSTANDARD2_1 || NETCOREAPP3_1_OR_GREATER
        /// <inheritdoc cref="BinaryWriter.Write(ReadOnlySpan{char})"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteChars(this IEnrichedBinaryWriter writer, ReadOnlySpan<char> chars) => writer.Write(chars);

        /// <inheritdoc cref="BinaryWriter.Write(ReadOnlySpan{byte})"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteBytes(this IEnrichedBinaryWriter writer, ReadOnlySpan<byte> buffer) => writer.Write(buffer);
#endif

        /// <inheritdoc cref="BinaryWriter.Write(long)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteInt64(this IEnrichedBinaryWriter writer, long value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(int)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteInt32(this IEnrichedBinaryWriter writer, int value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(double)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteDouble(this IEnrichedBinaryWriter writer, double value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(decimal)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteDecimal(this IEnrichedBinaryWriter writer, decimal value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(char[], int, int)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteChars(this IEnrichedBinaryWriter writer, char[] chars, int index, int count) => writer.Write(chars, index, count);

        /// <inheritdoc cref="BinaryWriter.Write(char[])"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteChars(this IEnrichedBinaryWriter writer, char[] chars) => writer.Write(chars);

        /// <inheritdoc cref="BinaryWriter.Write(byte[], int, int)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteBytes(this IEnrichedBinaryWriter writer, byte[] buffer, int index, int count) => writer.Write(buffer, index, count);

        /// <inheritdoc cref="BinaryWriter.Write(byte[])"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteBytes(this IEnrichedBinaryWriter writer, byte[] buffer) => writer.Write(buffer);

        /// <inheritdoc cref="BinaryWriter.Write(byte)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteByte(this IEnrichedBinaryWriter writer, byte value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(bool)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteBoolean(this IEnrichedBinaryWriter writer, bool value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(short)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteInt16(this IEnrichedBinaryWriter writer, short value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(char)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteChar(this IEnrichedBinaryWriter writer, char value) => writer.Write(value);

        /// <summary>Writes a GUID value to the current stream.</summary>
        /// <param name="writer">The binary writer.</param>
        public static void WriteGuid(this IEnrichedBinaryWriter writer, Guid value) => writer.WriteBytes(value.ToByteArray());

        /// <summary>Writes an Enum value to the current stream.</summary>
        /// <param name="writer">The binary writer.</param>
        /// <remarks>The enum's assembly qualified type name and it's string value are written, allowing <see cref="BinaryReaderExtensions.ReadEnum(BinaryReader)"/>
        /// to read the value and create the appropriate enum type.</remarks>
        public static void WriteEnum(this IEnrichedBinaryWriter writer, object value)
        {
            // Need the string representation of the value in order to convert it back to the original Enum type.
            // Convert.ChangeType() cannot convert an integral type to an Enum type.
            writer.Write(value.GetType().AssemblyQualifiedName);
            writer.Write($"{value}");
        }

        public static void WriteNullable<TValue>(this IEnrichedBinaryWriter writer, TValue? value) where TValue : struct
        {
            writer.WriteObject(value, typeof(TValue?));
        }

        // Writes the value type and the value
        public static void WriteObject<TType>(this IEnrichedBinaryWriter writer, TType value)     // required for nullable types (need the type information)
        {
            writer.WriteObject(value, typeof(TType));
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
                : typeof(object);

            WriteEnumerable(writer, enumerable, argType);
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
                writer.WriteObject(value, valueType);
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
                writer.WriteObject(entry.Key, keyType);
                writer.WriteObject(entry.Value, valueType);
            }
        }
    }
}
