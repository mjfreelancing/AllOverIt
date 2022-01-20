using AllOverIt.Converters;
using AllOverIt.Fixture;
using AllOverIt.Patterns.Enumeration;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Xunit;

namespace AllOverIt.Tests.Converters
{
    public class EnrichedEnumTypeConverterFixture : FixtureBase
    {
        [TypeConverter(typeof(EnrichedEnumTypeConverter<EnrichedEnumDummy>))]
        private class EnrichedEnumDummy : EnrichedEnum<EnrichedEnumDummy>
        {
            public static readonly EnrichedEnumDummy Value1 = new(1);

            // ReSharper disable once ExplicitCallerInfoArgument
            public static readonly EnrichedEnumDummy Value2 = new(2, "Value 2");

            private EnrichedEnumDummy(int value, [CallerMemberName] string name = null)
                : base(value, name)
            {
            }
        }

        private readonly EnrichedEnumTypeConverter<EnrichedEnumDummy> _enrichedEnumConverter;
        private readonly ITypeDescriptorContext _typeDescriptorContextFake;

        protected EnrichedEnumTypeConverterFixture()
        {
            _enrichedEnumConverter = new EnrichedEnumTypeConverter<EnrichedEnumDummy>();
            _typeDescriptorContextFake = A.Fake<ITypeDescriptorContext>();
        }

        public class CanConvertFrom : EnrichedEnumTypeConverterFixture
        {
            [Fact]
            public void Should_Convert_From_String()
            {
                var actual = _enrichedEnumConverter.CanConvertFrom(_typeDescriptorContextFake, typeof(string));

                actual.Should().BeTrue();
            }

            [Theory]
            [ClassData(typeof(DummyTypeTestData))]
            public void Should_Convert_From_Integral(Type type)
            {
                var actual = _enrichedEnumConverter.CanConvertFrom(_typeDescriptorContextFake, type);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Convert_From_Boolean()
            {
                var actual = _enrichedEnumConverter.CanConvertFrom(_typeDescriptorContextFake, typeof(bool));

                actual.Should().BeFalse();
            }
        }

        public class ConvertFrom : EnrichedEnumTypeConverterFixture
        {
            [Fact]
            public void Should_Return_Null()
            {
                var actual = _enrichedEnumConverter.ConvertFrom(_typeDescriptorContextFake, CultureInfo.CurrentCulture, null);

                actual.Should().BeNull();
            }

            [Theory]
            [ClassData(typeof(DummyIntegralTestData))]
            public void Should_Convert_From_Integral(object value)
            {
                var actual = (EnrichedEnumDummy) _enrichedEnumConverter.ConvertFrom(_typeDescriptorContextFake, CultureInfo.CurrentCulture, value);

                actual.Should().Be(EnrichedEnumDummy.Value2);
            }

            [Theory]
            [InlineData("1", nameof(EnrichedEnumDummy.Value1))]
            [InlineData("2", "Value 2")]
            public void Should_Convert_From_String(string value, string expectedName)
            {
                var actual = (EnrichedEnumDummy) _enrichedEnumConverter.ConvertFrom(_typeDescriptorContextFake, CultureInfo.CurrentCulture, value);

                actual!.Name.Should().Be(expectedName);
            }
        }

        public class CanConvertTo : EnrichedEnumTypeConverterFixture
        {
            [Fact]
            public void Should_Convert_To_String()
            {
                var actual = _enrichedEnumConverter.CanConvertTo(_typeDescriptorContextFake, typeof(string));

                actual.Should().BeTrue();
            }

            [Theory]
            [ClassData(typeof(DummyTypeTestData))]
            public void Should_Convert_To_Integral(Type type)
            {
                var actual = _enrichedEnumConverter.CanConvertTo(_typeDescriptorContextFake, type);

                actual.Should().BeTrue();
            }

            [Fact]
            public void Should_Not_Convert_To_Boolean()
            {
                var actual = _enrichedEnumConverter.CanConvertTo(_typeDescriptorContextFake, typeof(bool));

                actual.Should().BeFalse();
            }
        }

        public class ConvertTo : EnrichedEnumTypeConverterFixture
        {
            [Fact]
            public void Should_Return_Null()
            {
                EnrichedEnumDummy value = null;

                var actual = _enrichedEnumConverter.ConvertTo(value!, typeof(string));

                actual.Should().BeNull();
            }

            [Theory]
            [ClassData(typeof(DummyTypeTestData))]
            public void Should_Convert_To_Integral(Type type)
            {
                var actual = _enrichedEnumConverter.ConvertTo(EnrichedEnumDummy.Value2, type);

                actual.Should().BeOfType(type);
                actual.Should().Be(2);
            }

            [Fact]
            public void Should_Convert_To_String()
            {
                var actual = _enrichedEnumConverter.ConvertTo(EnrichedEnumDummy.Value2, typeof(string));

                actual.Should().BeOfType(typeof(string));
                actual.Should().Be("Value 2");
            }
        }

        private class DummyIntegralTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { (byte) 2 };
                yield return new object[] { (sbyte) 2 };
                yield return new object[] { (short) 2 };
                yield return new object[] { (ushort) 2 };
                yield return new object[] { (int) 2 };
                yield return new object[] { (uint) 2 };
                yield return new object[] { (long) 2 };
                yield return new object[] { (ulong) 2 };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        private class DummyTypeTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { typeof(byte) };
                yield return new object[] { typeof(sbyte) };
                yield return new object[] { typeof(short) };
                yield return new object[] { typeof(ushort) };
                yield return new object[] { typeof(int) };
                yield return new object[] { typeof(uint) };
                yield return new object[] { typeof(long) };
                yield return new object[] { typeof(ulong) };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
