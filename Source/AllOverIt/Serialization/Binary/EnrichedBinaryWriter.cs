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
            { TypeMapping.TypeId.Dictionary, (writer, value) => writer.WriteDictionary((IDictionary)value) },            
            { TypeMapping.TypeId.Enumerable, (writer, value) => writer.WriteEnumerable((IEnumerable)value) },

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

                    writer.Write(assemblyTypeName);
                    converter.WriteValue(writer, value);
                }
            }
        };

        private readonly IDictionary<string, int> _userDefinedTypeCache = new Dictionary<string, int>();
        private readonly IReadOnlyCollection<Func<Type, TypeMapping.TypeId?>> _typeIdLookups;

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

            WriteObject(value, value.GetType());
        }

        public void WriteObject(object value, Type type)
        {
            // Including null checking in case the values come from something like Enumerable.Range()
            if (type is null || type == ObjectType)
            {
                type = value?.GetType();
            }

            if (value is null && (type is null || type == ObjectType))
            {
                throw new BinaryWriterException("All binary serialized values must be typed or have a non-null value.");
            }

            var rawTypeId = GetRawTypeId(type);

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

        private IReadOnlyCollection<Func<Type, TypeMapping.TypeId?>> GetTypeIdLookups()
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

        private TypeMapping.TypeId GetRawTypeId(Type type)
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

        private TypeMapping.TypeId? IsTypeRegistered(Type type)
        {
            return TypeIdRegistry.TryGetValue(type, out var rawTypeId)
                ? rawTypeId
                : (TypeMapping.TypeId?) default;
        }

        private TypeMapping.TypeId? IsTypeEnum(Type type)
        {
            return type.IsEnum
                ? TypeMapping.TypeId.Enum
                : (TypeMapping.TypeId?) default;
        }

        private TypeMapping.TypeId? IsTypeNullable(Type type)
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

        private TypeMapping.TypeId? IsDictionary(Type type)
        {
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                return TypeMapping.TypeId.Dictionary;
            }

            return default;
        }

        private TypeMapping.TypeId? IsEnumerable(Type type)
        {
            // Not checking for strings since it's is a pre-registered type
            if (type.IsArray || type.IsEnumerableType())
            {
                // We could attempt to write the type once if it is an IEnumerable<T> but this gets
                // more complicated with nullable, anonymous and iterator types - opting for convenience
                // and simplicity over stream size.
                return TypeMapping.TypeId.Enumerable;
            }

            return default;
        }

        private TypeMapping.TypeId? IsUserDefinedOrCached(Type type)
        {
            var converter = Writers.SingleOrDefault(converter => converter.Type == type);

            if (converter == null)
            {
                return default;
            }

            var assemblyTypeName = type.AssemblyQualifiedName;

            return _userDefinedTypeCache.ContainsKey(assemblyTypeName)
                ? TypeMapping.TypeId.Cached
                : TypeMapping.TypeId.UserDefined;
        }
    }
}
