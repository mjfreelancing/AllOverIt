using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Serialization.Binary.Exceptions;
using AllOverIt.Serialization.Binary.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AllOverIt.Serialization.Binary
{


    public interface IEnrichedBinaryValueWriter
    {
        Type Type { get; }
        void WriteValue(EnrichedBinaryWriter writer, object value);
    }


    public abstract class EnrichedBinaryValueWriter<TType> : IEnrichedBinaryValueWriter
    {
        public Type Type => typeof(TType);

        public abstract void WriteValue(EnrichedBinaryWriter writer, object value);
    }





    public interface IEnrichedBinaryWriter
    {
        IList<IEnrichedBinaryValueWriter> Writers { get; }

        #region BinaryWriter

        /// <summary>Closes the current writer and the underlying stream.</summary>
        void Close();

        /// <summary>Clears all buffers for the current writer and causes any buffered data to be written to the underlying device.</summary>
        void Flush();
        
        /// <summary>Sets the position within the current stream.</summary>
        /// <param name="offset">A byte offset relative to origin.</param>
        /// <param name="origin">Indicates the reference point from which the new position is to be obtained.</param>
        /// <returns>The position with the current stream.</returns>
        long Seek(int offset, SeekOrigin origin);
        
        //
        // Summary:
        //     Writes an eight-byte unsigned integer to the current stream and advances the
        //     stream position by eight bytes.
        //
        // Parameters:
        //   value:
        //     The eight-byte unsigned integer to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(ulong value);

        //
        // Summary:
        //     Writes a four-byte unsigned integer to the current stream and advances the stream
        //     position by four bytes.
        //
        // Parameters:
        //   value:
        //     The four-byte unsigned integer to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(uint value);

        //
        // Summary:
        //     Writes a two-byte unsigned integer to the current stream and advances the stream
        //     position by two bytes.
        //
        // Parameters:
        //   value:
        //     The two-byte unsigned integer to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(ushort value);
        
        //
        // Summary:
        //     Writes a length-prefixed string to this stream in the current encoding of the
        //     System.IO.BinaryWriter, and advances the current position of the stream in accordance
        //     with the encoding used and the specific characters being written to the stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ArgumentNullException:
        //     value is null.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(string value);

        //
        // Summary:
        //     Writes a four-byte floating-point value to the current stream and advances the
        //     stream position by four bytes.
        //
        // Parameters:
        //   value:
        //     The four-byte floating-point value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(float value);

        //
        // Summary:
        //     Writes a signed byte to the current stream and advances the stream position by
        //     one byte.
        //
        // Parameters:
        //   value:
        //     The signed byte to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(sbyte value);

#if NETSTANDARD2_1 || NETCOREAPP3_1_OR_GREATER
        //
        // Summary:
        //     Writes a span of characters to the current stream, and advances the current position
        //     of the stream in accordance with the Encoding used and perhaps the specific characters
        //     being written to the stream.
        //
        // Parameters:
        //   chars:
        //     A span of chars to write.
        void Write(ReadOnlySpan<char> chars);

        //
        // Summary:
        //     Writes a span of bytes to the current stream.
        //
        // Parameters:
        //   buffer:
        //     The span of bytes to write.
        void Write(ReadOnlySpan<byte> buffer);
#endif

        //
        // Summary:
        //     Writes an eight-byte signed integer to the current stream and advances the stream
        //     position by eight bytes.
        //
        // Parameters:
        //   value:
        //     The eight-byte signed integer to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(long value);

        //
        // Summary:
        //     Writes a four-byte signed integer to the current stream and advances the stream
        //     position by four bytes.
        //
        // Parameters:
        //   value:
        //     The four-byte signed integer to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(int value);

        //
        // Summary:
        //     Writes an eight-byte floating-point value to the current stream and advances
        //     the stream position by eight bytes.
        //
        // Parameters:
        //   value:
        //     The eight-byte floating-point value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(double value);

        //
        // Summary:
        //     Writes a decimal value to the current stream and advances the stream position
        //     by sixteen bytes.
        //
        // Parameters:
        //   value:
        //     The decimal value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(decimal value);

        //
        // Summary:
        //     Writes a section of a character array to the current stream, and advances the
        //     current position of the stream in accordance with the Encoding used and perhaps
        //     the specific characters being written to the stream.
        //
        // Parameters:
        //   chars:
        //     A character array containing the data to write.
        //
        //   index:
        //     The index of the first character to read from chars and to write to the stream.
        //
        //   count:
        //     The number of characters to read from chars and to write to the stream.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The buffer length minus index is less than count.
        //
        //   T:System.ArgumentNullException:
        //     chars is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     index or count is negative.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(char[] chars, int index, int count);

        //
        // Summary:
        //     Writes a character array to the current stream and advances the current position
        //     of the stream in accordance with the Encoding used and the specific characters
        //     being written to the stream.
        //
        // Parameters:
        //   chars:
        //     A character array containing the data to write.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     chars is null.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        void Write(char[] chars);

        //
        // Summary:
        //     Writes a region of a byte array to the current stream.
        //
        // Parameters:
        //   buffer:
        //     A byte array containing the data to write.
        //
        //   index:
        //     The index of the first byte to read from buffer and to write to the stream.
        //
        //   count:
        //     The number of bytes to read from buffer and to write to the stream.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The buffer length minus index is less than count.
        //
        //   T:System.ArgumentNullException:
        //     buffer is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     index or count is negative.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(byte[] buffer, int index, int count);

        //
        // Summary:
        //     Writes a byte array to the underlying stream.
        //
        // Parameters:
        //   buffer:
        //     A byte array containing the data to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.ArgumentNullException:
        //     buffer is null.
        void Write(byte[] buffer);

        //
        // Summary:
        //     Writes an unsigned byte to the current stream and advances the stream position
        //     by one byte.
        //
        // Parameters:
        //   value:
        //     The unsigned byte to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(byte value);

        //
        // Summary:
        //     Writes a one-byte Boolean value to the current stream, with 0 representing false
        //     and 1 representing true.
        //
        // Parameters:
        //   value:
        //     The Boolean value to write (0 or 1).
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(bool value);

        //
        // Summary:
        //     Writes a two-byte signed integer to the current stream and advances the stream
        //     position by two bytes.
        //
        // Parameters:
        //   value:
        //     The two-byte signed integer to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(short value);

        //
        // Summary:
        //     Writes a Unicode character to the current stream and advances the current position
        //     of the stream in accordance with the Encoding used and the specific characters
        //     being written to the stream.
        //
        // Parameters:
        //   ch:
        //     The non-surrogate, Unicode character to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.ArgumentException:
        //     ch is a single surrogate character.
        void Write(char ch);

        #endregion

        // Includes support for enumerable and dictionary. Methods also exist for these (as extensions) purely for readability reasons.
        void WriteObject(object value);
        void WriteObject(Type type, object value);




        //void WriteNullable<TValue>(TValue? value) where TValue : struct;
        //void WriteObject<TType>(TType value);

    }


    public sealed class EnrichedBinaryWriter : BinaryWriter, IEnrichedBinaryWriter
    {
        private readonly Type ObjectType = typeof(object);

        private static readonly IDictionary<Type, TypeMapping.TypeId> TypeIdRegistry = new Dictionary<Type, TypeMapping.TypeId>
        {
            { typeof(bool), TypeMapping.TypeId.Bool },
            { typeof(byte), TypeMapping.TypeId.Byte },
            { typeof(sbyte), TypeMapping.TypeId.SByte },
            { typeof(ushort), TypeMapping.TypeId.UShort },
            { typeof(short), TypeMapping.TypeId.Short },
            { typeof(uint), TypeMapping.TypeId.UInt },
            { typeof(int), TypeMapping.TypeId.Int },
            { typeof(ulong), TypeMapping.TypeId.ULong },
            { typeof(long), TypeMapping.TypeId.Long },
            { typeof(float), TypeMapping.TypeId.Float },
            { typeof(double), TypeMapping.TypeId.Double },
            { typeof(decimal), TypeMapping.TypeId.Decimal },
            { typeof(string), TypeMapping.TypeId.String },
            { typeof(char), TypeMapping.TypeId.Char },
            { typeof(Enum), TypeMapping.TypeId.Enum },
            { typeof(Guid), TypeMapping.TypeId.Guid },
            { typeof(DateTime), TypeMapping.TypeId.DateTime },
            { typeof(TimeSpan), TypeMapping.TypeId.TimeSpan }
        };

        private static readonly IDictionary<TypeMapping.TypeId, Action<EnrichedBinaryWriter, object>> TypeIdWriter =
            new Dictionary<TypeMapping.TypeId, Action<EnrichedBinaryWriter, object>>
        {
            { TypeMapping.TypeId.Bool, (writer, value) => writer.WriteBoolean((bool)value) },
            { TypeMapping.TypeId.Byte, (writer, value) => writer.WriteByte((byte)value) },
            { TypeMapping.TypeId.SByte, (writer, value) => writer.WriteSByte((sbyte)value) },
            { TypeMapping.TypeId.UShort, (writer, value) => writer.WriteUInt16((ushort)value) },
            { TypeMapping.TypeId.Short, (writer, value) => writer.WriteInt16((short)value) },
            { TypeMapping.TypeId.UInt, (writer, value) => writer.WriteUInt32((uint)value) },
            { TypeMapping.TypeId.Int, (writer, value) => writer.WriteInt32((int)value) },
            { TypeMapping.TypeId.ULong, (writer, value) => writer.WriteUInt64((ulong)value) },
            { TypeMapping.TypeId.Long, (writer, value) => writer.WriteInt64((long)value) },
            { TypeMapping.TypeId.Float, (writer, value) => writer.WriteSingle((float)value) },
            { TypeMapping.TypeId.Double, (writer, value) => writer.WriteDouble((double)value) },
            { TypeMapping.TypeId.Decimal, (writer, value) => writer.WriteDecimal((decimal)value) },
            { TypeMapping.TypeId.String, (writer, value) => writer.WriteSafeString((string)value) },
            { TypeMapping.TypeId.Char, (writer, value) => writer.WriteChar((char)value) },
            { TypeMapping.TypeId.Enum, (writer, value) => writer.WriteEnum(value) },
            { TypeMapping.TypeId.Guid, (writer, value) => writer.WriteGuid((Guid)value) },
            { TypeMapping.TypeId.DateTime, (writer, value) => writer.WriteInt64(((DateTime)value).ToBinary()) },
            { TypeMapping.TypeId.TimeSpan, (writer, value) => writer.WriteInt64(((TimeSpan)value).Ticks) },

            {
                    TypeMapping.TypeId.Dictionary, (writer, value) =>
                    {
                        writer.WriteDictionary((IDictionary)value, (valueType, value) => writer.WriteObject(valueType, value));
                    }
            },
            
            {
                    TypeMapping.TypeId.Enumerable, (writer, value) =>
                    {
                        // valueType can be null if the enumerable come from something like Enumerable.Range(); WriteObject() deals with this.
                         writer.WriteEnumerable((IEnumerable)value, (valueType, value) => writer.WriteObject(valueType, value));
                    }
            },

            {
                TypeMapping.TypeId.Cached, (writer, value) =>
                {
                    var valueType = value.GetType();
                    var assemblyTypeName = valueType.AssemblyQualifiedName;

                    var index = writer._userDefinedTypeCache[assemblyTypeName];
                    writer.Write(index);

                    var converter = writer.Writers.SingleOrDefault(converter => converter.Type == valueType);
                    converter.WriteValue(writer, value);
                }
            },

            {
                TypeMapping.TypeId.UserDefined, (writer, value) =>
                {
                    var valueType = value.GetType();
                    var assemblyTypeName = valueType.AssemblyQualifiedName;

                    // cache for later, to write the value as a cached user defined type
                    var cacheIndex = writer._userDefinedTypeCache.Keys.Count + 1;
                    writer._userDefinedTypeCache.Add(assemblyTypeName, cacheIndex);

                    var converter = writer.Writers.SingleOrDefault(converter => converter.Type == valueType);

                    if (converter == null)
                    {
                        throw new BinaryWriterException($"No binary writer registered for the type '{valueType.GetFriendlyName()}'.");
                    }

                    writer.Write(assemblyTypeName);
                    converter.WriteValue(writer, value);
                }
            }
        };

        private readonly IDictionary<string, int> _userDefinedTypeCache = new Dictionary<string, int>();
        private readonly IReadOnlyCollection<Func<Type, object, TypeMapping.TypeId?>> _typeIdLookups;

        public IList<IEnrichedBinaryValueWriter> Writers { get; } = new List<IEnrichedBinaryValueWriter>();

        /// <inheritdoc cref="BinaryWriter(Stream)"/>
        public EnrichedBinaryWriter(Stream output)
            : base(output)
        {
            _typeIdLookups = GetTypeIdLookups();
        }

        /// <inheritdoc cref="BinaryWriter(Stream, Encoding)"/>
        public EnrichedBinaryWriter(Stream output, Encoding encoding)
            : base(output, encoding)
        {
            _typeIdLookups = GetTypeIdLookups();
        }

        /// <inheritdoc cref="BinaryWriter(Stream, Encoding, bool)"/>
        public EnrichedBinaryWriter(Stream output, Encoding encoding, bool leaveOpen)
            : base(output, encoding, leaveOpen)
        {
            _typeIdLookups = GetTypeIdLookups();
        }

        // Writes the value type and the value
        public void WriteObject(object value)
        {
            _ = value.WhenNotNull(nameof(value));

            WriteObject(value.GetType(), value);
        }

        public void WriteObject(Type type, object value)
        {
            // Including null checking in case the values come from something like Enumerable.Range()
            if (type is null || type == ObjectType)
            {
                type = value.GetType();
            }

            if (value is null && (type is null || type == ObjectType))
            {
                throw new BinaryWriterException("All binary serialized values must be typed or have a non-null value.");
            }

            var rawTypeId = GetRawTypeId(type, value);

            var typeId = (byte) rawTypeId;

            if (value == default)
            {
                typeId |= (byte) TypeMapping.TypeId.DefaultValue;
            }

            this.WriteByte(typeId);

            if ((typeId & (byte) TypeMapping.TypeId.DefaultValue) == 0)
            {
                TypeIdWriter[rawTypeId].Invoke(this, value);
            }
        }



        //public void WriteNullable<TValue>(TValue? value) where TValue : struct
        //{
        //    WriteObject(typeof(TValue?), value);
        //}

        //// Writes the value type and the value
        //public void WriteObject<TType>(TType value)     // required for nullable types (need the type information)
        //{
        //    WriteObject(typeof(TType), value);
        //}

        private IReadOnlyCollection<Func<Type, object, TypeMapping.TypeId?>> GetTypeIdLookups()
        {
            return new[]
            {
                IsTypeRegistered,
                IsTypeEnum,
                IsTypeNullable,
                IsDictionary,           // Make sure this is before IsEnumerable
                IsEnumerable,
                GetTypeAsCachedOrUserDefined
            };
        }

        private TypeMapping.TypeId GetRawTypeId(Type type, object value)
        {
            foreach (var lookup in _typeIdLookups)
            {
                var rawTypeId = lookup.Invoke(type, value);

                if (rawTypeId.HasValue)
                {
                    return rawTypeId.Value;
                }
            }

            throw new InvalidOperationException($"No RawTypeId lookup defined for type '{type.GetFriendlyName()}'.");
        }

        private TypeMapping.TypeId? IsTypeRegistered(Type type, object value)
        {
            if (TypeIdRegistry.TryGetValue(type, out var rawTypeId))
            {
                return rawTypeId;
            }

            return default;
        }

        private TypeMapping.TypeId? IsTypeEnum(Type type, object value)
        {
            if (type.IsEnum)
            {
                return TypeMapping.TypeId.Enum;
            }

            return default;
        }

        private TypeMapping.TypeId? IsTypeNullable(Type type, object _)
        {
            if (type.IsNullableType())
            {
                var underlyingType = Nullable.GetUnderlyingType(type);

                if (TypeIdRegistry.TryGetValue(underlyingType, out var rawTypeId))
                {
                    return rawTypeId;
                }
            }

            return default;
        }

        private TypeMapping.TypeId? IsDictionary(Type type, object _)
        {
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                return TypeMapping.TypeId.Dictionary;
            }

            return default;
        }

        private TypeMapping.TypeId? IsEnumerable(Type type, object _)
        {
            // Not checking for strings since this is a pre-registered type
            if (type.IsArray || type.IsEnumerableType())
            {
                // We could attempt to write the type once if it is a IEnumerable<T> but this gets
                // more complicated with nullable and anonymous or object types - opting for convenience
                // and simplicity over stream size.

                return TypeMapping.TypeId.Enumerable;
            }

            return default;
        }

        private TypeMapping.TypeId? GetTypeAsCachedOrUserDefined(Type _, object value)
        {
            var valueType = value.GetType();
            var assemblyTypeName = valueType.AssemblyQualifiedName;

            return _userDefinedTypeCache.ContainsKey(assemblyTypeName)
                ? TypeMapping.TypeId.Cached
                : TypeMapping.TypeId.UserDefined;
        }
    }











    // TODO: May be use this concept for types that have no custom writer - so use some extension methods



    //public sealed class ObjectBinaryWriter
    //{
    //    private readonly Type StringType = typeof(string);
    //    private readonly Type ObjectType = typeof(object);

    //    private readonly IEnrichedBinaryWriter _writer;

    //    public ObjectBinaryWriter(IEnrichedBinaryWriter writer)
    //    {
    //        _writer = writer.WhenNotNull(nameof(writer));
    //    }

    //    public void WriteObject<TType>(TType @object)
    //    {
    //        WriteObject(typeof(TType), @object);
    //    }

    //    private void WriteObject(Type objectType, object @object)
    //    {
    //        // check if there is a registered writer - if so use that....



    //        // otherwise....
    //        switch (@object)
    //        {
    //            case IDictionary dictionary:
    //                WriteDictionary(dictionary);
    //                break;

    //            case IEnumerable enumerable when @object.GetType() != typeof(string):
    //                _writer.Write((byte)TypeMapping.TypeId.Enumerable);
    //                _writer.WriteEnumerable(enumerable, (valueType, value) => WriteObject(valueType, value));
    //                break;

    //            default:
    //                if (!objectType.IsClass || objectType == StringType)
    //                {
    //                    _writer.WriteObject(objectType, @object);
    //                }
    //                else
    //                {
    //                    _writer.Write((byte) TypeMapping.TypeId.UserDefined);

    //                    var assemblyTypeName = objectType.AssemblyQualifiedName;
    //                    _writer.Write(assemblyTypeName);

    //                    WriteObjectProperties(@object, objectType);
    //                }
    //                break;
    //        }
    //    }

    //    private void WriteDictionary(IDictionary dictionary)
    //    {
    //        var argTypes = dictionary.GetType().GetGenericArguments();

    //        if (argTypes.NotAny())
    //        {
    //            // can't process null values unless we know the type
    //            throw new BinaryWriterException($"All {nameof(IDictionary)} properties must be typed.");
    //        }

    //        var keyType = argTypes[0];
    //        var valueType = argTypes[1];

    //        var values = new List<KeyValuePair<object, object>>();

    //        var keyEnumerator = dictionary.Keys.GetEnumerator();
    //        var valueEnumerator = dictionary.Values.GetEnumerator();

    //        while (keyEnumerator.MoveNext())
    //        {
    //            valueEnumerator.MoveNext();

    //            var key = keyEnumerator.Current;

    //            if (key == null && keyType == ObjectType)
    //            {
    //                throw new BinaryWriterException($"All {nameof(IDictionary)} keys must be typed or have a non-null value.");
    //            }

    //            var value = valueEnumerator.Current;

    //            if (value == null && valueType == ObjectType)
    //            {
    //                throw new BinaryWriterException($"All {nameof(IDictionary)} values must be typed or have a non-null value.");
    //            }

    //            var kvp = new KeyValuePair<object, object>(key, value);

    //            values.Add(kvp);
    //        }

    //        _writer.Write(values.Count / 2);

    //        foreach (var value in values)
    //        {
    //            WriteObject(keyType == ObjectType ? value.Key.GetType() : keyType, value.Key);
    //            WriteObject(valueType == ObjectType ? value.Value.GetType() : valueType, value.Value);
    //        }
    //    }

    //    private void WriteObjectProperties(object @object, Type objectType)
    //    {
    //        var properties = objectType.GetProperties();    // only interested in public properties at this stage

    //        foreach (var property in properties)
    //        {
    //            var value = property.GetValue(@object);
    //            WriteObject(property.PropertyType, value);
    //        }
    //    }

    //}





}
