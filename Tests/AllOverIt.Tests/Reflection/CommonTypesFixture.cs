﻿using AllOverIt.Fixture;
using System.Collections.Generic;
using System;
using Xunit;
using FluentAssertions;
using AllOverIt.Reflection;
using System.Collections;
using AllOverIt.Patterns.Enumeration;

namespace AllOverIt.Tests.Reflection
{
    public class CommonTypesFixture : FixtureBase
    {
        [Theory]
        [MemberData(nameof(GetTypeDeclarations))]
        public void Should_Have_Expected_Type(Type commonType, Type expected)
        {
            commonType.Should().BeSameAs(expected);
        }

        public static IEnumerable<object[]> GetTypeDeclarations()
        {
            yield return new object[] { CommonTypes.ObjectType, typeof(object) };
            yield return new object[] { CommonTypes.StringType, typeof(string) };
            yield return new object[] { CommonTypes.BoolType, typeof(bool) };
            yield return new object[] { CommonTypes.ByteType, typeof(byte) };
            yield return new object[] { CommonTypes.SByteType, typeof(sbyte) };
            yield return new object[] { CommonTypes.UShortType, typeof(ushort) };
            yield return new object[] { CommonTypes.ShortType, typeof(short) };
            yield return new object[] { CommonTypes.UIntType, typeof(uint) };
            yield return new object[] { CommonTypes.IntType, typeof(int) };
            yield return new object[] { CommonTypes.ULongType, typeof(ulong) };
            yield return new object[] { CommonTypes.LongType, typeof(long) };
            yield return new object[] { CommonTypes.FloatType, typeof(float) };
            yield return new object[] { CommonTypes.DoubleType, typeof(double) };
            yield return new object[] { CommonTypes.DecimalType, typeof(decimal) };
            yield return new object[] { CommonTypes.CharType, typeof(char) };
            yield return new object[] { CommonTypes.EnumType, typeof(Enum) };
            yield return new object[] { CommonTypes.GuidType, typeof(Guid) };
            yield return new object[] { CommonTypes.DateTimeType, typeof(DateTime) };
            yield return new object[] { CommonTypes.TimeSpanType, typeof(TimeSpan) };
            yield return new object[] { CommonTypes.NullableGenericType, typeof(Nullable<>) };
            yield return new object[] { CommonTypes.IEnumerableType, typeof(IEnumerable) };
            yield return new object[] { CommonTypes.IEnumerableGenericType, typeof(IEnumerable<>) };
            yield return new object[] { CommonTypes.ListGenericType, typeof(List<>) };
            yield return new object[] { CommonTypes.StringComparisonType, typeof(StringComparison) };
            yield return new object[] { CommonTypes.EnrichedEnumGenericType, typeof(EnrichedEnum<>) };
        }
    }
}