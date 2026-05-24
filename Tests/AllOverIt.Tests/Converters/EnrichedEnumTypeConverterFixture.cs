using AllOverIt.Converters;
using AllOverIt.Fixture;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Patterns.Enumeration;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace AllOverIt.Tests.Converters
{
    public class EnrichedEnumTypeConverterFixture : FixtureBase
    {
        [TypeConverter(typeof(EnrichedEnumTypeConverter<DummyEnrichedEnum>))]
        private class DummyEnrichedEnum : EnrichedEnum<DummyEnrichedEnum>
        {
            public static readonly DummyEnrichedEnum Value1 = new(1);

            public static readonly DummyEnrichedEnum Value2 = new(2, "Value 2");

            private DummyEnrichedEnum(int value, [CallerMemberName] string name = "")
                : base(value, name)
            {
            }
        }

        private readonly EnrichedEnumTypeConverter<DummyEnrichedEnum> _enrichedEnumConverter;
        private readonly ITypeDescriptorContext _typeDescriptorContextFake;

        protected EnrichedEnumTypeConverterFixture()
        {
            _enrichedEnumConverter = new EnrichedEnumTypeConverter<DummyEnrichedEnum>();
            _typeDescriptorContextFake = this.CreateStub<ITypeDescriptorContext>();
        }

        public class CanConvertFrom : EnrichedEnumTypeConverterFixture
        {
            [Fact]
            public void Should_Convert_From_String()
            {
                var actual = _enrichedEnumConverter.CanConvertFrom(_typeDescriptorContextFake, typeof(string));

                actual.ShouldBeTrue();
            }

            [Theory]
            [ClassData(typeof(DummyTypeTestData))]
            public void Should_Convert_From_Integral(Type type)
            {
                var actual = _enrichedEnumConverter.CanConvertFrom(_typeDescriptorContextFake, type);

                actual.ShouldBeTrue();
            }

            [Fact]
            public void Should_Not_Convert_From_Boolean()
            {
                var actual = _enrichedEnumConverter.CanConvertFrom(_typeDescriptorContextFake, typeof(bool));

                actual.ShouldBeFalse();
            }
        }

        public class ConvertFrom : EnrichedEnumTypeConverterFixture
        {
            [Fact]
            public void Should_Return_Null()
            {
                var actual = _enrichedEnumConverter.ConvertFrom(_typeDescriptorContextFake, CultureInfo.CurrentCulture, null);

                actual.ShouldBeNull();
            }

            [Theory]
            [ClassData(typeof(DummyIntegralTestData))]
            public void Should_Convert_From_Integral(object value)
            {
                var actual = (DummyEnrichedEnum) _enrichedEnumConverter.ConvertFrom(_typeDescriptorContextFake, CultureInfo.CurrentCulture, value);

                actual.ShouldBe(DummyEnrichedEnum.Value2);
            }

            [Theory]
            [InlineData("1", nameof(DummyEnrichedEnum.Value1))]
            [InlineData("2", "Value 2")]
            public void Should_Convert_From_String(string value, string expectedName)
            {
                var actual = (DummyEnrichedEnum) _enrichedEnumConverter.ConvertFrom(_typeDescriptorContextFake, CultureInfo.CurrentCulture, value);

                actual!.Name.ShouldBe(expectedName);
            }
        }

        public class CanConvertTo : EnrichedEnumTypeConverterFixture
        {
            [Fact]
            public void Should_Convert_To_String()
            {
                var actual = _enrichedEnumConverter.CanConvertTo(_typeDescriptorContextFake, typeof(string));

                actual.ShouldBeTrue();
            }

            [Theory]
            [ClassData(typeof(DummyTypeTestData))]
            public void Should_Convert_To_Integral(Type type)
            {
                var actual = _enrichedEnumConverter.CanConvertTo(_typeDescriptorContextFake, type);

                actual.ShouldBeTrue();
            }

            [Fact]
            public void Should_Not_Convert_To_Boolean()
            {
                var actual = _enrichedEnumConverter.CanConvertTo(_typeDescriptorContextFake, typeof(bool));

                actual.ShouldBeFalse();
            }
        }

        public class ConvertTo : EnrichedEnumTypeConverterFixture
        {
            [Fact]
            public void Should_Return_Null()
            {
                DummyEnrichedEnum value = null;

                var actual = _enrichedEnumConverter.ConvertTo(value!, typeof(string));

                actual.ShouldBeNull();
            }

            [Theory]
            [ClassData(typeof(DummyTypeTestData))]
            public void Should_Convert_To_Integral(Type type)
            {
                var actual = _enrichedEnumConverter.ConvertTo(DummyEnrichedEnum.Value2, type);

                actual.ShouldBeOfType(type);
                actual.ShouldBe(2);
            }

            [Fact]
            public void Should_Convert_To_String()
            {
                var actual = _enrichedEnumConverter.ConvertTo(DummyEnrichedEnum.Value2, typeof(string));

                actual.ShouldBeOfType(typeof(string));
                actual.ShouldBe("Value 2");
            }
        }

        public class Create : EnrichedEnumTypeConverterFixture
        {
            [Fact]
            public void Should_Create_Type_Converter()
            {
                var actual = EnrichedEnumTypeConverter<DummyEnrichedEnum>.Create();

                actual.ShouldBeOfType<EnrichedEnumTypeConverter<DummyEnrichedEnum>>();
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



