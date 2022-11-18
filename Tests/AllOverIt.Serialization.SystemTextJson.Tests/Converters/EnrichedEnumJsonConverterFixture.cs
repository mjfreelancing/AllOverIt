using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using AllOverIt.Fixture;
using AllOverIt.Patterns.Enumeration;
using AllOverIt.Serialization.SystemTextJson.Converters;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Serialization.SystemTextJson.Tests.Converters
{
    public class EnrichedEnumJsonConverterFixture : FixtureBase
    {
        private class EnrichedEnumDummy : EnrichedEnum<EnrichedEnumDummy>
        {
            public static readonly EnrichedEnumDummy Value1 = new(1);

            public static readonly EnrichedEnumDummy Value2 = new(2, "Value 2");

            public EnrichedEnumDummy()     // required for serialization
            {
            }

            private EnrichedEnumDummy(int value, [CallerMemberName] string name = null)
                : base(value, name)
            {
            }
        }

        private class DummyValue
        {
            public EnrichedEnumDummy Prop1 { get; set; }
        }

        private readonly SystemTextJsonSerializer _serializer;
        private readonly DummyValue _dummyValue;

        protected EnrichedEnumJsonConverterFixture()
        {
            var converter = new EnrichedEnumJsonConverter<EnrichedEnumDummy>();

            var options = new JsonSerializerOptions();
            options.Converters.Add(converter);

            _serializer = new SystemTextJsonSerializer(options);

            var rnd = new Random((int)DateTime.Now.Ticks);

            _dummyValue = new DummyValue
            {
                Prop1 = rnd.Next() % 2 == 0
                    ? EnrichedEnumDummy.Value1
                    : EnrichedEnumDummy.Value2
            };
        }

        public class Read : EnrichedEnumJsonConverterFixture
        {
            [Fact]
            public void Should_Read_Enum_Value()
            {
                var value = $@"{{""Prop1"":""{_dummyValue.Prop1.Name}""}}";

                var actual = _serializer.DeserializeObject<DummyValue>(value);

                var expected = _dummyValue.Prop1 == EnrichedEnumDummy.Value1
                    ? EnrichedEnumDummy.Value1
                    : EnrichedEnumDummy.Value2;

                actual.Prop1.Should().Be(expected);
            }
        }

        public class Write : EnrichedEnumJsonConverterFixture
        {
            [Fact]
            public void Should_Write_EnrichedEnum_Name()
            {
                var actual = _serializer.SerializeObject(_dummyValue);

                var expected = $@"{{""Prop1"":""{_dummyValue.Prop1.Name}""}}";

                actual.Should().Be(expected);
            }
        }

        public class Create : EnrichedEnumJsonConverterFixture
        {
            [Fact]
            public void Should_Create_Converter()
            {
                var actual = EnrichedEnumJsonConverter<EnrichedEnumDummy>.Create();

                actual.Should().BeOfType<EnrichedEnumJsonConverter<EnrichedEnumDummy>>();
            }
        }
    }
}