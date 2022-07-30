using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
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
    /// <inheritdoc cref="IEnrichedBinaryWriter"/>
    public sealed class EnrichedBinaryWriter : BinaryWriter, IEnrichedBinaryWriter
    {
        private static readonly IDictionary<Type, TypeIdentifier> TypeIdRegistry = new Dictionary<Type, TypeIdentifier>
        {
            { CommonTypes.BoolType, TypeIdentifier.Bool },
            { CommonTypes.ByteType, TypeIdentifier.Byte },
            { CommonTypes.SByteType, TypeIdentifier.SByte },
            { CommonTypes.UShortType, TypeIdentifier.UShort },
            { CommonTypes.ShortType, TypeIdentifier.Short },
            { CommonTypes.UIntType, TypeIdentifier.UInt },
            { CommonTypes.IntType, TypeIdentifier.Int },
            { CommonTypes.ULongType, TypeIdentifier.ULong },
            { CommonTypes.LongType, TypeIdentifier.Long },
            { CommonTypes.FloatType, TypeIdentifier.Float },
            { CommonTypes.DoubleType, TypeIdentifier.Double },
            { CommonTypes.DecimalType, TypeIdentifier.Decimal },
            { CommonTypes.StringType, TypeIdentifier.String },
            { CommonTypes.CharType, TypeIdentifier.Char },
            { CommonTypes.EnumType, TypeIdentifier.Enum },
            { CommonTypes.GuidType, TypeIdentifier.Guid },
            { CommonTypes.DateTimeType, TypeIdentifier.DateTime },
            { CommonTypes.TimeSpanType, TypeIdentifier.TimeSpan }
        };

        private static readonly IDictionary<TypeIdentifier, Action<EnrichedBinaryWriter, object>> TypeIdWriter =
            new Dictionary<TypeIdentifier, Action<EnrichedBinaryWriter, object>>
        {
            { TypeIdentifier.Bool, (writer, value) => writer.WriteBoolean((bool)value) },
            { TypeIdentifier.Byte, (writer, value) => writer.WriteByte((byte)value) },
            { TypeIdentifier.SByte, (writer, value) => writer.WriteSByte((sbyte)value) },
            { TypeIdentifier.UShort, (writer, value) => writer.WriteUInt16((ushort)value) },
            { TypeIdentifier.Short, (writer, value) => writer.WriteInt16((short)value) },
            { TypeIdentifier.UInt, (writer, value) => writer.WriteUInt32((uint)value) },
            { TypeIdentifier.Int, (writer, value) => writer.WriteInt32((int)value) },
            { TypeIdentifier.ULong, (writer, value) => writer.WriteUInt64((ulong)value) },
            { TypeIdentifier.Long, (writer, value) => writer.WriteInt64((long)value) },
            { TypeIdentifier.Float, (writer, value) => writer.WriteSingle((float)value) },
            { TypeIdentifier.Double, (writer, value) => writer.WriteDouble((double)value) },
            { TypeIdentifier.Decimal, (writer, value) => writer.WriteDecimal((decimal)value) },
            { TypeIdentifier.String, (writer, value) => writer.WriteSafeString((string)value) },
            { TypeIdentifier.Char, (writer, value) => writer.WriteChar((char)value) },
            { TypeIdentifier.Enum, (writer, value) => writer.WriteEnum(value) },
            { TypeIdentifier.Guid, (writer, value) => writer.WriteGuid((Guid)value) },
            { TypeIdentifier.DateTime, (writer, value) => writer.WriteInt64(((DateTime)value).ToBinary()) },
            { TypeIdentifier.TimeSpan, (writer, value) => writer.WriteInt64(((TimeSpan)value).Ticks) },
            { TypeIdentifier.Dictionary, (writer, value) => writer.WriteDictionary((IDictionary)value) },
            { TypeIdentifier.Enumerable, (writer, value) => writer.WriteEnumerable((IEnumerable)value) },

            {
                TypeIdentifier.Cached, (writer, value) =>
                {
                    var valueType = value.GetType();
                    var assemblyTypeName = valueType.AssemblyQualifiedName;

                    var index = writer._userDefinedTypeCache[assemblyTypeName];
                    writer.Write(index);

                    var converter = writer.Writers.Single(converter => converter.Type == valueType);
                    converter.WriteValue(writer, value);
                }
            },

            {
                TypeIdentifier.UserDefined, (writer, value) =>
                {
                    var valueType = value.GetType();
                    var assemblyTypeName = valueType.AssemblyQualifiedName;

                    // cache for later, to write the value as a cached user defined type
                    var cacheIndex = writer._userDefinedTypeCache.Keys.Count + 1;
                    writer._userDefinedTypeCache.Add(assemblyTypeName, cacheIndex);

                    var converter = writer.Writers.Single(converter => converter.Type == valueType);

                    writer.Write(assemblyTypeName);
                    converter.WriteValue(writer, value);
                }
            }
        };

        private readonly IDictionary<string, int> _userDefinedTypeCache = new Dictionary<string, int>();
        private readonly IReadOnlyCollection<Func<Type, TypeIdentifier?>> _typeIdLookups;

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

        /// <inheritdoc />
        public void WriteObject(object value)
        {
            _ = value.WhenNotNull(nameof(value));

            WriteObject(value, value.GetType());
        }

        /// <inheritdoc />
        public void WriteObject(object value, Type type)
        {
            // Including null checking in case the values come from something like Enumerable.Range()
            if (type is null || type == CommonTypes.ObjectType)
            {
                type = value?.GetType();
            }

            if (value is null && (type is null || type == CommonTypes.ObjectType))
            {
                throw new BinaryWriterException("All binary serialized values must be typed or have a non-null value.");
            }

            var rawTypeId = GetRawTypeId(type);

            var typeId = (byte) rawTypeId;

            if (value == default)
            {
                typeId |= (byte) TypeIdentifier.DefaultValue;
            }

            this.WriteByte(typeId);

            if ((typeId & (byte) TypeIdentifier.DefaultValue) == 0)
            {
                TypeIdWriter[rawTypeId].Invoke(this, value);
            }
        }

        private IReadOnlyCollection<Func<Type, TypeIdentifier?>> GetTypeIdLookups()
        {
            return new[]
            {
                IsTypeRegistered,
                IsTypeEnum,
                IsTypeNullable,
                IsUserDefinedOrCached,      // Above IDictionary / IEnumerable so class types, including those inheriting IDictionary / IEnumerable, can be checked first
                IsDictionary,               // Above IsEnumerable, since IDictionary is IEnumerable
                IsEnumerable
            };
        }

        private TypeIdentifier GetRawTypeId(Type type)
        {
            foreach (var lookup in _typeIdLookups)
            {
                var typeId = lookup.Invoke(type);

                if (typeId.HasValue)
                {
                    return typeId.Value;
                }
            }

            throw new BinaryWriterException($"No binary writer registered for the type '{type.GetFriendlyName()}'.");
        }

        private TypeIdentifier? IsTypeRegistered(Type type)
        {
            return TypeIdRegistry.TryGetValue(type, out var rawTypeId)
                ? rawTypeId
                : (TypeIdentifier?) default;
        }

        private TypeIdentifier? IsTypeEnum(Type type)
        {
            return type.IsEnum
                ? TypeIdentifier.Enum
                : (TypeIdentifier?) default;
        }

        private TypeIdentifier? IsTypeNullable(Type type)
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

        private TypeIdentifier? IsDictionary(Type type)
        {
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                return TypeIdentifier.Dictionary;
            }

            return default;
        }

        private TypeIdentifier? IsEnumerable(Type type)
        {
            // Not checking for strings since it's is a pre-registered type
            if (type.IsArray || type.IsEnumerableType())
            {
                // We could attempt to write the type once if it is an IEnumerable<T> but this gets
                // more complicated with nullable, anonymous and iterator types - opting for convenience
                // and simplicity over stream size.
                return TypeIdentifier.Enumerable;
            }

            return default;
        }

        private TypeIdentifier? IsUserDefinedOrCached(Type type)
        {
            var converter = Writers.SingleOrDefault(converter => converter.Type == type);

            if (converter == null)
            {
                return default;
            }

            var assemblyTypeName = type.AssemblyQualifiedName;

            return _userDefinedTypeCache.ContainsKey(assemblyTypeName)
                ? TypeIdentifier.Cached
                : TypeIdentifier.UserDefined;
        }
    }
}
