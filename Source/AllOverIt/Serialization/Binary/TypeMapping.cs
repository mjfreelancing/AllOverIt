namespace AllOverIt.Serialization.Binary
{
    /// <summary>An identifier that's used to determine how <see cref="IEnrichedBinaryReader"/> reads data on a stream.</summary>
    internal enum TypeIdentifier : byte
    {
        /// <summary>Indicates the next value in the stream is a boolean.</summary>
        Bool = 1,

        /// <summary>Indicates the next value in the stream is an unsigned byte.</summary>
        Byte = 2,

        /// <summary>Indicates the next value in the stream is signed byte.</summary>
        SByte = 3,

        /// <summary>Indicates the next value in the stream is an unsigned short.</summary>
        UShort = 4,

        /// <summary>Indicates the next value in the stream is a signed short.</summary>
        Short = 5,

        /// <summary>Indicates the next value in the stream is an unsigned integer.</summary>
        UInt = 6,

        /// <summary>Indicates the next value in the stream is a signed integer.</summary>
        Int = 7,

        /// <summary>Indicates the next value in the stream is an unsigned long.</summary>
        ULong = 8,

        /// <summary>Indicates the next value in the stream is a signed long.</summary>
        Long = 9,

        /// <summary>Indicates the next value in the stream is a float.</summary>
        Float = 10,

        /// <summary>Indicates the next value in the stream is a double.</summary>
        Double = 11,

        /// <summary>Indicates the next value in the stream is a decimal.</summary>
        Decimal = 12,

        /// <summary>Indicates the next value in the stream is a string.</summary>
        String = 13,

        /// <summary>Indicates the next value in the stream is char.</summary>
        Char = 14,

        /// <summary>Indicates the next value in the stream is an enumeration.</summary>
        Enum = 15,

        /// <summary>Indicates the next value in the stream is a Guid.</summary>
        Guid = 16,

        /// <summary>Indicates the next value in the stream is a DateTime.</summary>
        DateTime = 17,

        /// <summary>Indicates the next value in the stream is a TimeSpan.</summary>
        TimeSpan = 18,

        /// <summary>Indicates the next value in the stream is an IEnumerable.</summary>
        Enumerable = 19,

        /// <summary>Indicates the next value in the stream is an IDictionary.</summary>
        Dictionary = 20,

        /// <summary>Indicates the next value in the stream is a user defined type. The assembly qualified name will be written to the stream.</summary>
        UserDefined = 126,

        /// <summary>Indicates the next value in the stream is a user-defined type that has been previously read, hence an internal unique identifier is
        /// written in place of the assembly qualified name.</summary>
        Cached = 127,

        /// <summary>A bit flag to indicate the value is its default. Applied to types such as string and nullable so null values can be supported.</summary>
        DefaultValue = 128
    }
}