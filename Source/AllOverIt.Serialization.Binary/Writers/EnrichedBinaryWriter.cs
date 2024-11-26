using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
using AllOverIt.Serialization.Binary.Exceptions;
using AllOverIt.Serialization.Binary.Writers.Extensions;
using System.Collections;
using System.Text;

namespace AllOverIt.Serialization.Binary.Writers
{
    /// <inheritdoc cref="IEnrichedBinaryWriter"/>
    public sealed class EnrichedBinaryWriter : BinaryWriter, IEnrichedBinaryWriter
    {
        private static readonly Dictionary<Type, TypeIdentifier> TypeIdRegistry = new()
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

        private static readonly Dictionary<TypeIdentifier, Action<EnrichedBinaryWriter, object>> TypeIdWriter = new()
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
            { TypeIdentifier.DateTime, (writer, value) => writer.WriteDateTime((DateTime)value) },
            { TypeIdentifier.TimeSpan, (writer, value) => writer.WriteTimeSpan((TimeSpan)value) },
            { TypeIdentifier.Enumerable, (writer, value) => writer.WriteEnumerable((IEnumerable)value) },
            { TypeIdentifier.Dictionary, (writer, value) => writer.WriteDictionary((IDictionary)value) },
            { TypeIdentifier.Array, (writer, value) => writer.WriteEnumerable((IEnumerable)value) },
            {
                TypeIdentifier.Cached, (writer, value) =>
                {
                    var valueType = value.GetType();
                    var assemblyTypeName = valueType.AssemblyQualifiedName!;

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
                    var assemblyTypeName = valueType.AssemblyQualifiedName!;

                    // cache for later, to write the value as a cached user defined type
                    var cacheIndex = writer._userDefinedTypeCache.Keys.Count + 1;
                    writer._userDefinedTypeCache.Add(assemblyTypeName, cacheIndex);

                    var converter = writer.Writers.Single(converter => converter.Type == valueType);

                    writer.Write(assemblyTypeName);
                    converter.WriteValue(writer, value);
                }
            }
        };

        private readonly Dictionary<string, int> _userDefinedTypeCache = [];
        private readonly Func<Type, TypeIdentifier?>[] _typeIdLookups;

        /// <inheritdoc />
        /// <remarks>If a property type doesn't have a registered writer the <see cref="EnrichedBinaryWriter"/> will use a
        /// <see cref="DynamicBinaryValueWriter"/>.</remarks>
        public ICollection<IEnrichedBinaryValueWriter> Writers { get; } = [];

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
            _ = value.WhenNotNull();

            WriteObject(value, value.GetType());
        }

        /// <inheritdoc />
        public void WriteObject(object? value, Type type)
        {
            // Including null checking in case the values come from something like Enumerable.Range()
            if ((type is null || type == CommonTypes.ObjectType) && value is not null)
            {
                type = value.GetType();
            }

            if (value is null && (type is null || type == CommonTypes.ObjectType))
            {
                throw new BinaryWriterException("All binary serialized values must be typed or have a non-null value.");
            }

            var rawTypeId = GetRawTypeId(type!);

            var typeId = (byte) rawTypeId;

            if (value == default)
            {
                typeId |= (byte) TypeIdentifier.DefaultValue;
            }

            this.WriteByte(typeId);

            if ((typeId & (byte) TypeIdentifier.DefaultValue) == 0)
            {
                TypeIdWriter[rawTypeId].Invoke(this, value!);
            }
        }

        private Func<Type, TypeIdentifier?>[] GetTypeIdLookups()
        {
            return [
                IsTypeRegistered,
                IsTypeEnum,
                IsTypeNullable,
                IsUserDefinedOrCached,      // Above IDictionary / IEnumerable so class types, including those inheriting IDictionary / IEnumerable, can be checked first
                IsDictionary,               // Above IsEnumerable, since IDictionary is IEnumerable
                IsEnumerable,
                AddDynamicWriter            // Must be last in the list
            ];
        }

        private TypeIdentifier GetRawTypeId(Type type)
        {
            var identifierQuery = from lookup in _typeIdLookups
                                  let typeId = lookup.Invoke(type)
                                  where typeId.HasValue
                                  select typeId.Value;

            // Will always find a registered and/or dynamic writer, so we must return the first match.
            return identifierQuery.First();
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
                var underlyingType = Nullable.GetUnderlyingType(type)!;

                if (TypeIdRegistry.TryGetValue(underlyingType, out var rawTypeId))
                {
                    return rawTypeId;
                }
            }

            return null;
        }

        private TypeIdentifier? IsUserDefinedOrCached(Type type)
        {
            var converter = Writers.SingleOrDefault(converter => converter.Type == type);

            if (converter is null)
            {
                return null;
            }

            var assemblyTypeName = type.AssemblyQualifiedName!;

            return _userDefinedTypeCache.ContainsKey(assemblyTypeName)
                ? TypeIdentifier.Cached
                : TypeIdentifier.UserDefined;
        }

        private TypeIdentifier? IsDictionary(Type type)
        {
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                return TypeIdentifier.Dictionary;
            }

            return null;
        }

        private TypeIdentifier? IsEnumerable(Type type)
        {
            // Note: not checking for strings since it is a pre-registered type

            if (type.IsArray)
            {
                // When read back, the values are returned as an array
                return TypeIdentifier.Array;
            }

            if (type.IsArray || type.IsEnumerableType())
            {
                // When read back, the values are returned as a list
                return TypeIdentifier.Enumerable;
            }

            return null;
        }

        private TypeIdentifier? AddDynamicWriter(Type type)
        {
            // Add a writer that will dynamically process all properties
            var writer = new DynamicBinaryValueWriter(type);
            Writers.Add(writer);

            return TypeIdentifier.UserDefined;
        }
    }
}
