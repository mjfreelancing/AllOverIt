namespace AllOverIt.Serialization.Binary
{
    // TODO: TBC

    internal static class TypeMapping
    {
        public enum TypeId : byte
        {
            Bool = 1,
            Byte = 2,
            SByte = 3,
            UShort = 4,
            Short = 5,
            UInt = 6,
            Int = 7,
            ULong = 8,
            Long = 9,
            Float = 10,
            Double = 11,
            Decimal = 12,
            String = 13,
            Char = 14,
            Enum = 15,
            Guid = 16,
            DateTime = 17,
            TimeSpan = 18,
            Enumerable = 19,
            Dictionary = 20,
            //Array = ,
            //Struct = ,
            //KeyValuePair = ,
            //Object = ,

            Cached = 126,
            UserDefined = 127,

            // Bit to indicate the value is its default (such as for string and nullable) - so don't need to read the value
            DefaultValue = 128
        }


    }
}