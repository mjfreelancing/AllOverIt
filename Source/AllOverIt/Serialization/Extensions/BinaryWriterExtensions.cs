using System;
using System.IO;

namespace AllOverIt.Serialization.Extensions
{
    // TODO: To be moved as these are system extensions
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
