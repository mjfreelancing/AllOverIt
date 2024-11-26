using AllOverIt.Assertion;
using AllOverIt.Serialization.Binary.Exceptions;
using AllOverIt.Serialization.Binary.Readers.Extensions;
using System.Text;

namespace AllOverIt.Serialization.Binary.Readers
{
    /// <inheritdoc cref="IEnrichedBinaryReader"/>
    public sealed class EnrichedBinaryReader : BinaryReader, IEnrichedBinaryReader
    {
        private static readonly Dictionary<TypeIdentifier, Func<EnrichedBinaryReader, object?>> TypeIdReader = new()
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
            { TypeIdentifier.Enumerable, reader => reader.ReadEnumerable() },       // Returns the value as a List
            { TypeIdentifier.Dictionary, reader => reader.ReadDictionary() },
            { TypeIdentifier.Array, reader => reader.ReadArray() },                 // Returns the value as a []
            {
                TypeIdentifier.Cached, reader =>
                {
                    var cacheIndex = reader.ReadInt32();
                    var assemblyTypeName = reader._userDefinedTypeCache[cacheIndex];

                    var valueType = Type.GetType(assemblyTypeName);
                    var converter = reader.Readers.Single(converter => converter.Type == valueType);

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

                    // If there's no registered reader then add a dynamic reader
                    if (!reader.Readers.Any(converter => converter.Type == valueType))
                    {
                        reader.Readers.Add(new DynamicBinaryValueReader(valueType));
                    }

                    var converter = reader.Readers.Single(converter => converter.Type == valueType);

                    return converter.ReadValue(reader);
                }
            }
        };

        private readonly Dictionary<int, string> _userDefinedTypeCache = [];

        /// <inheritdoc />
        /// <remarks>If a property type doesn't have a registered reader the <see cref="EnrichedBinaryReader"/> will use a
        /// <see cref="DynamicBinaryValueReader"/>.</remarks>
        public ICollection<IEnrichedBinaryValueReader> Readers { get; } = [];

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
        public object? ReadObject()
        {
            var typeId = ReadByte();

            var rawTypeId = (TypeIdentifier) (typeId & ~0x80);       // Exclude the default bit flag

            object? rawValue = null;

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
