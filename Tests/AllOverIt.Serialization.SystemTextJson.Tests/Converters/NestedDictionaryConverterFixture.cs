using AllOverIt.Fixture;
using AllOverIt.Serialization.SystemTextJson.Converters;
using FluentAssertions;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace AllOverIt.Serialization.SystemTextJson.Tests.Converters
{
    public class NestedDictionaryConverterFixture : FixtureBase
    {
        private class DummyDictionary
        {
            public Dictionary<string, object> Prop { get; set; }
        }

        private SystemTextJsonSerializer _serializer;

        public class Read : NestedDictionaryConverterFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Read_As_Dictionary(bool useStrictPropertyName)
            {
                InitializeSerializerAndConverter(useStrictPropertyName);

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

                var prop2Dictionary = useStrictPropertyName
                    ? new Dictionary<string, object> { { "Value", prop2.Value } }
                    : new Dictionary<string, object> { { "value", prop2.Value } };

                var prop3Dictionary = useStrictPropertyName
                    ? new Dictionary<string, object> { { "Value1", prop2Dictionary }, { "Value2", prop1 } }
                    : new Dictionary<string, object> { { "value1", prop2Dictionary }, { "value2", prop1 } };

                var expected = useStrictPropertyName
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

                var value = $@"{{""Prop"":{{""Prop1"":{prop1},""Prop2"":{{""Value"":""{prop2.Value}""}},""Prop3"":{{""Value1"":{{""Value"":""{prop2.Value}""}},""Value2"":{prop1}}}}}}}";

                var actual = _serializer.DeserializeObject<DummyDictionary>(value);

                expected.Should().BeEquivalentTo(actual.Prop);
            }
        }

        public class Write : NestedDictionaryConverterFixture
        {
            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Write_Dictionary(bool useStrictPropertyName)
            {
                InitializeSerializerAndConverter(useStrictPropertyName);

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

                var expected = useStrictPropertyName
                    ? $@"{{""Prop"":{{""Prop1"":{prop1},""Prop2"":{{""Value"":""{prop2.Value}""}},""Prop3"":{{""Value1"":{{""Value"":""{prop2.Value}""}},""Value2"":{prop1}}}}}}}"
                    : $@"{{""Prop"":{{""prop1"":{prop1},""prop2"":{{""value"":""{prop2.Value}""}},""prop3"":{{""value1"":{{""value"":""{prop2.Value}""}},""value2"":{prop1}}}}}}}";

                actual.Should().Be(expected);
            }
        }

        private void InitializeSerializerAndConverter(bool useStrictPropertyName)
        {
            var converterOptions = new NestedDictionaryConverterOptions
            {
                StrictPropertyNames = useStrictPropertyName
            };

            var converter = new NestedDictionaryConverter(converterOptions);

            var options = new JsonSerializerOptions();
            options.Converters.Add(converter);

            _serializer = new SystemTextJsonSerializer(options);
        }
    }
}