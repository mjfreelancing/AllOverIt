using AllOverIt.Fixture;
using AllOverIt.Patterns.Enumeration;
using AllOverIt.Reflection;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;

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
            yield return new object[] { CommonTypes.NullableBoolType, typeof(bool?) };
            yield return new object[] { CommonTypes.ByteType, typeof(byte) };
            yield return new object[] { CommonTypes.NullableByteType, typeof(byte?) };
            yield return new object[] { CommonTypes.SByteType, typeof(sbyte) };
            yield return new object[] { CommonTypes.NullableSByteType, typeof(sbyte?) };
            yield return new object[] { CommonTypes.UShortType, typeof(ushort) };
            yield return new object[] { CommonTypes.NullableUShortType, typeof(ushort?) };
            yield return new object[] { CommonTypes.ShortType, typeof(short) };
            yield return new object[] { CommonTypes.NullableShortType, typeof(short?) };
            yield return new object[] { CommonTypes.UIntType, typeof(uint) };
            yield return new object[] { CommonTypes.NullableUIntType, typeof(uint?) };
            yield return new object[] { CommonTypes.IntType, typeof(int) };
            yield return new object[] { CommonTypes.NullableIntType, typeof(int?) };
            yield return new object[] { CommonTypes.ULongType, typeof(ulong) };
            yield return new object[] { CommonTypes.NullableULongType, typeof(ulong?) };
            yield return new object[] { CommonTypes.LongType, typeof(long) };
            yield return new object[] { CommonTypes.NullableLongType, typeof(long?) };
            yield return new object[] { CommonTypes.FloatType, typeof(float) };
            yield return new object[] { CommonTypes.NullableFloatType, typeof(float?) };
            yield return new object[] { CommonTypes.DoubleType, typeof(double) };
            yield return new object[] { CommonTypes.NullableDoubleType, typeof(double?) };
            yield return new object[] { CommonTypes.DecimalType, typeof(decimal) };
            yield return new object[] { CommonTypes.NullableDecimalType, typeof(decimal?) };
            yield return new object[] { CommonTypes.CharType, typeof(char) };
            yield return new object[] { CommonTypes.NullableCharType, typeof(char?) };
            yield return new object[] { CommonTypes.EnumType, typeof(Enum) };
            yield return new object[] { CommonTypes.GuidType, typeof(Guid) };
            yield return new object[] { CommonTypes.NullableGuidType, typeof(Guid?) };
            yield return new object[] { CommonTypes.DateTimeType, typeof(DateTime) };
            yield return new object[] { CommonTypes.NullableDateTimeType, typeof(DateTime?) };
            yield return new object[] { CommonTypes.TimeSpanType, typeof(TimeSpan) };
            yield return new object[] { CommonTypes.NullableTimeSpanType, typeof(TimeSpan?) };
            yield return new object[] { CommonTypes.NullableGenericType, typeof(Nullable<>) };
            yield return new object[] { CommonTypes.IEnumerableType, typeof(IEnumerable) };
            yield return new object[] { CommonTypes.IEnumerableGenericType, typeof(IEnumerable<>) };
            yield return new object[] { CommonTypes.ICollectionType, typeof(ICollection) };
            yield return new object[] { CommonTypes.ICollectionGenericType, typeof(ICollection<>) };
            yield return new object[] { CommonTypes.IListGenericType, typeof(IList<>) };
            yield return new object[] { CommonTypes.ListGenericType, typeof(List<>) };
            yield return new object[] { CommonTypes.IDictionaryGenericType, typeof(IDictionary<,>) };
            yield return new object[] { CommonTypes.DictionaryGenericType, typeof(Dictionary<,>) };
            yield return new object[] { CommonTypes.KeyValuePairType, typeof(KeyValuePair<,>) };
            yield return new object[] { CommonTypes.StringComparisonType, typeof(StringComparison) };
            yield return new object[] { CommonTypes.EnrichedEnumGenericType, typeof(EnrichedEnum<>) };
            yield return new object[] { CommonTypes.IConvertibleType, typeof(IConvertible) };
        }
    }
}