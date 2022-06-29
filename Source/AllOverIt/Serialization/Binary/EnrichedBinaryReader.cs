using AllOverIt.Serialization.Binary.Exceptions;
using AllOverIt.Serialization.Binary.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AllOverIt.Serialization.Binary
{
    public interface IEnrichedBinaryTypeReader
    {
        Type Type { get; }
        object ReadValue(EnrichedBinaryReader reader);
        TValue ReadValue<TValue>(EnrichedBinaryReader reader);
    }

    public abstract class EnrichedBinaryTypeReader<TType> : IEnrichedBinaryTypeReader
    {
        public Type Type => typeof(TType);

        public abstract object ReadValue(EnrichedBinaryReader reader);

        public TValue ReadValue<TValue>(EnrichedBinaryReader reader)
        {
            return (TValue)ReadValue(reader);
        }
    }


    public interface IEnrichedBinaryReader
    {
        IList<IEnrichedBinaryTypeReader> Readers { get; }
        object ReadObject();
        TValue ReadObject<TValue>();
        TValue? ReadNullable<TValue>() where TValue : struct;
    }

    public sealed class EnrichedBinaryReader : BinaryReader, IEnrichedBinaryReader
    {
        private static readonly IDictionary<TypeMapping.TypeId, Func<EnrichedBinaryReader, object>> TypeIdReader = new Dictionary<TypeMapping.TypeId, Func<EnrichedBinaryReader, object>>
        {
            { TypeMapping.TypeId.Bool, reader => reader.ReadBoolean() },
            { TypeMapping.TypeId.Byte, reader => reader.ReadByte() },
            { TypeMapping.TypeId.SByte, reader => reader.ReadSByte() },
            { TypeMapping.TypeId.UShort, reader => reader.ReadUInt16() },
            { TypeMapping.TypeId.Short, reader => reader.ReadInt16() },
            { TypeMapping.TypeId.UInt, reader => reader.ReadUInt32() },
            { TypeMapping.TypeId.Int, reader => reader.ReadInt32() },
            { TypeMapping.TypeId.ULong, reader => reader.ReadUInt64() },
            { TypeMapping.TypeId.Long, reader => reader.ReadInt64() },
            { TypeMapping.TypeId.Float, reader => reader.ReadSingle() },
            { TypeMapping.TypeId.Double, reader => reader.ReadDouble() },
            { TypeMapping.TypeId.Decimal, reader => reader.ReadDecimal() },
            { TypeMapping.TypeId.String, reader => reader.ReadSafeString() },
            { TypeMapping.TypeId.Char, reader => reader.ReadChar() },
            { TypeMapping.TypeId.Enum, reader => reader.ReadEnum() },
            { TypeMapping.TypeId.Guid, reader => reader.ReadGuid() },
            { TypeMapping.TypeId.DateTime, reader => DateTime.FromBinary(reader.ReadInt64()) },
            { TypeMapping.TypeId.TimeSpan, reader => new TimeSpan(reader.ReadInt64()) },
            {
                TypeMapping.TypeId.Cached, reader =>
                {
                    var cacheIndex = reader.ReadInt32();
                    var assemblyTypeName = reader._userDefinedTypeCache[cacheIndex];

                    var valueType = Type.GetType(assemblyTypeName);
                    var converter = reader.Readers.SingleOrDefault(converter => converter.Type == valueType);

                    return converter.ReadValue(reader);
                }
            },
            {
                TypeMapping.TypeId.UserDefined, reader =>
                {
                    var assemblyTypeName = reader.ReadString();
                    var valueType = Type.GetType(assemblyTypeName);                    // TODO: Check for null

                    if (valueType == null)
                    {
                        throw new BinaryReaderException($"Unknown type '{assemblyTypeName}'.");
                    }

                    // cache for later, to read the value as a cached user defined type
                    var cacheIndex = reader._userDefinedTypeCache.Keys.Count + 1;
                    reader._userDefinedTypeCache.Add(cacheIndex, assemblyTypeName);

                    var converter = reader.Readers.SingleOrDefault(converter => converter.Type == valueType);

                    return converter.ReadValue(reader);
                }
            }
        };

        private readonly IDictionary<int, string> _userDefinedTypeCache = new Dictionary<int, string>();

        public IList<IEnrichedBinaryTypeReader> Readers { get; } = new List<IEnrichedBinaryTypeReader>();

        /// <inheritdoc cref="BinaryReader(Stream)"/>
        public EnrichedBinaryReader(Stream output)
            : base(output)
        {
        }

        /// <inheritdoc cref="BinaryReader(Stream, Encoding)"/>
        public EnrichedBinaryReader(Stream output, Encoding encoding)
            : base(output, encoding)
        {
        }

        /// <inheritdoc cref="BinaryReader(Stream, Encoding, bool)"/>
        public EnrichedBinaryReader(Stream output, Encoding encoding, bool leaveOpen)
            : base(output, encoding, leaveOpen)
        {
        }

        public object ReadObject()
        {
            var typeId = ReadByte();

            var rawTypeId = (TypeMapping.TypeId) (typeId & ~0x80);       // Exclude the default bit flag

            object rawValue = default;

            // Applicable to strings and nullable types
            var haveValue = (typeId & (byte) TypeMapping.TypeId.DefaultValue) == 0;

            if (haveValue)
            {
                // Read the value
                rawValue = TypeIdReader[rawTypeId].Invoke(this);
            }

            return rawValue;
        }

        public TValue ReadObject<TValue>()
        {
            return (TValue) ReadObject();
        }

        public TValue? ReadNullable<TValue>() where TValue : struct
        {
            return (TValue?) ReadObject();
        }
    }
}
