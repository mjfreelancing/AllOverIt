using AllOverIt.Patterns.Enumeration;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AllOverIt.Reflection
{
    /// <summary>Contains a number of static <see cref="Type"/> for common types.</summary>
    public static class CommonTypes
    {
        /// <summary>Type declaration for <see cref="object"/>; ie., <c>typeof(object)</c>.</summary>
        public static readonly Type ObjectType = typeof(object);

        /// <summary>Type declaration for <see cref="string"/>; ie., <c>typeof(string)</c>.</summary>
        public static readonly Type StringType = typeof(string);

        /// <summary>Type declaration for <see cref="bool"/>; ie., <c>typeof(bool)</c>.</summary>
        public static readonly Type BoolType = typeof(bool);

        /// <summary>Type declaration for <see cref="byte"/>; ie., <c>typeof(byte)</c>.</summary>
        public static readonly Type ByteType = typeof(byte);

        /// <summary>Type declaration for <see cref="sbyte"/>; ie., <c>typeof(sbyte)</c>.</summary>
        public static readonly Type SByteType = typeof(sbyte);

        /// <summary>Type declaration for <see cref="ushort"/>; ie., <c>typeof(ushort)</c>.</summary>
        public static readonly Type UShortType = typeof(ushort);

        /// <summary>Type declaration for <see cref="short"/>; ie., <c>typeof(short)</c>.</summary>
        public static readonly Type ShortType = typeof(short);

        /// <summary>Type declaration for <see cref="uint"/>; ie., <c>typeof(uint)</c>.</summary>
        public static readonly Type UIntType = typeof(uint);

        /// <summary>Type declaration for <see cref="int"/>; ie., <c>typeof(int)</c>.</summary>
        public static readonly Type IntType = typeof(int);

        /// <summary>Type declaration for <see cref="ulong"/>; ie., <c>typeof(ulong)</c>.</summary>
        public static readonly Type ULongType = typeof(ulong);

        /// <summary>Type declaration for <see cref="long"/>; ie., <c>typeof(long)</c>.</summary>
        public static readonly Type LongType = typeof(long);

        /// <summary>Type declaration for <see cref="float"/>; ie., <c>typeof(float)</c>.</summary>
        public static readonly Type FloatType = typeof(float);

        /// <summary>Type declaration for <see cref="double"/>; ie., <c>typeof(double)</c>.</summary>
        public static readonly Type DoubleType = typeof(double);

        /// <summary>Type declaration for <see cref="decimal"/>; ie., <c>typeof(decimal)</c>.</summary>
        public static readonly Type DecimalType = typeof(decimal);

        /// <summary>Type declaration for <see cref="char"/>; ie., <c>typeof(char)</c>.</summary>
        public static readonly Type CharType = typeof(char);

        /// <summary>Type declaration for <see cref="Enum"/>; ie., <c>typeof(Enum)</c>.</summary>
        public static readonly Type EnumType = typeof(Enum);

        /// <summary>Type declaration for <see cref="Guid"/>; ie., <c>typeof(Guid)</c>.</summary>
        public static readonly Type GuidType = typeof(Guid);

        /// <summary>Type declaration for <see cref="DateTime"/>; ie., <c>typeof(DateTime)</c>.</summary>
        public static readonly Type DateTimeType = typeof(DateTime);

        /// <summary>Type declaration for <see cref="TimeSpan"/>; ie., <c>typeof(TimeSpan)</c>.</summary>
        public static readonly Type TimeSpanType = typeof(TimeSpan);

        /// <summary>Type declaration for <see cref="Nullable{}"/>; ie., <c>typeof(Nullable&lt;&gt;)</c>.</summary>
        public static readonly Type NullableGenericType = typeof(Nullable<>);

        /// <summary>Type declaration for <see cref="IEnumerable"/>; ie., <c>typeof(IEnumerable)</c>.</summary>
        public static readonly Type IEnumerableType = typeof(IEnumerable);

        /// <summary>Type declaration for <see cref="IEnumerable{}"/>; ie., <c>typeof(IEnumerable&lt;&gt;)</c>.</summary>
        public static readonly Type IEnumerableGenericType = typeof(IEnumerable<>);

        /// <summary>Type declaration for <see cref="List{}"/>; ie., <c>typeof(List&lt;&gt;)</c>.</summary>
        public static readonly Type ListGenericType = typeof(List<>);

        /// <summary>Type declaration for <see cref="StringComparison"/>; ie., <c>typeof(StringComparison)</c>.</summary>
        public static readonly Type StringComparisonType = typeof(StringComparison);

        /// <summary>Type declaration for <see cref="EnrichedEnum{}"/>; ie., <c>typeof(EnrichedEnum&lt;&gt;)</c>.</summary>
        public static readonly Type EnrichedEnumGenericType = typeof(EnrichedEnum<>);
    }
}
