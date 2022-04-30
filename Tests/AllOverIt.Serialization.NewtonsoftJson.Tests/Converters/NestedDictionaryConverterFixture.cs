using AllOverIt.Fixture;
using AllOverIt.Serialization.NewtonsoftJson.Converters;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Serialization.NewtonsoftJson.Tests.Converters
{
    public class NestedDictionaryConverterFixture : FixtureBase
    {
        private class DummyDictionary
        {
            public Dictionary<string, object> Prop { get; set; }
        }

        private NestedDictionaryConverter _converter;
        private NewtonsoftJsonSerializer _serializer;

        public class CanConvert : NestedDictionaryConverterFixture
        {
            [Theory]
            [InlineData(typeof(IDictionary<string, object>), false)]
            [InlineData(typeof(Dictionary<string, object>), true)]
            public void Should_Return_Expected_CanConvert_Result(Type type, bool expected)
            {
                InitializeSerializerAndConverter();

                var actual = _converter.CanConvert(type);

                actual.Should().Be(expected);
            }
        }

        public class ReadJson : NestedDictionaryConverterFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Read_As_Dictionary(bool includeUppercase)
            {
                InitializeSerializerAndConverter();

                var prop1 = Create<int>();

                var prop2 = new
                {
                    Value = Create<string>()
                };

                //var prop3 = new
                //{
                //    Value1 = prop2,
                //    Value2 = prop1
                //};

                var prop2Dictionary = includeUppercase
                    ? new Dictionary<string, object> {{"Value", prop2.Value}}
                    : new Dictionary<string, object> {{"value", prop2.Value}};

                var prop3Dictionary = includeUppercase
                    ? new Dictionary<string, object> {{"Value1", prop2Dictionary}, {"Value2", prop1}}
                    : new Dictionary<string, object> {{"value1", prop2Dictionary}, {"value2", prop1}};

                var expected = includeUppercase
                    ? new Dictionary<string, object>
                    {
                        {"Prop1", prop1},
                        {"Prop2", prop2Dictionary},
                        {"Prop3", prop3Dictionary}
                    }
                    : new Dictionary<string, object>
                    {
                        {"prop1", prop1},
                        {"prop2", prop2Dictionary},
                        {"prop3", prop3Dictionary}
                    };

                var value = includeUppercase
                    ? $@"{{""Prop"":{{""Prop1"":{prop1},""Prop2"":{{""Value"":""{prop2.Value}""}},""Prop3"":{{""Value1"":{{""Value"":""{prop2.Value}""}},""Value2"":{prop1}}}}}}}"
                    : $@"{{""Prop"":{{""prop1"":{prop1},""prop2"":{{""value"":""{prop2.Value}""}},""prop3"":{{""value1"":{{""value"":""{prop2.Value}""}},""value2"":{prop1}}}}}}}";

                var actual = _serializer.DeserializeObject<DummyDictionary>(value);

                expected.Should().BeEquivalentTo(actual.Prop);
            }
        }

        public class WriteJson : NestedDictionaryConverterFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Write_Dictionary(bool includeUppercase)
            {
                InitializeSerializerAndConverter();

                var prop1 = Create<int>();

                var prop2 = new
                {
                    Value = Create<string>()
                };

                //var prop3 = new
                //{
                //    Value1 = prop2,
                //    Value2 = prop1
                //};

                var prop2Dictionary = includeUppercase
                    ? new Dictionary<string, object> { { "Value", prop2.Value } }
                    : new Dictionary<string, object> { { "value", prop2.Value } };

                var prop3Dictionary = includeUppercase
                    ? new Dictionary<string, object> { { "Value1", prop2Dictionary }, { "Value2", prop1 } }
                    : new Dictionary<string, object> { { "value1", prop2Dictionary }, { "value2", prop1 } };

                var dummyValue = includeUppercase
                    ? (object) new
                    {
                        Prop = new Dictionary<string, object>
                        {
                            {"Prop1", prop1},
                            {"Prop2", prop2Dictionary},
                            {"Prop3", prop3Dictionary}
                        }
                    }
                    : new
                    {
                        prop = new Dictionary<string, object>
                        {
                            {"prop1", prop1},
                            {"prop2", prop2Dictionary},
                            {"prop3", prop3Dictionary}
                        }
                    };

                var actual = _serializer.SerializeObject(dummyValue);

                var expected = includeUppercase
                    ? $@"{{""Prop"":{{""Prop1"":{prop1},""Prop2"":{{""Value"":""{prop2.Value}""}},""Prop3"":{{""Value1"":{{""Value"":""{prop2.Value}""}},""Value2"":{prop1}}}}}}}"
                    : $@"{{""prop"":{{""prop1"":{prop1},""prop2"":{{""value"":""{prop2.Value}""}},""prop3"":{{""value1"":{{""value"":""{prop2.Value}""}},""value2"":{prop1}}}}}}}";

                actual.Should().Be(expected);
            }
        }

        private void InitializeSerializerAndConverter()
        {
            _converter = new NestedDictionaryConverter();

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(_converter);

            _serializer = new NewtonsoftJsonSerializer(settings);
        }
    }
}