using AllOverIt.Fixture;
using AllOverIt.Patterns.Enumeration;
using AllOverIt.Serialization.Json.Newtonsoft.Converters;
using FluentAssertions;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace AllOverIt.Serialization.Json.Newtonsoft.Tests.Converters
{
    public class EnrichedEnumJsonConverterFactoryFixture : FixtureBase
    {
        private class DummyEnrichedEnum1 : EnrichedEnum<DummyEnrichedEnum1>
        {
            public static readonly DummyEnrichedEnum1 Value1 = new(1);
            public static readonly DummyEnrichedEnum1 Value2 = new(2);

            public DummyEnrichedEnum1()     // required for serialization
            {
            }

            private DummyEnrichedEnum1(int value, [CallerMemberName] string name = null)
                : base(value, name)
            {
            }
        }

        private class DummyEnrichedEnum2 : EnrichedEnum<DummyEnrichedEnum2>
        {
            public static readonly DummyEnrichedEnum2 Value3 = new(3);
            public static readonly DummyEnrichedEnum2 Value4 = new(4);

            public DummyEnrichedEnum2()     // required for serialization
            {
            }

            private DummyEnrichedEnum2(int value, [CallerMemberName] string name = null)
                : base(value, name)
            {
            }
        }

        private class DummyValue
        {
            public DummyEnrichedEnum1 Prop1 { get; set; }
            public DummyEnrichedEnum2 Prop2 { get; set; }
        }

        private readonly EnrichedEnumJsonConverterFactory _converterFactory;
        private readonly NewtonsoftJsonSerializer _serializer;

        protected EnrichedEnumJsonConverterFactoryFixture()
        {
            _converterFactory = new EnrichedEnumJsonConverterFactory();

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(_converterFactory);

            _serializer = new NewtonsoftJsonSerializer(settings);
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
            [InlineData(typeof(DummyEnrichedEnum1), true)]
            [InlineData(typeof(DummyEnrichedEnum2), true)]
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
                var converter = _converterFactory.CreateConverter(typeof(DummyEnrichedEnum1));

                converter.Should().BeOfType<EnrichedEnumJsonConverter<DummyEnrichedEnum1>>();
            }

            [Fact]
            public void Should_Not_Cache_Converter()
            {
                var converterFactory = new EnrichedEnumJsonConverterFactory
                {
                    EnableCaching = false,
                };

                var converter1 = converterFactory.CreateConverter(typeof(DummyEnrichedEnum1));
                var converter2 = converterFactory.CreateConverter(typeof(DummyEnrichedEnum1));

                converter1.Should().BeOfType<EnrichedEnumJsonConverter<DummyEnrichedEnum1>>();
                converter1.Should().NotBeSameAs(converter2);
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
                        ? DummyEnrichedEnum1.Value1
                        : DummyEnrichedEnum1.Value2,

                    Prop2 = rnd.Next() % 2 == 0
                        ? DummyEnrichedEnum2.Value3
                        : DummyEnrichedEnum2.Value4
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