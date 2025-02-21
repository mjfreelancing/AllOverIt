using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
using AllOverIt.Serialization.Binary.Readers;
using AllOverIt.Serialization.Binary.Readers.Extensions;
using System.Collections;

namespace AllOverIt.Serialization.Binary.Writers.Extensions
{
    /// <summary>Provides extension methods for <see cref="IEnrichedBinaryWriter"/>.</summary>
    public static class EnrichedBinaryWriterExtensions
    {
        /// <inheritdoc cref="BinaryWriter.Write(ulong)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteUInt64(this IEnrichedBinaryWriter writer, ulong value) => writer.WhenNotNull().Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(uint)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteUInt32(this IEnrichedBinaryWriter writer, uint value) => writer.WhenNotNull().Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(ushort)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteUInt16(this IEnrichedBinaryWriter writer, ushort value) => writer.WhenNotNull().Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(string)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        /// <remarks>The value can null.</remarks>
        public static void WriteSafeString(this IEnrichedBinaryWriter writer, string? value)
        {
            _ = writer.WhenNotNull();

            var hasValue = value is not null;

            writer.Write(hasValue);

            if (hasValue)
            {
                writer.Write(value!);
            }
        }

        /// <inheritdoc cref="BinaryWriter.Write(float)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteSingle(this IEnrichedBinaryWriter writer, float value) => writer.WhenNotNull().Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(sbyte)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteSByte(this IEnrichedBinaryWriter writer, sbyte value) => writer.WhenNotNull().Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(ReadOnlySpan{char})"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteChars(this IEnrichedBinaryWriter writer, ReadOnlySpan<char> value) => writer.WhenNotNull().Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(ReadOnlySpan{byte})"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteBytes(this IEnrichedBinaryWriter writer, ReadOnlySpan<byte> value) => writer.WhenNotNull().Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(long)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteInt64(this IEnrichedBinaryWriter writer, long value) => writer.WhenNotNull().Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(int)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteInt32(this IEnrichedBinaryWriter writer, int value) => writer.WhenNotNull().Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(double)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteDouble(this IEnrichedBinaryWriter writer, double value) => writer.WhenNotNull().Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(decimal)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteDecimal(this IEnrichedBinaryWriter writer, decimal value) => writer.WhenNotNull().Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(char[], int, int)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        /// <param name="index">The index of the first character to read from the value and write to the stream.</param>
        /// <param name="count">The number of characters to read from the value and write to the stream.</param>
        public static void WriteChars(this IEnrichedBinaryWriter writer, char[] value, int index, int count) => writer.WhenNotNull().Write(value, index, count);

        /// <inheritdoc cref="BinaryWriter.Write(char[])"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteChars(this IEnrichedBinaryWriter writer, char[] value) => writer.WhenNotNull().Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(byte[], int, int)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        /// <param name="index">The index of the first character to read from the value and write to the stream.</param>
        /// <param name="count">The number of characters to read from the value and write to the stream.</param>
        public static void WriteBytes(this IEnrichedBinaryWriter writer, byte[] value, int index, int count) => writer.WhenNotNull().Write(value, index, count);

        /// <inheritdoc cref="BinaryWriter.Write(byte[])"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteBytes(this IEnrichedBinaryWriter writer, byte[] value) => writer.WhenNotNull().Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(byte)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteByte(this IEnrichedBinaryWriter writer, byte value) => writer.WhenNotNull().Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(bool)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteBoolean(this IEnrichedBinaryWriter writer, bool value) => writer.WhenNotNull().Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(short)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteInt16(this IEnrichedBinaryWriter writer, short value) => writer.WhenNotNull().Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(char)"/>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteChar(this IEnrichedBinaryWriter writer, char value) => writer.WhenNotNull().Write(value);

        /// <summary>Writes a GUID value to the current stream.</summary>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteGuid(this IEnrichedBinaryWriter writer, Guid value) => writer.WhenNotNull().Write(value.ToByteArray());

        /// <summary>Writes an Enum value to the current stream.</summary>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        /// <remarks>The enum's assembly qualified type name and it's string value are written, allowing
        /// <see cref="EnrichedBinaryReaderExtensions.ReadEnum(IEnrichedBinaryReader)"/> to read the value and create the appropriate enum type.</remarks>
        public static void WriteEnum(this IEnrichedBinaryWriter writer, object value)
        {
            _ = value.WhenNotNull();

            var valueType = value.GetType();

            if (!valueType!.IsEnum)
            {
                throw new ArgumentException($"{valueType.GetFriendlyName()} is not an enum type.", nameof(value));
            }

            // Need the string representation of the value in order to convert it back to the original Enum type.
            // Convert.ChangeType() cannot convert an integral type to an Enum type.
            writer
                .WhenNotNull()
                .Write(valueType.AssemblyQualifiedName!);

            writer.Write($"{value}");
        }

        /// <summary>Writes a <see cref="DateTime"/> value to the current stream.</summary>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteDateTime(this IEnrichedBinaryWriter writer, DateTime value) => writer.WhenNotNull().Write(value.ToBinary());

        /// <summary>Writes a <see cref="DateOnly"/> value to the current stream.</summary>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteDateOnly(this IEnrichedBinaryWriter writer, DateOnly value) => writer.WhenNotNull().Write(value.DayNumber);

        /// <summary>Writes a <see cref="TimeOnly"/> value to the current stream.</summary>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteTimeOnly(this IEnrichedBinaryWriter writer, TimeOnly value) => writer.WhenNotNull().Write(value.Ticks);

        /// <summary>Writes a TimeSpan value to the current stream.</summary>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value"></param>
        public static void WriteTimeSpan(this IEnrichedBinaryWriter writer, TimeSpan value) => writer.WhenNotNull().Write(value.Ticks);

        /// <summary>Writes a nullable value to the current stream.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteNullable<TValue>(this IEnrichedBinaryWriter writer, TValue? value) where TValue : struct
        {
            writer
                .WhenNotNull()
                .WriteObject(value, typeof(TValue?));
        }

        /// <summary>Writes an object, or value, to the current stream. This method supports the writing of regular scalar, IEnumerable, and IDictionary types
        /// but will result in a slightly larger stream compared to using the dedicated methods. This method should be primarily used for writing complex objects
        /// in conjunction with custom value writers.</summary>
        /// <typeparam name="TType">The value type.</typeparam>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="value">The value to be written.</param>
        public static void WriteObject<TType>(this IEnrichedBinaryWriter writer, TType value)     // required for nullable types (need the type information)
        {
            writer
                .WhenNotNull()
                .WriteObject(value, typeof(TType));
        }

        /// <summary>Writes an array to the current stream. All other arrays in the form of an object or an <see cref="IEnumerable"/> can
        /// be written using <see cref="WriteEnumerable(IEnrichedBinaryWriter, IEnumerable)"/>, including <see cref="ArrayList"/>.</summary>
        /// <typeparam name="TValue">The array's element type.</typeparam>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="array">The array of values to be written.</param>
        public static void WriteArray<TValue>(this IEnrichedBinaryWriter writer, TValue[] array)
        {
            _ = array.WhenNotNull();

            writer.WriteEnumerable(array);
        }

        /// <summary>Writes an <see cref="IEnumerable"/> to the current stream. Each value type will be determined by the generic
        /// argument of IEnumerable, if available, otherwise the runtime type of each value will be determined (which has a small
        /// additional overhead). Use generic IEnumerable's where possible. If the <see cref="IEnumerable"/> is a generic type then
        /// it is expected to have exactly one generic argument. Use the <see cref="WriteDictionary(IEnrichedBinaryWriter, IDictionary)"/>
        /// method, or one of its overloads, for dictionary values.
        /// <br />
        /// The number of elements and the element type is included in the stream to ensure the values are read back correctly.
        /// If the <see cref="IEnumerable"/> is not a generic type then the element type is recorded as an
        /// <see cref="object"/> so casting may be required when reading the values back.</summary>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="enumerable">The IEnumerable to be written.</param>
        public static void WriteEnumerable(this IEnrichedBinaryWriter writer, IEnumerable enumerable)
        {
            _ = writer.WhenNotNull();
            _ = enumerable.WhenNotNull();

            // Enumerable.Range()                    => returns a RangeIterator - no generic arguments
            // IEnumerable<int>                      => contains one generic argument
            // int?[]{}.Select(item => (object)item) => returns SelectEnumerableIterator<int?, object> - two generic arguments
            //

            var enumerableType = enumerable.GetType();

            // Due to the above potential edge cases this code is not using enumerableType.IsGenericEnumerableType()
            // as this does not (intentionally) cater for anything other than arrays and IEnumerable types.

            Type? elementType = null;         // Passed to WriteEnumerable() when enumerableType is IEnumerable

            if (enumerableType.IsArray)
            {
                elementType = enumerableType.GetElementType();
            }
            else if (enumerableType.IsGenericEnumerableType())
            {
                var genericsArguments = enumerableType.GetGenericArguments();

                if (genericsArguments.Length == 1)
                {
                    elementType = enumerableType.GetGenericArguments()[0];
                }
            }

            writer.WriteEnumerable(enumerable, elementType);
        }

        /// <summary>Writes an <see cref="IEnumerable{TType}"/> to the current stream.<br />
        /// The number of elements and the element type is included in the stream to ensure the values are read back correctly.</summary>
        /// <typeparam name="TType">The type of each value in the IEnumerable.</typeparam>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="enumerable">The IEnumerable to be written.</param>
        public static void WriteEnumerable<TType>(this IEnrichedBinaryWriter writer, IEnumerable<TType> enumerable)
        {
            _ = writer.WhenNotNull();
            _ = enumerable.WhenNotNull();

            writer.WriteEnumerable(enumerable, typeof(TType));
        }

        /// <summary>Writes an <see cref="IEnumerable"/> to the current stream. The number of elements and the element type is included
        /// in the stream to ensure the values are read back correctly.</summary>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="enumerable">The IEnumerable to be written.</param>
        /// <param name="valueType">The type of each value in the <see cref="IEnumerable{TType}"/>. If the <paramref name="valueType"/>
        /// is <see langword="null"/> then the runtime type of the first value read will be used. If the first value is <see langword="null"/>
        /// then object is used.</param>
        public static void WriteEnumerable(this IEnrichedBinaryWriter writer, IEnumerable enumerable, Type? valueType)
        {
            _ = writer.WhenNotNull();
            _ = enumerable.WhenNotNull();

            var elementType = valueType;

            if (elementType is null || enumerable is not ICollection collection)
            {
                collection = null!;
                IList? values = null;

                foreach (var value in enumerable)
                {
                    if (values is null)
                    {
                        elementType ??= value?.GetType() ?? CommonTypes.ObjectType;

                        // break out of the loop if all we needed was the elementType
                        if (enumerable is ICollection enumerableCollection)
                        {
                            collection = enumerableCollection;
                            break;
                        }

                        values = value is null
                            ? new List<object>()
                            : elementType.CreateListOf();
                    }

                    values.Add(value);
                }

                if (collection is null)
                {
                    if (values is null)
                    {
                        elementType = CommonTypes.ObjectType;
                        values = Array.Empty<object>();
                    }

                    collection = values;
                }
            }

            writer.Write(collection.Count);

            // The element type is required so ReadEnumerable knows the type of list to create
            var assemblyTypeName = elementType!.AssemblyQualifiedName!;
            writer.Write(assemblyTypeName);

            if (collection.Count > 0)
            {
                foreach (var value in collection)
                {
                    writer.WriteObject(value, elementType);
                }
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
            _ = writer.WhenNotNull();
            _ = dictionary.WhenNotNull();

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
                keyType = CommonTypes.ObjectType;
                valueType = CommonTypes.ObjectType;
            }

            writer.WriteDictionary(dictionary, keyType, valueType);
        }

        /// <summary>>Writes an IDictionary to the current stream.</summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="dictionary">The IDictionary to be written.</param>
        /// <remarks>This method cannot exist as an overload of <see cref="WriteDictionary(IEnrichedBinaryWriter, IDictionary)"/>
        /// without becoming ambiguous.</remarks>
        public static void WriteDictionary<TKey, TValue>(this IEnrichedBinaryWriter writer, IDictionary<TKey, TValue> dictionary)
        {
            _ = writer.WhenNotNull();
            _ = dictionary.WhenNotNull();

            writer.WriteDictionary((IDictionary)dictionary, typeof(TKey), typeof(TValue));
        }

        /// <summary>Writes an IDictionary to the current stream.</summary>
        /// <param name="writer">The binary writer that is writing to the current stream.</param>
        /// <param name="dictionary">The IDictionary to be written.</param>
        /// <param name="keyType">The key type.</param>
        /// <param name="valueType">The value type.</param>
        public static void WriteDictionary(this IEnrichedBinaryWriter writer, IDictionary dictionary, Type keyType, Type valueType)
        {
            _ = writer.WhenNotNull();
            _ = dictionary.WhenNotNull();

            writer.Write(dictionary.Count);

            foreach (DictionaryEntry entry in dictionary)
            {
                writer.WriteObject(entry.Key, keyType);
                writer.WriteObject(entry.Value, valueType);
            }
        }
    }
}
