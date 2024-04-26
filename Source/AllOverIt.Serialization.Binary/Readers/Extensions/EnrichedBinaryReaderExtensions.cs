using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
using AllOverIt.Serialization.Binary.Exceptions;
using AllOverIt.Serialization.Binary.Writers;
using AllOverIt.Serialization.Binary.Writers.Extensions;
using System.Collections;

namespace AllOverIt.Serialization.Binary.Readers.Extensions
{
    /// <summary>Provides extension methods for <see cref="IEnrichedBinaryReader"/>.</summary>
    public static class EnrichedBinaryReaderExtensions
    {
        /// <summary>Reads a string from the current stream, including null values.</summary>
        /// <param name="reader">The reader that is reading from the current stream.</param>
        /// <returns>The read string. If the written string was null then null will be returned.</returns>
        /// <remarks>This method must be used in conjunction with <see cref="EnrichedBinaryWriterExtensions.WriteSafeString(IEnrichedBinaryWriter, string)"/>.</remarks>
        public static string ReadSafeString(this IEnrichedBinaryReader reader)
        {
            var hasValue = reader
                .WhenNotNull(nameof(reader))
                .ReadBoolean();

            return hasValue
                ? reader.ReadString()
                : default;
        }

        /// <summary>Reads a GUID from the current stream.</summary>
        /// <param name="reader">The reader that is reading from the current stream.</param>
        /// <returns>The read GUID.</returns>
        public static Guid ReadGuid(this IEnrichedBinaryReader reader)
        {
            var bytes = reader
                .WhenNotNull(nameof(reader))
                .ReadBytes(16);

            return new Guid(bytes);
        }

        /// <summary>Reads an enumeration from the current stream.</summary>
        /// <param name="reader">The reader that is reading from the current stream.</param>
        /// <returns>The read Enum.</returns>
        /// <remarks>This method must be used in conjunction with <see cref="EnrichedBinaryWriterExtensions.WriteEnum(IEnrichedBinaryWriter, object)"/>.</remarks>
        public static object ReadEnum(this IEnrichedBinaryReader reader)
        {
            _ = reader.WhenNotNull(nameof(reader));

            var enumType = GetEnumType(reader);

            var value = reader.ReadString();

            return Enum.Parse(enumType, value);
        }

        /// <summary>Reads an enumeration from the current stream.</summary>
        /// <typeparam name="TEnum">The enumeration type.</typeparam>
        /// <param name="reader">The reader that is reading from the current stream.</param>
        /// <returns>The read Enum.</returns>
        /// <remarks>This method must be used in conjunction with <see cref="EnrichedBinaryWriterExtensions.WriteEnum(IEnrichedBinaryWriter, object)"/>.</remarks>
        public static TEnum ReadEnum<TEnum>(this IEnrichedBinaryReader reader)
        {
            _ = reader.WhenNotNull(nameof(reader));

            var enumType = GetEnumType(reader);

            var value = reader.ReadString();

            return (TEnum) Enum.Parse(enumType, value);
        }

        /// <summary>Reads a <see cref="DateTime"/> from the current stream.</summary>
        /// <param name="reader">The reader that is reading from the current stream.</param>
        /// <returns>The read <see cref="DateTime"/>.</returns>
        public static DateTime ReadDateTime(this IEnrichedBinaryReader reader)
        {
            _ = reader.WhenNotNull(nameof(reader));

            var date = reader.ReadInt64();
            return DateTime.FromBinary(date);
        }

        /// <summary>Reads a <see cref="TimeSpan"/> from the current stream.</summary>
        /// <param name="reader">The reader that is reading from the current stream.</param>
        /// <returns>The read <see cref="TimeSpan"/>.</returns>
        public static TimeSpan ReadTimeSpan(this IEnrichedBinaryReader reader)
        {
            _ = reader.WhenNotNull(nameof(reader));

            var timespan = reader.ReadInt64();
            return new TimeSpan(timespan);
        }

        /// <summary>Reads an object from the current stream that was originally written using
        /// <see cref="EnrichedBinaryWriterExtensions.WriteObject{TType}(IEnrichedBinaryWriter, TType)"/>.</summary>
        /// <typeparam name="TValue">The value type to be read from the current stream.</typeparam>
        /// <param name="reader">The reader that is reading from the current stream.</param>
        /// <returns>The object read from the stream. If the value written was a collection then the value
        /// read will always be a <see cref="List{TValue}"/>.</returns>
        public static TValue ReadObject<TValue>(this IEnrichedBinaryReader reader)
        {
            return (TValue) reader
                .WhenNotNull(nameof(reader))
                .ReadObject();
        }

        /// <summary>Reads a typed dictionary from the current stream that was originally written using
        /// <see cref="EnrichedBinaryWriterExtensions.WriteObject{TType}(IEnrichedBinaryWriter, TType)"/>.</summary>
        /// <typeparam name="TKey">The dictionary key type to be read from the current stream.</typeparam>
        /// <typeparam name="TValue">The dictionary value type to be read from the current stream.</typeparam>
        /// <param name="reader">The reader that is reading from the current stream.</param>
        /// <returns>The object read from the stream.</returns>
        public static IDictionary<TKey, TValue> ReadObjectAsDictionary<TKey, TValue>(this IEnrichedBinaryReader reader)
        {
            return reader
                .WhenNotNull(nameof(reader))
                .ReadObject<Dictionary<object, object>>().ToDictionary(kvp => (TKey) kvp.Key, kvp => (TValue) kvp.Value);
        }

        /// <summary>Reads a nullable value from the current stream that was originally written using
        /// <see cref="EnrichedBinaryWriterExtensions.WriteNullable{TValue}(IEnrichedBinaryWriter, TValue?)"/>
        /// or <see cref="EnrichedBinaryWriterExtensions.WriteObject{TType}(IEnrichedBinaryWriter, TType)"/>.</summary>
        /// <typeparam name="TValue">The value type to be read from the current stream.</typeparam>
        /// <param name="reader">The reader that is reading from the current stream.</param>
        /// <returns>The value read from the stream.</returns>
        public static TValue? ReadNullable<TValue>(this IEnrichedBinaryReader reader) where TValue : struct
        {
            return (TValue?) reader
                .WhenNotNull(nameof(reader))
                .ReadObject();
        }

        /// <summary>Reads an array from the current stream that was originally written using
        /// <see cref="EnrichedBinaryWriterExtensions.WriteArray{TValue}(IEnrichedBinaryWriter, TValue[])"/> or
        /// <see cref="EnrichedBinaryWriterExtensions.WriteEnumerable(IEnrichedBinaryWriter, IEnumerable)"/> (or an overload)
        /// and returns it as an array (not a List).</summary>
        /// <param name="reader">The reader that is reading from the current stream.</param>
        /// <returns>The array read from the stream.</returns>
        public static object ReadArray(this IEnrichedBinaryReader reader)
        {
            var count = reader
              .WhenNotNull(nameof(reader))
              .ReadInt32();

            var assemblyTypeName = reader.ReadString();
            var elementType = Type.GetType(assemblyTypeName);

            var values = Array.CreateInstance(elementType, count);

            for (var i = 0; i < count; i++)
            {
                var value = reader.ReadObject();
                values.SetValue(value, i);
            }

            return values;
        }

        /// <summary>Reads an array from the current stream that was originally written using
        /// <see cref="EnrichedBinaryWriterExtensions.WriteArray{TValue}(IEnrichedBinaryWriter, TValue[])"/> or
        /// <see cref="EnrichedBinaryWriterExtensions.WriteEnumerable(IEnrichedBinaryWriter, IEnumerable)"/> (or an overload)
        /// and returns it as an array (not a List).</summary>
        /// <typeparam name="TValue">The array's element type. The actual element type will be read from the stream so
        /// if it doesn't match this type then an <see cref="InvalidCastException"/> will be raised.</typeparam>
        /// <param name="reader">The reader that is reading from the current stream.</param>
        /// <returns>The array read from the stream.</returns>
        public static TValue[] ReadArray<TValue>(this IEnrichedBinaryReader reader)
        {
            return (TValue[]) reader.ReadArray();
        }

        /// <summary>Reads an enumerable value from the current stream that was originally written using
        /// <see cref="EnrichedBinaryWriterExtensions.WriteEnumerable(IEnrichedBinaryWriter, IEnumerable)"/>
        /// or one of its overloads.</summary>
        /// <param name="reader">The reader that is reading from the current stream.</param>
        /// <returns>The IEnumerable read from the stream.</returns>
        public static IEnumerable ReadEnumerable(this IEnrichedBinaryReader reader)
        {
            var count = reader
                .WhenNotNull(nameof(reader))
                .ReadInt32();

            var assemblyTypeName = reader.ReadString();
            var elementType = Type.GetType(assemblyTypeName);

            if (count == 0)
            {
                return elementType.CreateList();
            }

            var values = elementType.CreateList();

            for (var i = 0; i < count; i++)
            {
                var value = reader.ReadObject();
                values.Add(value);
            }

            return values;
        }

        /// <summary>Reads an enumerable value from the current stream that was originally written using
        /// <see cref="EnrichedBinaryWriterExtensions.WriteEnumerable{TType}(IEnrichedBinaryWriter, IEnumerable{TType})"/>
        /// or one of its overloads.</summary>
        /// <typeparam name="TValue">The value type to be read from the current stream.</typeparam>
        /// <param name="reader">The reader that is reading from the current stream.</param>
        /// <returns>The IEnumerable read from the stream. The values are read as a List&lt;object&gt; and each value is cast
        /// to the <typeparamref name="TValue"/> type.</returns>
        public static IEnumerable<TValue> ReadEnumerable<TValue>(this IEnrichedBinaryReader reader)
        {
            return (IEnumerable<TValue>) reader.ReadEnumerable();
        }

        /// <summary>Reads a dictionary value from the current stream that was originally written using
        /// <see cref="EnrichedBinaryWriterExtensions.WriteDictionary(IEnrichedBinaryWriter, IDictionary)"/> or one of its overloads.</summary>
        /// <param name="reader">The reader that is reading from the current stream.</param>
        /// <returns>The IDictionary read from the stream, returned as IDictionary&lt;object, object&gt;.</returns>
        public static IDictionary<object, object> ReadDictionary(this IEnrichedBinaryReader reader)
        {
            var count = reader
                .WhenNotNull(nameof(reader))
                .ReadInt32();

            var values = new Dictionary<object, object>();

            for (var i = 0; i < count; i++)
            {
                var key = reader.ReadObject();
                var value = reader.ReadObject();

                values.Add(key, value);
            }

            return values;
        }

        /// <summary>Reads a dictionary value from the current stream that was originally written using
        /// <see cref="EnrichedBinaryWriterExtensions.WriteDictionary(IEnrichedBinaryWriter, IDictionary, Type, Type)"/>
        /// or one of its overloads.</summary>
        /// <typeparam name="TKey">The key type to be read from the current stream.</typeparam>
        /// <typeparam name="TValue">The value type to be read from the current stream.</typeparam>
        /// <param name="reader">The reader that is reading from the current stream.</param>
        /// <returns>The IDictionary read from the stream, after converting from IDictionary&lt;object, object&gt; to IDictionary&lt;TKey, TValue&gt;
        /// by casting the key and value values.</returns>
        public static IDictionary<TKey, TValue> ReadDictionary<TKey, TValue>(this IEnrichedBinaryReader reader)
        {
            var dictionary = reader
                .WhenNotNull(nameof(reader))
                .ReadDictionary();

            if (typeof(TKey) == CommonTypes.ObjectType && typeof(TValue) == CommonTypes.ObjectType)
            {
                return (IDictionary<TKey, TValue>) dictionary;
            }

            return dictionary.ToDictionary(kvp => (TKey) kvp.Key, kvp => (TValue) kvp.Value);
        }

        private static Type GetEnumType(IEnrichedBinaryReader reader)
        {
            var enumTypeName = reader.ReadString();
            var enumType = Type.GetType(enumTypeName);

            Throw<BinaryReaderException>.WhenNull(enumType, $"Unknown enum type '{enumTypeName}'.");

            return enumType;
        }
    }
}
