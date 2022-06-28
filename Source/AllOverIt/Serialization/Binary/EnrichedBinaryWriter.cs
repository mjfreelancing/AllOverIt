using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Serialization.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AllOverIt.Serialization
{


    public interface IEnrichedBinaryTypeWriter
    {
        Type Type { get; }
        void WriteValue(EnrichedBinaryWriter writer, object value);
    }

    public abstract class EnrichedBinaryTypeWriter<TType> : IEnrichedBinaryTypeWriter
    {
        public Type Type => typeof(TType);

        public abstract void WriteValue(EnrichedBinaryWriter writer, object value);
    }

    public interface IEnrichedBinaryWriter
    {
        IList<IEnrichedBinaryTypeWriter> Writers { get; }
        void WriteObject(object value);
    }



    public sealed class EnrichedBinaryWriter : BinaryWriter, IEnrichedBinaryWriter
    {
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

        private static readonly IDictionary<TypeMapping.TypeId, Action<EnrichedBinaryWriter, object>> TypeIdWriter = new Dictionary<TypeMapping.TypeId, Action<EnrichedBinaryWriter, object>>
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
            { TypeMapping.TypeId.String, (writer, value) => writer.WriteString((string)value) },
            { TypeMapping.TypeId.Char, (writer, value) => writer.WriteChar((char)value) },
            
            { TypeMapping.TypeId.Enum, (writer, value) =>
                {
                    // Need the string representation of the value in order to convert it back to the original Enum type.
                    // Convert.ChangeType() cannot convert an integral type to an Enum type.
                    writer.WriteString(value.GetType().AssemblyQualifiedName);        // Need to store a registry of types rather than write them all the time
                    writer.WriteString($"{value}");   
                }
            },

            { TypeMapping.TypeId.Guid, (writer, value) => writer.WriteBytes(((Guid)value).ToByteArray()) },
            { TypeMapping.TypeId.DateTime, (writer, value) => writer.WriteInt64(((DateTime)value).ToBinary()) },
            { TypeMapping.TypeId.TimeSpan, (writer, value) => writer.WriteInt64(((TimeSpan)value).Ticks) },
            { TypeMapping.TypeId.UserDefined, (writer, value) =>
                {
                    var valueType = value.GetType();
                    var converter = writer.Writers.SingleOrDefault(converter => converter.Type == valueType);

                    writer.WriteString(valueType.AssemblyQualifiedName);
                    converter.WriteValue(writer, value);
                }
            }
        };

        public IList<IEnrichedBinaryTypeWriter> Writers { get; } = new List<IEnrichedBinaryTypeWriter>();

        /// <inheritdoc cref="BinaryWriter(Stream)"/>
        public EnrichedBinaryWriter(Stream output)
            : base(output)
        {
        }

        /// <inheritdoc cref="BinaryWriter(Stream, Encoding)"/>
        public EnrichedBinaryWriter(Stream output, Encoding encoding)
            : base(output, encoding)
        {
        }

        /// <inheritdoc cref="BinaryWriter(Stream, Encoding, bool)"/>
        public EnrichedBinaryWriter(Stream output, Encoding encoding, bool leaveOpen)
            : base(output, encoding, leaveOpen)
        {
        }

        public void WriteObject(object value)
        {
            _ = value.WhenNotNull(nameof(value));

            WriteObject(value.GetType(), value);
        }

        private void WriteObject(Type type, object value)
        {
            if (type.IsArray || type.IsEnumerableType() || type.IsGenericEnumerableType())        // handle enumerable, dictionary etc
            {
                throw new NotImplementedException();
            }

            // TODO: error handling and OCP (code below)
            var hasTypeId = TypeIdRegistry.TryGetValue(type, out var rawTypeId);

            if (!hasTypeId)
            {
                if (type.IsEnum)
                {
                    rawTypeId = TypeMapping.TypeId.Enum;
                    hasTypeId = true;
                }
            }

            if (!hasTypeId)
            {
                if (type.IsNullableType())
                {
                    var underlyingType = Nullable.GetUnderlyingType(type);
                    hasTypeId = TypeIdRegistry.TryGetValue(underlyingType, out rawTypeId);
                }
            }

            if (!hasTypeId)
            {
                rawTypeId = TypeMapping.TypeId.UserDefined;
                hasTypeId = true;
            }

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
    }
}
