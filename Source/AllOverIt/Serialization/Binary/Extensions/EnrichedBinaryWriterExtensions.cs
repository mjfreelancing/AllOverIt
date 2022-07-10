﻿using AllOverIt.Assertion;
using AllOverIt.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AllOverIt.Serialization.Binary.Extensions
{
    /// <summary>Provides extension methods for <see cref="IEnrichedBinaryWriter"/>.</summary>
    public static class EnrichedBinaryWriterExtensions
    {
        private static readonly Type ObjectType = typeof(object);

        /// <inheritdoc cref="BinaryWriter.Write(ulong)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        public static void WriteUInt64(this IEnrichedBinaryWriter writer, ulong value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(uint)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        public static void WriteUInt32(this IEnrichedBinaryWriter writer, uint value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(ushort)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        public static void WriteUInt16(this IEnrichedBinaryWriter writer, ushort value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(string)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
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
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        public static void WriteSingle(this IEnrichedBinaryWriter writer, float value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(sbyte)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        public static void WriteSByte(this IEnrichedBinaryWriter writer, sbyte value) => writer.Write(value);

#if NETSTANDARD2_1 || NETCOREAPP3_1_OR_GREATER
        /// <inheritdoc cref="BinaryWriter.Write(ReadOnlySpan{char})"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        public static void WriteChars(this IEnrichedBinaryWriter writer, ReadOnlySpan<char> chars) => writer.Write(chars);

        /// <inheritdoc cref="BinaryWriter.Write(ReadOnlySpan{byte})"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        public static void WriteBytes(this IEnrichedBinaryWriter writer, ReadOnlySpan<byte> buffer) => writer.Write(buffer);
#endif

        /// <inheritdoc cref="BinaryWriter.Write(long)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        public static void WriteInt64(this IEnrichedBinaryWriter writer, long value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(int)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        public static void WriteInt32(this IEnrichedBinaryWriter writer, int value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(double)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        public static void WriteDouble(this IEnrichedBinaryWriter writer, double value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(decimal)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        public static void WriteDecimal(this IEnrichedBinaryWriter writer, decimal value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(char[], int, int)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        public static void WriteChars(this IEnrichedBinaryWriter writer, char[] chars, int index, int count) => writer.Write(chars, index, count);

        /// <inheritdoc cref="BinaryWriter.Write(char[])"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        public static void WriteChars(this IEnrichedBinaryWriter writer, char[] chars) => writer.Write(chars);

        /// <inheritdoc cref="BinaryWriter.Write(byte[], int, int)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        public static void WriteBytes(this IEnrichedBinaryWriter writer, byte[] buffer, int index, int count) => writer.Write(buffer, index, count);

        /// <inheritdoc cref="BinaryWriter.Write(byte[])"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        public static void WriteBytes(this IEnrichedBinaryWriter writer, byte[] buffer) => writer.Write(buffer);

        /// <inheritdoc cref="BinaryWriter.Write(byte)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        public static void WriteByte(this IEnrichedBinaryWriter writer, byte value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(bool)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        public static void WriteBoolean(this IEnrichedBinaryWriter writer, bool value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(short)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        public static void WriteInt16(this IEnrichedBinaryWriter writer, short value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(char)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        public static void WriteChar(this IEnrichedBinaryWriter writer, char value) => writer.Write(value);

        /// <summary>Writes a GUID value to the current stream.</summary>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        public static void WriteGuid(this IEnrichedBinaryWriter writer, Guid value) => writer.WriteBytes(value.ToByteArray());

        /// <summary>Writes an Enum value to the current stream.</summary>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
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

        /// <summary>Writes an object, or value, to the current stream. This method supports the writing of regular scalar, IEnumerable, and IDictionary types
        /// but will result in a slightly larger stream compared to using the dedicated methods. This method should be primarily used for writing complex objects
        /// in conjunction with custom value writers.</summary>
        /// <typeparam name="TType">The value type.</typeparam>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteObject<TType>(this IEnrichedBinaryWriter writer, TType value)     // required for nullable types (need the type information)
        {
            writer.WriteObject(value, typeof(TType));
        }

        /// <summary>Writes an IEnumerable to the current stream. Each value type will be determined by the generic argument of IEnumerable, if available,
        /// otherwise the runtime type of each value will be determined (which has a small additional overhead). Use generic IEnumerable's where possible.</summary>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="enumerable">The IEnumerable to be written.</param>
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
                : ObjectType;

            WriteEnumerable(writer, enumerable, argType);
        }

        /// <summary>Writes an IEnumerable to the current stream.</summary>
        /// <typeparam name="TType">The type of each value in the IEnumerable.</typeparam>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="enumerable">The IEnumerable to be written.</param>
        public static void WriteEnumerable<TType>(this IEnrichedBinaryWriter writer, IEnumerable<TType> enumerable)
        {
            _ = enumerable.WhenNotNull(nameof(enumerable));

            WriteEnumerable(writer, enumerable, typeof(TType));
        }

        /// <summary>Writes an IEnumerable to the current stream.</summary>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="enumerable">The IEnumerable to be written.</param>
        /// <param name="valueType">The type of each value in the IEnumerable.</param>
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

        // This method exists so it supports methods such as Environment.GetEnvironmentVariables() which returns IDictionary.
        //
        /// <summary>Writes an IDictionary to the current stream. Each key and value type will be determined by the generic argument of IDictionary, if available,
        /// otherwise the runtime type of each key and value will be determined (which has a small additional overhead). Use generic IDictionary's where possible.</summary>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="dictionary">The IDictionary to be written.</param>
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
                keyType = ObjectType;
                valueType = ObjectType;
            }

            WriteDictionary(writer, dictionary, keyType, valueType);
        }

        /// <summary>>Writes an IDictionary to the current stream.</summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="dictionary">The IDictionary to be written.</param>
        /// <remarks>This method cannot exist as an overload of <see cref="EnrichedBinaryWriterExtensions.WriteDictionary(IEnrichedBinaryWriter, IDictionary)"/>
        /// without becoming ambigious.</remarks>
        public static void WriteTypedDictionary<TKey, TValue>(this IEnrichedBinaryWriter writer, IDictionary<TKey, TValue> dictionary)
        {
            _ = dictionary.WhenNotNull(nameof(dictionary));

            WriteDictionary(writer, (IDictionary) dictionary, typeof(TKey), typeof(TValue));
        }

        /// <summary>Writes an IDictionary to the current stream.</summary>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="dictionary">The IDictionary to be written.</param>
        /// <param name="keyType">The key type.</param>
        /// <param name="valueType">The value type.</param>
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