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

        private readonly SystemTextJsonSerializer _serializer;

        protected NestedDictionaryConverterFixture()
        {
            var converter = new NestedDictionaryConverter();

            var options = new JsonSerializerOptions();
            options.Converters.Add(converter);

            _serializer = new SystemTextJsonSerializer(options);
        }

        public class Read : NestedDictionaryConverterFixture
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
                var prop3Dictionary = new Dictionary<string, object> {{"Value1", prop2Dictionary}, {"Value2", prop1}};

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

            public class Write : NestedDictionaryConverterFixture
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