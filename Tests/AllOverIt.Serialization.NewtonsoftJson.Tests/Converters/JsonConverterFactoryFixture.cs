using System;
using System.Runtime.CompilerServices;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Patterns.Enumeration;
using AllOverIt.Serialization.NewtonsoftJson.Converters;
using AllOverIt.Serialization.NewtonsoftJson.Extensions;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace AllOverIt.Serialization.NewtonsoftJson.Tests.Converters
{
    public class JsonConverterFactoryFixture : FixtureBase
    {
        private class EnrichedEnumDummy1 : EnrichedEnum<EnrichedEnumDummy1>
        {
            public static readonly EnrichedEnumDummy1 Value1 = new(0);
            public static readonly EnrichedEnumDummy1 Value2 = new(1);

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

        private class EnrichedEnumDummyJsonConverterFactory : JsonConverterFactory
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
            public EnrichedEnumDummy1 Prop1 { get; set; }
            public EnrichedEnumDummy2 Prop2 { get; set; }
        }

        private readonly NewtonsoftJsonSerializer _serializer;
        private readonly JsonConverterFactory _factory;
        private readonly DummyValue _dummyValue;

        protected JsonConverterFactoryFixture()
        {
            _serializer = new NewtonsoftJsonSerializer();
            _factory = new EnrichedEnumDummyJsonConverterFactory();
            _serializer.AddConverters(_factory);

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

        // Given a name so it doesn't conflict with the method on EnrichedEnumDummyJsonConverterFactory
        public class CreateConverter_Factory : JsonConverterFactoryFixture
        {
            [Fact]
            public void Should_Create_Converter()
            {
                var actual = _factory.CreateConverter(typeof(EnrichedEnumDummy2));

                actual.Should().BeOfType<EnrichedEnumJsonConverter<EnrichedEnumDummy2>>();
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
                    Prop1 = _dummyValue.Prop1 == EnrichedEnumDummy1.Value1 ? EnrichedEnumDummy1.Value1 : EnrichedEnumDummy1.Value2,
                    Prop2 = _dummyValue.Prop2 == EnrichedEnumDummy2.Value3 ? EnrichedEnumDummy2.Value3 : EnrichedEnumDummy2.Value4,
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