using System;
using System.Collections.Generic;

namespace AllOverIt.Serialization
{
    // TODO: TBC

    internal static class TypeMapping
    {
        public enum TypeId : byte
        {
            Descriptor = 1,         // A type descriptor for unknown types
            Bool = 2,
            Byte = 3,
            SByte = 4,
            UShort = 5,
            Short = 6,
            UInt = 7,
            Int = 8,
            ULong = 9,
            Long = 10,
            Float = 11,
            Double = 12,
            Decimal = 13,
            String = 14,
            Char = 15,
            Enum = 16,
            Guid = 17,
            DateTime = 18,
            TimeSpan = 19,
            Array = 20,
            //Struct = ,
            //KeyValuePair = ,
            //IEnumerable = ,
            //IDictionary = ,
            //Object = ,

            // Bit to indicate the type is nullable
            //Nullable = 64,        // don't think this is required

            // Bit to indicate the value is its default (such as for string and nullable) - so don't need to read the value
            DefaultValue = 128
        }

        public static readonly IDictionary<Type, TypeId> TypeIdRegistry = new Dictionary<Type, TypeId>
        {
            { typeof(bool), TypeId.Bool },
            { typeof(byte), TypeId.Byte },
            { typeof(sbyte), TypeId.SByte },
            { typeof(ushort), TypeId.UShort },
            { typeof(short), TypeId.Short },
            { typeof(uint), TypeId.UInt },
            { typeof(int), TypeId.Int },
            { typeof(ulong), TypeId.ULong },
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
}