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
            { TypeMapping.TypeId.Enum, reader => reader.ReadInt32() },
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
