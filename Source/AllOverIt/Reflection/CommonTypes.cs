﻿using AllOverIt.Patterns.Enumeration;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AllOverIt.Reflection
{
    /// <summary>Contains a number of static <see cref="Type"/> for common types.</summary>
    public static class CommonTypes
    {
        /// <summary>Type declaration for a <c>typeof(object)</c>.</summary>
        public static readonly Type ObjectType = typeof(object);

        /// <summary>Type declaration for a <c>typeof(string)</c>.</summary>
        public static readonly Type StringType = typeof(string);

        /// <summary>Type declaration for a <c>typeof(bool)</c>.</summary>
        public static readonly Type BoolType = typeof(bool);

        /// <summary>Type declaration for a <c>typeof(byte)</c>.</summary>
        public static readonly Type ByteType = typeof(byte);

        /// <summary>Type declaration for a <c>typeof(sbyte)</c>.</summary>
        public static readonly Type SByteType = typeof(sbyte);

        /// <summary>Type declaration for a <c>typeof(ushort)</c>.</summary>
        public static readonly Type UShortType = typeof(ushort);

        /// <summary>Type declaration for a <c>typeof(short)</c>.</summary>
        public static readonly Type ShortType = typeof(short);

        /// <summary>Type declaration for a <c>typeof(uint)</c>.</summary>
        public static readonly Type UIntType = typeof(uint);

        /// <summary>Type declaration for a <c>typeof(int)</c>.</summary>
        public static readonly Type IntType = typeof(int);

        /// <summary>Type declaration for a <c>typeof(ulong)</c>.</summary>
        public static readonly Type ULongType = typeof(ulong);

        /// <summary>Type declaration for a <c>typeof(long)</c>.</summary>
        public static readonly Type LongType = typeof(long);

        /// <summary>Type declaration for a <c>typeof(float)</c>.</summary>
        public static readonly Type FloatType = typeof(float);

        /// <summary>Type declaration for a <c>typeof(double)</c>.</summary>
        public static readonly Type DoubleType = typeof(double);

        /// <summary>Type declaration for a <c>typeof(decimal)</c>.</summary>
        public static readonly Type DecimalType = typeof(decimal);

        /// <summary>Type declaration for a <c>typeof(char)</c>.</summary>
        public static readonly Type CharType = typeof(char);

        /// <summary>Type declaration for a <c>typeof(Enum)</c>.</summary>
        public static readonly Type EnumType = typeof(Enum);

        /// <summary>Type declaration for a <c>typeof(Guid)</c>.</summary>
        public static readonly Type GuidType = typeof(Guid);

        /// <summary>Type declaration for a <c>typeof(DateTime)</c>.</summary>
        public static readonly Type DateTimeType = typeof(DateTime);

        /// <summary>Type declaration for a <c>typeof(TimeSpan)</c>.</summary>
        public static readonly Type TimeSpanType = typeof(TimeSpan);

        /// <summary>Type declaration for a <c>typeof(Nullable&lt;&gt;)</c>.</summary>
        public static readonly Type NullableGenericType = typeof(Nullable<>);

        /// <summary>Type declaration for a <c>typeof(IEnumerable)</c>.</summary>
        public static readonly Type IEnumerableType = typeof(IEnumerable);

        /// <summary>Type declaration for a <c>typeof(IEnumerable&lt;&gt;)</c>.</summary>
        public static readonly Type IEnumerableGenericType = typeof(IEnumerable<>);

        /// <summary>Type declaration for a <c>typeof(List&lt;&gt;)</c>.</summary>
        public static readonly Type ListGenericType = typeof(List<>);

        /// <summary>Type declaration for a <c>typeof(StringComparison)</c>.</summary>
        public static readonly Type StringComparisonType = typeof(StringComparison);

        /// <summary>Type declaration for a <c>typeof(EnrichedEnum&lt;&gt;)</c>.</summary>
        public static readonly Type EnrichedEnumGenericType = typeof(EnrichedEnum<>);
    }
}
