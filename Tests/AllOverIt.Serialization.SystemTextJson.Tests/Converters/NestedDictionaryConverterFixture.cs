using AllOverIt.Fixture;
using AllOverIt.Serialization.SystemTextJson.Converters;
using FluentAssertions;
using System;
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
            public void Should_Read_As_Dictionary(bool includeUppercase)
            {
                InitializeSerializerAndConverter();

                var prop1 = Create<int>();

                var prop2 = new
                {
                    Value = Create<string>(),
                    DayOfWeek = $"{Create<DayOfWeek>()}"            // there's no strong typing so this cannot be deserialized as an enum
                };

                //var prop3 = new
                //{
                //    Value1 = prop2,
                //    Value2 = prop1
                //};

                var prop2Dictionary = includeUppercase
                    ? new Dictionary<string, object> { { "Value", prop2.Value }, { "DayOfWeek", prop2.DayOfWeek } }
                    : new Dictionary<string, object> { { "value", prop2.Value }, { "dayOfWeek", prop2.DayOfWeek } };

                var prop3Dictionary = includeUppercase
                    ? new Dictionary<string, object> { { "Value1", prop2Dictionary }, { "Value2", prop1 } }
                    : new Dictionary<string, object> { { "value1", prop2Dictionary }, { "value2", prop1 } };

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
                    ? $@"{{""Prop"":{{""Prop1"":{prop1},""Prop2"":{{""Value"":""{prop2.Value}"",""DayOfWeek"":""{prop2.DayOfWeek}""}},""Prop3"":{{""Value1"":{{""Value"":""{prop2.Value}"",""DayOfWeek"":""{prop2.DayOfWeek}""}},""Value2"":{prop1}}}}}}}"
                    : $@"{{""Prop"":{{""prop1"":{prop1},""prop2"":{{""value"":""{prop2.Value}"",""dayOfWeek"":""{prop2.DayOfWeek}""}},""prop3"":{{""value1"":{{""value"":""{prop2.Value}"",""dayOfWeek"":""{prop2.DayOfWeek}""}},""value2"":{prop1}}}}}}}";

                var actual = _serializer.DeserializeObject<DummyDictionary>(value);

                expected.Should().BeEquivalentTo(actual.Prop);
            }
        }

        public class Write : NestedDictionaryConverterFixture
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
                    Value = Create<string>(),
                    DayOfWeek = Create<DayOfWeek>()
                };

                //var prop3 = new
                //{
                //    Value1 = prop2,
                //    Value2 = prop1
                //};

                var prop2Dictionary = includeUppercase
                    ? new Dictionary<string, object> { { "Value", prop2.Value }, { "DayOfWeek", prop2.DayOfWeek } }
                    : new Dictionary<string, object> { { "value", prop2.Value }, { "dayOfWeek", prop2.DayOfWeek } };

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
                    ? $@"{{""Prop"":{{""Prop1"":{prop1},""Prop2"":{{""Value"":""{prop2.Value}"",""DayOfWeek"":""{prop2.DayOfWeek}""}},""Prop3"":{{""Value1"":{{""Value"":""{prop2.Value}"",""DayOfWeek"":""{prop2.DayOfWeek}""}},""Value2"":{prop1}}}}}}}"
                    : $@"{{""prop"":{{""prop1"":{prop1},""prop2"":{{""value"":""{prop2.Value}"",""dayOfWeek"":""{prop2.DayOfWeek}""}},""prop3"":{{""value1"":{{""value"":""{prop2.Value}"",""dayOfWeek"":""{prop2.DayOfWeek}""}},""value2"":{prop1}}}}}}}";

                actual.Should().Be(expected);
            }
        }

        private void InitializeSerializerAndConverter()
        {
            var converter = new NestedDictionaryConverter();

            var options = new JsonSerializerOptions();
            options.Converters.Add(converter);

            _serializer = new SystemTextJsonSerializer(options);
        }
    }
}