using AllOverIt.Fixture;
using AllOverIt.Serialization.NewtonsoftJson.Converters;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Serialization.NewtonsoftJson.Tests.Converters
{
    public class StringObjectDictionaryConverterFixture : FixtureBase
    {
        private class DummyDictionary
        {
            public Dictionary<string, object> Prop { get; set; }
        }

        private readonly StringObjectDictionaryConverter _converter;
        private readonly NewtonsoftJsonSerializer _serializer;

        protected StringObjectDictionaryConverterFixture()
        {
            _converter = new StringObjectDictionaryConverter();

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(_converter);

            _serializer = new NewtonsoftJsonSerializer(settings);
        }

        public class CanConvert : StringObjectDictionaryConverterFixture
        {
            [Theory]
            [InlineData(typeof(IDictionary<string, object>), false)]
            [InlineData(typeof(Dictionary<string, object>), true)]
            public void Should_Return_Expected_CanConvert_Result(Type type, bool expected)
            {
                var actual = _converter.CanConvert(type);

                actual.Should().Be(expected);
            }
        }

        public class ReadJson : StringObjectDictionaryConverterFixture
        {
            [Fact]
            public void Should_Read_As_Dictionary()
            {
                var prop1 = Create<int>();

                var prop2 = new
                {
                    Value = Create<string>()
                };

                var prop3 = new
                {
                    Value1 = prop2,
                    Value2 = prop1
                };

                var prop2Dictionary = new Dictionary<string, object> {{"Value", prop2.Value}};
                var prop3Dictionary = new Dictionary<string, object> {{"Value1", prop2Dictionary }, {"Value2", prop1}};

                var expected = new Dictionary<string, object>
                {
                    {"Prop1", prop1},
                    {"Prop2", prop2Dictionary},
                    {"Prop3", prop3Dictionary}
                };

                var value = $@"{{""Prop"":{{""Prop1"":{prop1},""Prop2"":{{""Value"":""{prop2.Value}""}},""Prop3"":{{""Value1"":{{""Value"":""{prop2.Value}""}},""Value2"":{prop1}}}}}}}";

                var actual = _serializer.DeserializeObject<DummyDictionary>(value);

                expected.Should().BeEquivalentTo(actual.Prop);
            }

            public class WriteJson : StringObjectDictionaryConverterFixture
            {
                [Fact]
                public void Should_Write_Dictionary()
                {
                    var prop1 = Create<int>();

                    var prop2 = new
                    {
                        Value = Create<string>()
                    };

                    var prop3 = new
                    {
                        Value1 = prop2,
                        Value2 = prop1
                    };

                    var prop2Dictionary = new Dictionary<string, object> { { "Value", prop2.Value } };
                    var prop3Dictionary = new Dictionary<string, object> { { "Value1", prop2Dictionary }, { "Value2", prop1 } };

                    var dummyValue = new
                    {
                        Prop = new Dictionary<string, object>
                        {
                            {"Prop1", prop1},
                            {"Prop2", prop2Dictionary},
                            {"Prop3", prop3Dictionary}
                        }
                    };

                    var actual = _serializer.SerializeObject(dummyValue);

                    var expected = $@"{{""Prop"":{{""Prop1"":{prop1},""Prop2"":{{""Value"":""{prop2.Value}""}},""Prop3"":{{""Value1"":{{""Value"":""{prop2.Value}""}},""Value2"":{prop1}}}}}}}";

                    actual.Should().Be(expected);
                }
            }
        }
    }
}