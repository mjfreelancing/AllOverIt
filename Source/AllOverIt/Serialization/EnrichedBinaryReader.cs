using AllOverIt.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AllOverIt.Serialization
{
    public sealed class EnrichedBinaryReader : BinaryReader
    {
        private static readonly IDictionary<TypeMapping.TypeId, Func<BinaryReader, object>> TypeIdReader = new Dictionary<TypeMapping.TypeId, Func<BinaryReader, object>>
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
            { TypeMapping.TypeId.String, reader => reader.ReadString() },
            { TypeMapping.TypeId.Char, reader => reader.ReadChar() },

            { TypeMapping.TypeId.Enum, reader =>
                {
                    var valueTypeName = reader.ReadString();
                    var valueType = Type.GetType(valueTypeName);                    // TODO: Check for null

                    var value = reader.ReadString();
                    return Enum.Parse(valueType, value);
                }
            },

            { TypeMapping.TypeId.Guid, reader => new Guid(reader.ReadBytes(16)) },
            { TypeMapping.TypeId.DateTime, reader => DateTime.FromBinary(reader.ReadInt64()) },
            { TypeMapping.TypeId.TimeSpan, reader => new TimeSpan(reader.ReadInt64()) }
        };

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

            var rawTypeId = (TypeMapping.TypeId) (typeId & 0x3F);       // Exclude all bit flags

            var type = TypeMapping.TypeIdRegistry.Where(kvp => kvp.Value == rawTypeId).Single().Key;

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
    }
}
