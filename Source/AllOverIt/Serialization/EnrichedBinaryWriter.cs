using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Serialization.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AllOverIt.Serialization
{
    public sealed class EnrichedBinaryWriter : BinaryWriter
    {
        private static readonly IDictionary<TypeMapping.TypeId, Action<BinaryWriter, object>> TypeIdWriter = new Dictionary<TypeMapping.TypeId, Action<BinaryWriter, object>>
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
            { TypeMapping.TypeId.Enum, (writer, value) => writer.WriteInt32((int)value) },                              // TODO: Need to write (and restore) the actual enum type
            { TypeMapping.TypeId.Guid, (writer, value) => writer.WriteBytes(((Guid)value).ToByteArray()) },
            { TypeMapping.TypeId.DateTime, (writer, value) => writer.WriteInt64(((DateTime)value).ToBinary()) },
            { TypeMapping.TypeId.TimeSpan, (writer, value) => writer.WriteInt64(((TimeSpan)value).Ticks) }
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
            var hasTypeId = TypeMapping.TypeIdRegistry.TryGetValue(type, out var rawTypeId);

            if (!hasTypeId)
            {
                if (type.IsEnum)
                {
                    rawTypeId = TypeMapping.TypeId.Enum;
                }
            }

            if (!hasTypeId)
            {
                if (type.IsNullableType())
                {
                    var underlyingType = Nullable.GetUnderlyingType(type);
                    hasTypeId = TypeMapping.TypeIdRegistry.TryGetValue(underlyingType, out rawTypeId);
                }
            }

            var typeId = (byte) rawTypeId;

            //if (type.IsGenericNullableType())
            //{
            //    typeId |= (byte) TypeNapping.TypeId.Nullable;
            //}

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
