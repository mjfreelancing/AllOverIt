using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Patterns.Enumeration;
using AllOverIt.Serialization.Json.Newtonsoft.Converters;
using AllOverIt.Serialization.Json.Newtonsoft.Extensions;
using FluentAssertions;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace AllOverIt.Serialization.Json.Newtonsoft.Tests.Converters
{
    public class JsonConverterFactoryFixture : FixtureBase
    {
        private class DummyEnrichedEnum1 : EnrichedEnum<DummyEnrichedEnum1>
        {
            public static readonly DummyEnrichedEnum1 Value1 = new(0);
            public static readonly DummyEnrichedEnum1 Value2 = new(1);

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

        private class DummyEnrichedEnumJsonConverterFactory : JsonConverterFactory
        {
            public override bool CanConvert(Type objectType)
            {
                // The objectType is derived from EnrichedEnum<TEnum>, so need to get the generic from the base class.
                return objectType.IsEnrichedEnum() &&
                       objectType == objectType.BaseType!.GenericTypeArguments[0];      // must be MyEnum : EnrichedEnum<MyEnum>
            }

            public override JsonConverter CreateConverter(Type objectType)
            {
                var genericArg = objectType.BaseType!.GenericTypeArguments[0];
                var genericType = typeof(EnrichedEnumJsonConverter<>).MakeGenericType(genericArg);

                return (JsonConverter) Activator.CreateInstance(genericType);
            }
        }

        private class DummyValue
        {
            public DummyEnrichedEnum1 Prop1 { get; set; }
            public DummyEnrichedEnum2 Prop2 { get; set; }
        }

        private readonly NewtonsoftJsonSerializer _serializer;
        private readonly JsonConverterFactory _factory;
        private readonly DummyValue _dummyValue;

        protected JsonConverterFactoryFixture()
        {
            _serializer = new NewtonsoftJsonSerializer();
            _factory = new DummyEnrichedEnumJsonConverterFactory();
            _serializer.AddConverters(_factory);

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

        // Given a name so it doesn't conflict with the method on DummyEnrichedEnumJsonConverterFactory
        public class CreateConverter_Factory : JsonConverterFactoryFixture
        {
            [Fact]
            public void Should_Create_Converter()
            {
                var actual = _factory.CreateConverter(typeof(DummyEnrichedEnum2));

                actual.Should().BeOfType<EnrichedEnumJsonConverter<DummyEnrichedEnum2>>();
            }
        }

        public class ReadJson : JsonConverterFactoryFixture
        {
            [Fact]
            public void Should_Read_Enum_Values()
            {
                var value = $@"{{""Prop1"":""{_dummyValue.Prop1.Name}"",""Prop2"":""{_dummyValue.Prop2.Name}""}}";

                var actual = _serializer.DeserializeObject<DummyValue>(value);

                var expected = new
                {
                    Prop1 = _dummyValue.Prop1 == DummyEnrichedEnum1.Value1 ? DummyEnrichedEnum1.Value1 : DummyEnrichedEnum1.Value2,
                    Prop2 = _dummyValue.Prop2 == DummyEnrichedEnum2.Value3 ? DummyEnrichedEnum2.Value3 : DummyEnrichedEnum2.Value4,
                };

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class WriteJson : JsonConverterFactoryFixture
        {
            [Fact]
            public void Should_Write_Enum_Values()
            {
                // lower camel-case on the property names
                var expected = $@"{{""Prop1"":""{_dummyValue.Prop1.Name}"",""Prop2"":""{_dummyValue.Prop2.Name}""}}";

                var actual = _serializer.SerializeObject(_dummyValue);

                actual.Should().Be(expected);
            }
        }
    }
}