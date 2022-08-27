using AllOverIt.Assertion;
using AllOverIt.Serialization.Binary.Exceptions;
using AllOverIt.Serialization.Binary.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AllOverIt.Serialization.Binary
{
    /// <inheritdoc cref="IEnrichedBinaryReader"/>
    public sealed class EnrichedBinaryReader : BinaryReader, IEnrichedBinaryReader
    {
        private static readonly IDictionary<TypeIdentifier, Func<EnrichedBinaryReader, object>> TypeIdReader = new Dictionary<TypeIdentifier, Func<EnrichedBinaryReader, object>>
        {
            { TypeIdentifier.Bool, reader => reader.ReadBoolean() },
            { TypeIdentifier.Byte, reader => reader.ReadByte() },
            { TypeIdentifier.SByte, reader => reader.ReadSByte() },
            { TypeIdentifier.UShort, reader => reader.ReadUInt16() },
            { TypeIdentifier.Short, reader => reader.ReadInt16() },
            { TypeIdentifier.UInt, reader => reader.ReadUInt32() },
            { TypeIdentifier.Int, reader => reader.ReadInt32() },
            { TypeIdentifier.ULong, reader => reader.ReadUInt64() },
            { TypeIdentifier.Long, reader => reader.ReadInt64() },
            { TypeIdentifier.Float, reader => reader.ReadSingle() },
            { TypeIdentifier.Double, reader => reader.ReadDouble() },
            { TypeIdentifier.Decimal, reader => reader.ReadDecimal() },
            { TypeIdentifier.String, reader => reader.ReadSafeString() },
            { TypeIdentifier.Char, reader => reader.ReadChar() },
            { TypeIdentifier.Enum, reader => reader.ReadEnum() },
            { TypeIdentifier.Guid, reader => reader.ReadGuid() },
            { TypeIdentifier.DateTime, reader => reader.ReadDateTime() },
            { TypeIdentifier.TimeSpan, reader => reader.ReadTimeSpan() },
            { TypeIdentifier.Dictionary, reader => reader.ReadDictionary() },
            { TypeIdentifier.Enumerable, reader => reader.ReadEnumerable() },
            {
                TypeIdentifier.Cached, reader =>
                {
                    var cacheIndex = reader.ReadInt32();
                    var assemblyTypeName = reader._userDefinedTypeCache[cacheIndex];

                    var valueType = Type.GetType(assemblyTypeName);
                    var converter = reader.Readers.SingleOrDefault(converter => converter.Type == valueType);

                    return converter.ReadValue(reader);
                }
            },
            {
                TypeIdentifier.UserDefined, reader =>
                {
                    var assemblyTypeName = reader.ReadString();
                    var valueType = Type.GetType(assemblyTypeName);

                    Throw<BinaryReaderException>.WhenNull(valueType, $"Unknown type '{assemblyTypeName}'.");

                    // cache for later, to read the value as a cached user defined type
                    var cacheIndex = reader._userDefinedTypeCache.Keys.Count + 1;
                    reader._userDefinedTypeCache.Add(cacheIndex, assemblyTypeName);

                    var converter = reader.Readers.SingleOrDefault(converter => converter.Type == valueType);

                    return converter.ReadValue(reader);
                }
            }
        };

        private readonly IDictionary<int, string> _userDefinedTypeCache = new Dictionary<int, string>();

        /// <inheritdoc />
        public IList<IEnrichedBinaryValueReader> Readers { get; } = new List<IEnrichedBinaryValueReader>();

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

        /// <inheritdoc />
        public object ReadObject()
        {
            var typeId = ReadByte();

            var rawTypeId = (TypeIdentifier) (typeId & ~0x80);       // Exclude the default bit flag

            object rawValue = default;

            // Applicable to strings and nullable types
            var haveValue = (typeId & (byte) TypeIdentifier.DefaultValue) == 0;

            if (haveValue)
            {
                // Read the value
                rawValue = TypeIdReader[rawTypeId].Invoke(this);
            }

            return rawValue;
        }
    }
}
