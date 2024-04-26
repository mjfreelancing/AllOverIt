using AllOverIt.Fixture;
using AllOverIt.Patterns.Enumeration;
using AllOverIt.Serialization.Json.SystemText.Converters;
using FluentAssertions;
using System;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace AllOverIt.Serialization.Json.SystemText.Tests.Converters
{
    public class EnrichedEnumJsonConverterFixture : FixtureBase
    {
        private class DummyEnrichedEnum : EnrichedEnum<DummyEnrichedEnum>
        {
            public static readonly DummyEnrichedEnum Value1 = new(1);

            public static readonly DummyEnrichedEnum Value2 = new(2, "Value 2");

            public DummyEnrichedEnum()     // required for serialization
            {
            }

            private DummyEnrichedEnum(int value, [CallerMemberName] string name = null)
                : base(value, name)
            {
            }
        }

        private class DummyValue
        {
            public DummyEnrichedEnum Prop1 { get; set; }
        }

        private readonly SystemTextJsonSerializer _serializer;
        private readonly DummyValue _dummyValue;

        protected EnrichedEnumJsonConverterFixture()
        {
            var converter = new EnrichedEnumJsonConverter<DummyEnrichedEnum>();

            var options = new JsonSerializerOptions();
            options.Converters.Add(converter);

            _serializer = new SystemTextJsonSerializer(options);

            var rnd = new Random((int) DateTime.Now.Ticks);

            _dummyValue = new DummyValue
            {
                Prop1 = rnd.Next() % 2 == 0
                    ? DummyEnrichedEnum.Value1
                    : DummyEnrichedEnum.Value2
            };
        }

        public class Read : EnrichedEnumJsonConverterFixture
        {
            [Fact]
            public void Should_Read_Enum_Value()
            {
                var value = $@"{{""Prop1"":""{_dummyValue.Prop1.Name}""}}";

                var actual = _serializer.DeserializeObject<DummyValue>(value);

                var expected = _dummyValue.Prop1 == DummyEnrichedEnum.Value1
                    ? DummyEnrichedEnum.Value1
                    : DummyEnrichedEnum.Value2;

                actual.Prop1.Should().Be(expected);
            }

            [Fact]
            public void Should_Read_Null_Enum_Value()
            {
                var value = $@"{{""Prop1"":null}}";

                var actual = _serializer.DeserializeObject<DummyValue>(value);

                actual.Prop1.Should().BeNull();
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
                var actual = EnrichedEnumJsonConverter<DummyEnrichedEnum>.Create();

                actual.Should().BeOfType<EnrichedEnumJsonConverter<DummyEnrichedEnum>>();
            }
        }
    }
}