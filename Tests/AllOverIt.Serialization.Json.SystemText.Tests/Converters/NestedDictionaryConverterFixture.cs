using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Serialization.Json.SystemText.Converters;
using FluentAssertions;
using System.Text.Json;

namespace AllOverIt.Serialization.Json.SystemText.Tests.Converters
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
                    DayOfWeek = Create<DayOfWeek>(),
                    Numbers = CreateMany<int>().ToArray()
                };

                var prop2Dictionary = includeUppercase
                    ? new Dictionary<string, object> { { "Value", prop2.Value }, { "DayOfWeek", prop2.DayOfWeek }, { "Numbers", prop2.Numbers } }
                    : new Dictionary<string, object> { { "value", prop2.Value }, { "dayOfWeek", prop2.DayOfWeek }, { "numbers", prop2.Numbers } };

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
                    ? $@"{{""Prop"":{{""Prop1"":{prop1},""Prop2"":{{""Value"":""{prop2.Value}"",""DayOfWeek"":""{prop2.DayOfWeek}"",""Numbers"":[{string.Join(',', prop2.Numbers)}]}},""Prop3"":{{""Value1"":{{""Value"":""{prop2.Value}"",""DayOfWeek"":""{prop2.DayOfWeek}"",""Numbers"":[{string.Join(',', prop2.Numbers)}]}},""Value2"":{prop1}}}}}}}"
                    : $@"{{""prop"":{{""prop1"":{prop1},""prop2"":{{""value"":""{prop2.Value}"",""dayOfWeek"":""{prop2.DayOfWeek}"",""numbers"":[{string.Join(',', prop2.Numbers)}]}},""prop3"":{{""value1"":{{""value"":""{prop2.Value}"",""dayOfWeek"":""{prop2.DayOfWeek}"",""numbers"":[{string.Join(',', prop2.Numbers)}]}},""value2"":{prop1}}}}}}}";

                actual.Should().Be(expected);
            }
        }
        public class Read_Write : NestedDictionaryConverterFixture
        {
            private class DummyModel
            {
                public string Prop1 { get; set; }
                public DateTime Prop2 { get; set; }
                public long Prop3 { get; set; }
                public int Prop4 { get; set; }
                public float Prop5 { get; set; }
                public double Prop6 { get; set; }
                public decimal Prop7 { get; set; }
                public bool Prop8 { get; set; }
                public DayOfWeek Prop9 { get; set; }
                public int? Prop10 { get; set; }
            }

            private class DummyNested
            {
                public string Prop1 { get; set; }
                public DateTime Prop2 { get; set; }
                public long Prop3 { get; set; }
                public int Prop4 { get; set; }
                public float Prop5 { get; set; }
                public double Prop6 { get; set; }
                public decimal Prop7 { get; set; }
                public bool Prop8 { get; set; }
                public DayOfWeek Prop9 { get; set; }
                public int? Prop10 { get; set; }
                public IDictionary<string, object> Prop11 { get; set; }
            }

            [Fact]
            public void Should_Read_Write()         // Testing more code paths for multiple property types
            {
                InitializeSerializerAndConverter();

                // Using this to simplify the creation of a Dictionary<string, object>
                var model = new DummyModel
                {
                    Prop1 = "",
                    Prop2 = DateTime.Now,
                    Prop3 = Create<long>(),
                    Prop4 = Create<int>(),
                    Prop5 = Create<float>(),
                    Prop6 = Create<double>(),
                    Prop7 = Create<decimal>(),
                    Prop8 = Create<bool>(),
                    Prop9 = Create<DayOfWeek>(),
                    Prop10 = null
                };

                var nested = new DummyNested
                {
                    Prop1 = Create<string>(),
                    Prop2 = DateTime.Now.AddDays(1),
                    Prop3 = Create<long>(),
                    Prop4 = Create<int>(),
                    Prop5 = Create<float>(),
                    Prop6 = Create<double>(),
                    Prop7 = Create<decimal>(),
                    Prop8 = Create<bool>(),
                    Prop9 = Create<DayOfWeek>(),
                    Prop10 = null,
                    Prop11 = model.ToPropertyDictionary(true)
                };

                var dictionary = nested.ToPropertyDictionary(true);

                // enums are written as numbers in ToPropertyDictionary() - change them to a string
                dictionary[nameof(DummyNested.Prop9)] = nested.Prop9.ToString();

                var serialized = _serializer.SerializeObject(new
                {
                    Prop = dictionary
                });

                var deserialized = _serializer.DeserializeObject<DummyDictionary>(serialized).Prop;

                // The serializaiton and deserialization process is not storing type information so there are some
                // differences that make comparing 'deserialized' with 'dictionary' difficult. Examples include
                // float vs double vs decimal as well as how enum's are handled (ToPropertyDictionary() uses numbers
                // whereas serialization uses strings).
                //
                // To make the testing easier, the values in deserialized are asssigned to propertis on a concrete type
                // and then the test compares this constructed object with the original 'nested' object.
                var actual = new DummyNested();

                actual.Prop1 = ObjectExtensions.As<string>(deserialized[nameof(DummyNested.Prop1)]);
                actual.Prop2 = ObjectExtensions.As<DateTime>(deserialized[nameof(DummyNested.Prop2)]);
                actual.Prop3 = ObjectExtensions.As<long>(deserialized[nameof(DummyNested.Prop3)]);
                actual.Prop4 = ObjectExtensions.As<int>(deserialized[nameof(DummyNested.Prop4)]);
                actual.Prop5 = ObjectExtensions.As<float>(deserialized[nameof(DummyNested.Prop5)]);
                actual.Prop6 = ObjectExtensions.As<double>(deserialized[nameof(DummyNested.Prop6)]);
                actual.Prop7 = ObjectExtensions.As<decimal>(deserialized[nameof(DummyNested.Prop7)]);
                actual.Prop8 = ObjectExtensions.As<bool>(deserialized[nameof(DummyNested.Prop8)]);
                actual.Prop9 = ObjectExtensions.As<DayOfWeek>(deserialized[nameof(DummyNested.Prop9)]);
                actual.Prop10 = deserialized[nameof(DummyNested.Prop10)].AsNullable<int>();

                var deserializedProp11 = (IDictionary<string, object>) deserialized[nameof(DummyNested.Prop11)];

                actual.Prop11 = new Dictionary<string, object>()
                {
                    { nameof(DummyModel.Prop1), ObjectExtensions.As<string>(deserializedProp11[nameof(DummyModel.Prop1)]) },
                    { nameof(DummyModel.Prop2), ObjectExtensions.As<DateTime>(deserializedProp11[nameof(DummyModel.Prop2)]) },
                    { nameof(DummyModel.Prop3), ObjectExtensions.As<long>(deserializedProp11[nameof(DummyModel.Prop3)]) },
                    { nameof(DummyModel.Prop4), ObjectExtensions.As<int>(deserializedProp11[nameof(DummyModel.Prop4)]) },
                    { nameof(DummyModel.Prop5), ObjectExtensions.As<float>(deserializedProp11[nameof(DummyModel.Prop5)]) },
                    { nameof(DummyModel.Prop6), ObjectExtensions.As<double>(deserializedProp11[nameof(DummyModel.Prop6)]) },
                    { nameof(DummyModel.Prop7), ObjectExtensions.As<decimal>(deserializedProp11[nameof(DummyModel.Prop7)]) },
                    { nameof(DummyModel.Prop8), ObjectExtensions.As<bool>(deserializedProp11[nameof(DummyModel.Prop8)]) },
                    { nameof(DummyModel.Prop9), ObjectExtensions.As<DayOfWeek>(deserializedProp11[nameof(DummyModel.Prop9)]) },
                    { nameof(DummyModel.Prop10), deserializedProp11[nameof(DummyModel.Prop10)].AsNullable<int>() },
                };

                actual.Should().BeEquivalentTo(nested, options =>
                {
                    options.Using<float>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 0.00000001f)).WhenTypeIs<float>();
                    options.Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 0.00000001d)).WhenTypeIs<double>();
                    options.Using<decimal>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 0.00000001m)).WhenTypeIs<decimal>();

                    return options;
                });
            }
        }

        private void InitializeSerializerAndConverter()
        {
            var converterOptions = new NestedDictionaryConverterOptions
            {
                ReadFloatingAsDecimals = true
            };

            var converter = new NestedDictionaryConverter(converterOptions);

            var options = new JsonSerializerOptions();
            options.Converters.Add(converter);

            _serializer = new SystemTextJsonSerializer(options);
        }
    }
}