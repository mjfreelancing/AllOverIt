using AllOverIt.Assertion;
using AllOverIt.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AllOverIt.Serialization
{
    // TODO: TBC

    internal static class TypeNapping
    {
        public enum TypeId : byte
        {
            Descriptor = 1,         // A type descriptor for unknown types
            Bool = 2,
            Byte = 3,
            Short = 4,
            Int = 5,
            Long = 6,
            Float = 7,
            Double = 8,
            Decimal = 9,
            String = 10,
            Char = 11,
            Enum = 12,
            Guid = 13,
            DateTime = 14,
            TimeSpan = 15,
            Array = 16,
            //Struct = 17,
            //KeyValuePair = 18,
            //IEnumerable = 19,
            //IDictionary = 20,
            //Object = 21,

            // Bit to indicate the type is nullable
            //Nullable = 64,        // don't need to know

            // Bit to indicate the value is its default (including string and nullable)
            DefaultValue = 128
        }

        public static readonly IDictionary<Type, TypeId> TypeIdRegistry = new Dictionary<Type, TypeId>
        {
            { typeof(bool), TypeId.Bool },
            { typeof(byte), TypeId.Byte },
            { typeof(short), TypeId.Short },
            { typeof(int), TypeId.Int },
            { typeof(long), TypeId.Long },
            { typeof(float), TypeId.Float },
            { typeof(double), TypeId.Double },
            { typeof(decimal), TypeId.Decimal },
            { typeof(string), TypeId.String },
            { typeof(char), TypeId.Char },
            { typeof(Enum), TypeId.Enum },
            { typeof(Guid), TypeId.Guid },
            { typeof(DateTime), TypeId.DateTime },
            { typeof(TimeSpan), TypeId.TimeSpan }
        };
    }



    public static class BinaryWriterExtensions
    {
        /// <inheritdoc cref="BinaryWriter.Write(ulong)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteUInt64(this BinaryWriter writer, ulong value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(uint)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteUInt32(this BinaryWriter writer, uint value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(ushort)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteUInt16(this BinaryWriter writer, ushort value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(string)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteString(this BinaryWriter writer, string value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(float)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteSingle(this BinaryWriter writer, float value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(sbyte)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteSByte(this BinaryWriter writer, sbyte value) => writer.Write(value);

#if NETSTANDARD2_1 || NETCOREAPP3_1_OR_GREATER
        /// <inheritdoc cref="BinaryWriter.Write(ReadOnlySpan{char})"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteChars(this BinaryWriter writer, ReadOnlySpan<char> chars) => writer.Write(chars);

        /// <inheritdoc cref="BinaryWriter.Write(ReadOnlySpan{byte})"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteBytes(this BinaryWriter writer, ReadOnlySpan<byte> buffer) => writer.Write(buffer);
#endif

        /// <inheritdoc cref="BinaryWriter.Write(long)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteInt64(this BinaryWriter writer, long value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(int)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteInt32(this BinaryWriter writer, int value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(double)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteDouble(this BinaryWriter writer, double value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(decimal)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteDecimal(this BinaryWriter writer, decimal value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(char[], int, int)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteChars(this BinaryWriter writer, char[] chars, int index, int count) => writer.Write(chars, index, count);

        /// <inheritdoc cref="BinaryWriter.Write(char[])"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteChars(this BinaryWriter writer, char[] chars) => writer.Write(chars);

        /// <inheritdoc cref="BinaryWriter.Write(byte[], int, int)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteBytes(this BinaryWriter writer, byte[] buffer, int index, int count) => writer.Write(buffer, index, count);

        /// <inheritdoc cref="BinaryWriter.Write(byte[])"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteBytes(this BinaryWriter writer, byte[] buffer) => writer.Write(buffer);

        /// <inheritdoc cref="BinaryWriter.Write(byte)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteByte(this BinaryWriter writer, byte value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(bool)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteBoolean(this BinaryWriter writer, bool value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(short)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteInt16(this BinaryWriter writer, short value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(char)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteChar(this BinaryWriter writer, char value) => writer.Write(value);
    }



    public static class ByteUtils
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct UInt32ToFloat
        {
            [FieldOffset(0)] public uint UInt32;
            [FieldOffset(0)] public float Float;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct UInt64ToDouble
        {
            [FieldOffset(0)] public ulong UInt64;
            [FieldOffset(0)] public double Double;
        }


        [StructLayout(LayoutKind.Explicit)]
        private struct UInt64UInt64ToDecimal
        {
            [FieldOffset(0)] public ulong UInt64;
            [FieldOffset(8)] public ulong UInt642;
            [FieldOffset(0)] public decimal Decimal;
        }

        public static ushort SwapBytes(this ushort value)
        {
            return unchecked((ushort) (((value & 0xFF00U) >> 8) | ((value & 0x00FFU) << 8)));
        }

        public static short SwapBytes(this short value)
        {
            return unchecked((short) SwapBytes(unchecked((ushort) value)));
        }

        public static uint SwapBytes(this uint value)
        {
            //((x & 0x000000ff) << 24) + ((x & 0x0000ff00) << 8) |
            //((x & 0x00ff0000) >> 8) + ((x & 0xff000000) >> 24);

            // Swap adjacent 16-bit blocks
            value = (value >> 16) | (value << 16);

            // Swap adjacent 8-bit blocks
            return ((value & 0xFF00FF00U) >> 8) | ((value & 0x00FF00FFU) << 8);
        }


        public static int SwapBytes(this int value)
        {
            return unchecked((int) SwapBytes(unchecked((uint) value)));
        }

        public static ulong SwapBytes(this ulong value)
        {
            //(0x00000000000000FF) & (value >> 56) | (0x000000000000FF00) & (value >> 40) |
            //(0x0000000000FF0000) & (value >> 24) | (0x00000000FF000000) & (value >> 8) |
            //(0x000000FF00000000) & (value << 8) | (0x0000FF0000000000) & (value << 24) |
            //(0x00FF000000000000) & (value << 40) | (0xFF00000000000000) & (value << 56);

            // Swap adjacent 32-bit blocks
            value = (value >> 32) | (value << 32);

            // Swap adjacent 16-bit blocks
            value = ((value & 0xFFFF0000FFFF0000U) >> 16) | ((value & 0x0000FFFF0000FFFFU) << 16);

            // Swap adjacent 8-bit blocks
            return ((value & 0xFF00FF00FF00FF00U) >> 8) | ((value & 0x00FF00FF00FF00FFU) << 8);
        }

        public static long SwapBytes(this long value)
        {
            return unchecked((long) SwapBytes(unchecked((ulong) value)));
        }

        public static float SwapBytes(this float value)
        {
            var union = new UInt32ToFloat()
            {
                Float = value
            };

            union.UInt32 = SwapBytes(union.UInt32);
            return union.Float;
        }

        public static double SwapBytes(this double value)
        {
            var union = new UInt64ToDouble()
            {
                Double = value
            };

            union.UInt64 = SwapBytes(union.UInt64);
            return union.Double;
        }

        public static decimal SwapBytes(this decimal value)
        {
            var union = new UInt64UInt64ToDecimal()
            {
                Decimal = value
            };

            //var tmp = SwapBytes(map.UInt64);
            //map.UInt64 = SwapBytes(map.UInt642);
            //map.UInt642 = tmp;

            (union.UInt64, union.UInt642) = (SwapBytes(union.UInt642), SwapBytes(union.UInt64));

            return union.Decimal;
        }
    }


    public static class EndianUtils
    {
        /// <summary>Indicates if the executing platform uses a little-endian byte ordering scheme.</summary>
        private static bool IsLittleEndian => BitConverter.IsLittleEndian;

        public static ushort AsBigEndian(this ushort value)
        {
            return IsLittleEndian
                ? value.SwapBytes()
                : value;
        }

        public static ushort AsLittleEndian(this ushort value)
        {
            return IsLittleEndian
                ? value
                : value.SwapBytes();
        }

        public static short AsBigEndian(this short value)
        {
            return IsLittleEndian
                ? value.SwapBytes()
                : value;
        }

        public static short AsLittleEndian(this short value)
        {
            return IsLittleEndian
                ? value
                : value.SwapBytes();
        }

        public static uint AsBigEndian(this uint value)
        {
            return IsLittleEndian
                ? value.SwapBytes()
                : value;
        }

        public static uint AsLittleEndian(this uint value)
        {
            return IsLittleEndian
                ? value
                : value.SwapBytes();
        }

        public static int AsBigEndian(this int value)
        {
            return IsLittleEndian
                ? value.SwapBytes()
                : value;
        }

        public static int AsLittleEndian(this int value)
        {
            return IsLittleEndian
                ? value
                : value.SwapBytes();
        }

        public static ulong AsBigEndian(this ulong value)
        {
            return IsLittleEndian
                ? value.SwapBytes()
                : value;
        }

        public static ulong AsLittleEndian(this ulong value)
        {
            return IsLittleEndian
                ? value
                : value.SwapBytes();
        }

        public static long AsBigEndian(this long value)
        {
            return IsLittleEndian
                ? value.SwapBytes()
                : value;
        }

        public static long AsLittleEndian(this long value)
        {
            return IsLittleEndian
                ? value
                : value.SwapBytes();
        }

        public static float AsBigEndian(this float value)
        {
            return IsLittleEndian
                ? value.SwapBytes()
                : value;
        }

        public static float AsLittleEndian(this float value)
        {
            return IsLittleEndian
                ? value
                : value.SwapBytes();
        }

        public static double AsBigEndian(this double value)
        {
            return IsLittleEndian
                ? value.SwapBytes()
                : value;
        }

        public static double AsLittleEndian(this double value)
        {
            return IsLittleEndian
                ? value
                : value.SwapBytes();
        }

        public static decimal AsBigEndian(this decimal value)
        {
            return IsLittleEndian
                ? value.SwapBytes()
                : value;
        }

        public static decimal AsLittleEndian(this decimal value)
        {
            return IsLittleEndian
                ? value
                : value.SwapBytes();
        }
    }






    public sealed class EnrichedBinaryWriter : BinaryWriter
    {
        private static readonly IDictionary<TypeNapping.TypeId, Action<BinaryWriter, object>> TypeIdWriter = new Dictionary<TypeNapping.TypeId, Action<BinaryWriter, object>>
        {
            { TypeNapping.TypeId.Bool, (writer, value) => writer.WriteBoolean((bool)value) },
            { TypeNapping.TypeId.Byte, (writer, value) => writer.WriteByte((byte)value) },
            { TypeNapping.TypeId.Short, (writer, value) => writer.WriteInt16((short)value) },
            { TypeNapping.TypeId.Int, (writer, value) => writer.WriteInt32((int)value) },
            { TypeNapping.TypeId.Long, (writer, value) => writer.WriteInt64((long)value) },
            { TypeNapping.TypeId.Float, (writer, value) => writer.WriteSingle((float)value) },
            { TypeNapping.TypeId.Double, (writer, value) => writer.WriteDouble((double)value) },
            { TypeNapping.TypeId.Decimal, (writer, value) => writer.WriteDecimal((decimal)value) },
            { TypeNapping.TypeId.String, (writer, value) => writer.WriteString((string)value) },
            { TypeNapping.TypeId.Char, (writer, value) => writer.WriteChar((char)value) },
            { TypeNapping.TypeId.Enum, (writer, value) => writer.WriteInt32((int)value) },                              // TODO: Need to write (and restore) the actual enum type
            { TypeNapping.TypeId.Guid, (writer, value) => writer.WriteBytes(((Guid)value).ToByteArray()) },
            { TypeNapping.TypeId.DateTime, (writer, value) => writer.WriteInt64(((DateTime)value).ToBinary()) },
            { TypeNapping.TypeId.TimeSpan, (writer, value) => writer.WriteInt64(((TimeSpan)value).Ticks) }
        };

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

        public void WriteObject(Type type, object value)
        {
            if (type.IsArray || type.IsEnumerableType() || type.IsGenericEnumerableType())        // handle enumerable, dictionary etc
            {
                throw new NotImplementedException();
            }

            // TODO: error handling and OCP (code below)
            var hasTypeId = TypeNapping.TypeIdRegistry.TryGetValue(type, out var rawTypeId);

            if (!hasTypeId)
            {
                if (type.IsEnum)
                {
                    rawTypeId = TypeNapping.TypeId.Enum;
                }
            }

            if (!hasTypeId)
            {
                if (type.IsGenericNullableType())
                {
                    var underlyingType = Nullable.GetUnderlyingType(type);
                    hasTypeId = TypeNapping.TypeIdRegistry.TryGetValue(underlyingType, out rawTypeId);
                }
            }

            var typeId = (byte) rawTypeId;

            //if (type.IsGenericNullableType())
            //{
            //    typeId |= (byte) TypeNapping.TypeId.Nullable;
            //}

            if (value == default)
            {
                typeId |= (byte) TypeNapping.TypeId.DefaultValue;
            }

            this.WriteByte(typeId);

            if ((typeId & (byte) TypeNapping.TypeId.DefaultValue) == 0)
            {
                TypeIdWriter[rawTypeId].Invoke(this, value);
            }
        }
    }


    public sealed class EnrichedBinaryReader : BinaryReader
    {
        private static readonly IDictionary<TypeNapping.TypeId, Func<BinaryReader, object>> TypeIdReader = new Dictionary<TypeNapping.TypeId, Func<BinaryReader, object>>
        {
            { TypeNapping.TypeId.Bool, reader => reader.ReadBoolean() },
            { TypeNapping.TypeId.Byte, reader => reader.ReadByte() },
            { TypeNapping.TypeId.Short, reader => reader.ReadInt16() },
            { TypeNapping.TypeId.Int, reader => reader.ReadInt32() },
            { TypeNapping.TypeId.Long, reader => reader.ReadInt64() },
            { TypeNapping.TypeId.Float, reader => reader.ReadSingle() },
            { TypeNapping.TypeId.Double, reader => reader.ReadDouble() },
            { TypeNapping.TypeId.Decimal, reader => reader.ReadDecimal() },
            { TypeNapping.TypeId.String, reader => reader.ReadString() },
            { TypeNapping.TypeId.Char, reader => reader.ReadChar() },
            { TypeNapping.TypeId.Enum, reader => reader.ReadInt32() },
            { TypeNapping.TypeId.Guid, reader => new Guid(reader.ReadBytes(16)) },
            { TypeNapping.TypeId.DateTime, reader => DateTime.FromBinary(reader.ReadInt64()) },
            { TypeNapping.TypeId.TimeSpan, reader => new TimeSpan(reader.ReadInt64()) }
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

            var rawTypeId = (TypeNapping.TypeId) (typeId & 0x3F);       // Exclude all bit flags

            var type = TypeNapping.TypeIdRegistry.Where(kvp => kvp.Value == rawTypeId).Single().Key;

            object rawValue = default;

            // Applicable to strings and nullable types
            var haveValue = (typeId & (byte) TypeNapping.TypeId.DefaultValue) == 0;

            if (haveValue)
            {
                // Read the value
                rawValue = TypeIdReader[rawTypeId].Invoke(this);
            }

            // TODO: See if we need to change the type if it's an Enum
            return rawValue;


            //// If we have a value then return it (don't need to convert to a nullable type and it will already be a string if applicable)
            //if (rawValue != default)
            //{
            //    // It's not a nullable type, or it is but we have a value
            //    return rawValue;
            //}

            //// Is it a nullable type 
            //var isNullable = (typeId & (byte) TypeNapping.TypeId.Nullable) != 0;

            //if (isNullable)
            //{
            //    var nullableType = typeof(Nullable<>).MakeGenericType(type);
            //    return Activator.CreateInstance(nullableType);
            //}

            //return default;
        }
    }




}




//internal sealed class TokenTypeValue
//{
//    public Type Type { get; init; }
//    public object Value { get; init; }
//}




//internal sealed class TokenValueConverterJST : System.Text.Json.Serialization.JsonConverter<TokenValue>
//{
//    public override TokenValue Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
//    {
//        do
//        {
//            if (reader.TokenType == JsonTokenType.EndArray)
//            {
//                return new TokenValue();
//            }      

//            // expect type code
//            if (reader.TokenType != JsonTokenType.Number)
//            {
//                throw new Exception();
//            }

//            var typeCode = (TypeCode)reader.GetInt32();

//            reader.Read();

//            switch (typeCode)
//            {
//                case TypeCode.Empty:        // null
//                    //reader.Read();
//                    break;

//                case TypeCode.Boolean:
//                    //reader.Read();
//                    break;

//                case TypeCode.Int16:
//                    //reader.Read();
//                    break;

//                case TypeCode.Int32:
//                    //reader.Read();
//                    break;

//                case TypeCode.Int64:
//                    //reader.Read();
//                    break;

//                case TypeCode.Single:
//                    //reader.Read();
//                    break;

//                case TypeCode.Double:
//                    //reader.Read();
//                    break;

//                case TypeCode.Decimal:
//                    //reader.Read();
//                    break;

//                case TypeCode.DateTime:
//                    //reader.Read();
//                    break;

//                case TypeCode.String:
//                    // read the actual type
//                    var strType = reader.GetString();
//                    var type = Type.GetType(strType);       // or Type.GetType("Namespace.MyClass, MyAssembly");

//                    reader.Read();
//                    var strValue = reader.GetString();
//                    var objValue = (object)Convert.ChangeType(strValue, type);
//                    break;


//            }

//        } while (reader.Read());

//        throw new InvalidOperationException("Expected at least one continuation token value");
//    }

//    public override void Write(System.Text.Json.Utf8JsonWriter writer, TokenValue value, System.Text.Json.JsonSerializerOptions options)
//    {
//        writer.WriteStartObject();
//        //writer.WriteStartArray();

//        var valueType = value.Value.GetType();
//        var typeCode = Type.GetTypeCode(valueType);

//        writer.WritePropertyName("code");
//        writer.WriteNumberValue((int)typeCode);






//        writer.WritePropertyName("name");

//        switch (value.Value)
//        {
//            case string _:
//            case DateTime _:
//            case long _:
//            case int _:
//            case float _:
//            case double _:
//            case decimal _:
//            case bool _:
//                writer.WriteNullValue();
//                break;

//            case Guid _:
//            case Enum _:
//                writer.WriteStringValue(valueType.FullName);
//                break;

//            default:
//                throw new InvalidOperationException($"Unhandled object type '{value.Value.GetType().GetFriendlyName()}' while writing a nested dictionary.");
//        }



//        writer.WritePropertyName("value");

//        switch (value.Value)
//        {
//            case null:
//                writer.WriteNullValue();
//                break;

//            case string stringValue:
//                writer.WriteStringValue(stringValue);
//                break;

//            case DateTime dateTime:
//                writer.WriteStringValue(dateTime);
//                break;

//            case Guid guid:
//                writer.WriteStringValue(guid);
//                break;

//            case long longValue:
//                writer.WriteNumberValue(longValue);
//                break;

//            case int intValue:
//                writer.WriteNumberValue(intValue);
//                break;

//            case float floatValue:
//                writer.WriteNumberValue(floatValue);
//                break;

//            case double doubleValue:
//                writer.WriteNumberValue(doubleValue);
//                break;

//            case decimal decimalValue:
//                writer.WriteNumberValue(decimalValue);
//                break;

//            case bool boolValue:
//                writer.WriteBooleanValue(boolValue);
//                break;

//            case Enum enumValue:
//                var underlyingType = Enum.GetUnderlyingType(enumValue.GetType());
//                var underlyingValue = Convert.ChangeType(enumValue, underlyingType);
//                writer.WriteNumberValue((int)underlyingValue);
//                break;

//            default:
//                throw new InvalidOperationException($"Unhandled object type '{value.Value.GetType().GetFriendlyName()}' while writing a nested dictionary.");
//        }

//        //writer.WriteEndArray();
//        writer.WriteEndObject();
//    }
//}



//public sealed class TokenValueConverterNSJ : Newtonsoft.Json.JsonConverter
//{
//    public override bool CanConvert(Type objectType)
//    {
//        return typeof(TokenValue) == objectType;
//    }

//    /// <inheritdoc />
//    public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
//    {
//        if (reader.Value == null)
//        {
//            throw new FormatException("Expecting to read a DateTime value but found null");
//        }

//        var dateTime = (DateTime) reader.Value;

//        return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
//    }

//    /// <inheritdoc />
//    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
//    {
//        var utcDateTime = DateTime.SpecifyKind((DateTime) value, DateTimeKind.Utc);
//        writer.WriteValue(utcDateTime);
//    }
//}








//internal static class ContinuationTokenExtensions
//{
//    internal static string Encode(this ContinuationToken token)
//    {
//        var str = System.Text.Json.JsonSerializer.Serialize(token);

//        return str;
//    }
//}
