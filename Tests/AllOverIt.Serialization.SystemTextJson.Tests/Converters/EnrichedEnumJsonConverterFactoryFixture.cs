using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using AllOverIt.Fixture;
using AllOverIt.Patterns.Enumeration;
using AllOverIt.Serialization.SystemTextJson.Converters;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Serialization.SystemTextJson.Tests.Converters
{
    public class EnrichedEnumJsonConverterFactoryFixture : FixtureBase
    {
        private class EnrichedEnumDummy1 : EnrichedEnum<EnrichedEnumDummy1>
        {
            public static readonly EnrichedEnumDummy1 Value1 = new(1);
            public static readonly EnrichedEnumDummy1 Value2 = new(2);

            public EnrichedEnumDummy1()     // required for serialization
            {
            }

            private EnrichedEnumDummy1(int value, [CallerMemberName] string name = null)
                : base(value, name)
            {
            }
        }

        private class EnrichedEnumDummy2 : EnrichedEnum<EnrichedEnumDummy2>
        {
            public static readonly EnrichedEnumDummy2 Value3 = new(3);
            public static readonly EnrichedEnumDummy2 Value4 = new(4);

            public EnrichedEnumDummy2()     // required for serialization
            {
            }

            private EnrichedEnumDummy2(int value, [CallerMemberName] string name = null)
                : base(value, name)
            {
            }
        }

        private class DummyValue
        {
            public EnrichedEnumDummy1 Prop1 { get; set; }
            public EnrichedEnumDummy2 Prop2 { get; set; }
        }

        private readonly EnrichedEnumJsonConverterFactory _converterFactory;
        private readonly SystemTextJsonSerializer _serializer;

        protected EnrichedEnumJsonConverterFactoryFixture()
        {
            _converterFactory = new EnrichedEnumJsonConverterFactory();

            var options = new JsonSerializerOptions();
            options.Converters.Add(_converterFactory);

            _serializer = new SystemTextJsonSerializer(options);
        }

        public class Constructor : EnrichedEnumJsonConverterFactoryFixture
        {
            // There's no way to test caching without abstracting the internal cache
            [Fact]
            public void Should_Enable_Caching_As_Default()
            {
                _converterFactory.EnableCaching.Should().BeTrue();
            }
        }

        public class CanConvert : EnrichedEnumJsonConverterFactoryFixture
        {
            [Theory]
            [InlineData(typeof(EnrichedEnumDummy1), true)]
            [InlineData(typeof(EnrichedEnumDummy2), true)]
            [InlineData(typeof(string), false)]
            public void Should_Only_Convert_EnrichedEnum_Types(Type objectType, bool expected)
            {
                var actual = _converterFactory.CanConvert(objectType);

                actual.Should().Be(expected);
            }
        }

        public class CreateConverter : EnrichedEnumJsonConverterFactoryFixture
        {
            [Fact]
            public void Should_Create_Converter()
            {
                var converter = _converterFactory.CreateConverter(typeof(EnrichedEnumDummy1), new JsonSerializerOptions());

                converter.Should().BeOfType<EnrichedEnumJsonConverter<EnrichedEnumDummy1>>();
            }
        }

        public class Serialization : EnrichedEnumJsonConverterFactoryFixture
        {
            private readonly DummyValue _dummyValue;

            public Serialization()
            {
                var rnd = new Random((int) DateTime.Now.Ticks);

                _dummyValue = new DummyValue
                {
                    Prop1 = rnd.Next() % 2 == 0
                        ? EnrichedEnumDummy1.Value1
                        : EnrichedEnumDummy1.Value2,

                    Prop2 = rnd.Next() % 2 == 0
                        ? EnrichedEnumDummy2.Value3
                        : EnrichedEnumDummy2.Value4
                };
            }

            [Fact]
            public void Should_Serialize()
            {
                var expected = $@"{{""Prop1"":""{_dummyValue.Prop1}"",""Prop2"":""{_dummyValue.Prop2}""}}";

                var actual = _serializer.SerializeObject(_dummyValue);

                actual.Should().Be(expected);
            }

            [Fact]
            public void Should_Deserialize()
            {
                var value = $@"{{""Prop1"":""{_dummyValue.Prop1}"",""Prop2"":""{_dummyValue.Prop2}""}}";

                var actual = _serializer.DeserializeObject<DummyValue>(value);

                _dummyValue.Should().BeEquivalentTo(actual);
            }
        }
    }
}