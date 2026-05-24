using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Patterns.Enumeration;
using AllOverIt.Serialization.Json.Abstractions;
using AllOverIt.Serialization.Json.Abstractions.Exceptions;
using AllOverIt.Serialization.Json.Newtonsoft.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Runtime.CompilerServices;
using System.Text;
using AllOverIt.Shouldly.Extensions;

namespace AllOverIt.Serialization.Json.Newtonsoft.Tests
{
    public class NewtonsoftJsonSerializerFixture : FixtureBase
    {
        private readonly NewtonsoftJsonSerializer _serializer;

        private class DummyChildType
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
        }

        private class DummyType
        {
            public int PropOne { get; set; }
            public string Prop2 { get; set; }
            public DummyChildType Child1 { get; set; }
            public DummyChildType Child2 { get; set; }
        }

        protected NewtonsoftJsonSerializerFixture()
        {
            _serializer = new NewtonsoftJsonSerializer();
        }

        public class Constructor : NewtonsoftJsonSerializerFixture
        {
            [Fact]
            public void Should_Set_Default_Settings()
            {
                var serializer = new NewtonsoftJsonSerializer(null);

                var expected = new JsonSerializerSettings();

                expected
                    .ShouldBeEquivalentTo(serializer.Settings);
            }

            [Fact]
            public void Should_Set_Custom_Settings()
            {
                var settings = new JsonSerializerSettings();
                var serializer = new NewtonsoftJsonSerializer(settings);

                serializer.Settings
                    .ShouldBeSameAs(settings);
            }
        }

        public class Configure : NewtonsoftJsonSerializerFixture
        {
            [Fact]
            public void Should_Throw_When_Configuration_Null()
            {
                Invoking(() =>
                    {
                        _serializer.Configure(null);
                    })
                    .ShouldThrow<ArgumentNullException>()
                    .WithNamedMessageWhenNull("configuration");
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Apply_CamelCase(bool useCamelCase)
            {
                var config = new JsonSerializerConfiguration
                {
                    UseCamelCase = useCamelCase
                };

                _serializer.Configure(config);

                if (useCamelCase)
                {
                    _serializer.Settings.ContractResolver
                        .ShouldBeOfType<CamelCasePropertyNamesContractResolver>();
                }
                else
                {
                    _serializer.Settings.ContractResolver.ShouldBeNull();
                }
            }

            [Fact]
            public void Should_Throw_When_Apply_CaseSensitive()
            {
                Invoking(() =>
                    {
                        var config = new JsonSerializerConfiguration
                        {
                            CaseSensitive = true
                        };

                        _serializer.Configure(config);
                    })
                    .ShouldThrow<SerializerConfigurationException>()
                    .WithMessage("Newtonsoft requires a custom converter to support case sensitivity.");
            }

            [Fact]
            public void Should_Add_Support_Enriched_Enums()
            {
                _serializer.Configure(new JsonSerializerConfiguration
                {
                    SupportEnrichedEnums = true
                });

                var hasFactory = _serializer.Settings.Converters.SingleOrDefault(converter => converter.GetType() == typeof(EnrichedEnumJsonConverterFactory));

                hasFactory.ShouldNotBeNull();
            }

            [Fact]
            public void Should_Remove_Support_Enriched_Enums()
            {
                var serializer = new NewtonsoftJsonSerializer();

                serializer.Configure(new JsonSerializerConfiguration
                {
                    SupportEnrichedEnums = true
                });

                var hasFactory = serializer.Settings.Converters.SingleOrDefault(converter => converter.GetType() == typeof(EnrichedEnumJsonConverterFactory));

                hasFactory.ShouldNotBeNull();

                serializer.Configure(new JsonSerializerConfiguration
                {
                    SupportEnrichedEnums = false
                });

                hasFactory = serializer.Settings.Converters.SingleOrDefault(converter => converter.GetType() == typeof(EnrichedEnumJsonConverterFactory));

                hasFactory.ShouldBeNull();
            }
        }

        public class SerializeObject : NewtonsoftJsonSerializerFixture
        {
            [Fact]
            public void Should_Serialize_Type_With_Default_Settings()
            {
                var value = Create<DummyType>();
                value.Child2 = null;

                // Child2 will be included by default
                var actual = _serializer.SerializeObject(value);

                var expected = $@"{{""propOne"":{value.PropOne},""prop2"":""{value.Prop2}"",""child1"":{{""prop1"":{value.Child1.Prop1},""prop2"":""{value.Child1.Prop2}""}},""child2"":null}}";

                actual.ShouldBeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Serialize_Type_With_Custom_Settings()
            {
                var value = Create<DummyType>();
                value.Child1 = null;
                value.Child2 = null;

                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                var serializer = new NewtonsoftJsonSerializer(settings);

                var actual = serializer.SerializeObject(value);

                var expected = $@"{{""propOne"":{value.PropOne},""prop2"":""{value.Prop2}""}}";

                actual.ShouldBeEquivalentTo(expected);
            }
        }

        public class SerializeToUtf8Bytes : NewtonsoftJsonSerializerFixture
        {
            [Fact]
            public void Should_Serialize()
            {
                var value = Create<DummyType>();

                var actual = _serializer.SerializeToUtf8Bytes(value);

                var expected = Encoding.UTF8.GetBytes(_serializer.SerializeObject(value));

                actual.ShouldBeEquivalentTo(expected);
            }
        }

        public class DeserializeObject : NewtonsoftJsonSerializerFixture
        {
            [Fact]
            public void Should_Deserialize()
            {
                var expected = Create<DummyType>();
                expected.Child2 = null;

                var value = $@"{{""propOne"":{expected.PropOne},""prop2"":""{expected.Prop2}"",""child1"":{{""prop1"":{expected.Child1.Prop1},""prop2"":""{expected.Child1.Prop2}""}},""child2"":null}}";

                var actual = _serializer.DeserializeObject<DummyType>(value);

                actual.ShouldBeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Deserialize_With_Custom_Settings()
            {
                var expected = Create<DummyType>();
                expected.Child2 = null;

                // PropOne should be prop_one
                var value = $@"{{""PropOne"":{expected.PropOne},""prop2"":""{expected.Prop2}"",""child1"":{{""prop1"":{expected.Child1.Prop1},""prop2"":""{expected.Child1.Prop2}""}},""child2"":null}}";

                var serializer = new NewtonsoftJsonSerializer()
                {
                    Settings =
                    {
                        ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new SnakeCaseNamingStrategy()
                        }
                    }
                };

                var actual = serializer.DeserializeObject<DummyType>(value);

                actual.PropOne.ShouldBe(0);
                actual.PropOne.ShouldNotBe(expected.PropOne);
            }
        }

        public class DeserializeObjectAsync : NewtonsoftJsonSerializerFixture
        {
            [Fact]
            public async Task Should_Deserialize()
            {
                var expected = Create<DummyType>();
                expected.Child2 = null;

                var value = $@"{{""propOne"":{expected.PropOne},""prop2"":""{expected.Prop2}"",""child1"":{{""prop1"":{expected.Child1.Prop1},""prop2"":""{expected.Child1.Prop2}""}},""child2"":null}}";

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
                {
                    var actual = await _serializer.DeserializeObjectAsync<DummyType>(stream, CancellationToken.None);

                    actual.ShouldBeEquivalentTo(expected);
                }
            }

            [Fact]
            public async Task Should_Deserialize_With_Custom_Settings()
            {
                var expected = Create<DummyType>();
                expected.Child2 = null;

                // PropOne should be Prop_One
                var value = $@"{{""PropOne"":{expected.PropOne},""Prop2"":""{expected.Prop2}"",""Child1"":{{""Prop1"":{expected.Child1.Prop1},""Prop2"":""{expected.Child1.Prop2}""}},""Child2"":null}}";

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
                {
                    var serializer = new NewtonsoftJsonSerializer
                    {
                        Settings =
                        {
                            ContractResolver = new DefaultContractResolver
                            {
                                NamingStrategy = new SnakeCaseNamingStrategy()
                            }
                        }
                    };

                    var actual = await serializer.DeserializeObjectAsync<DummyType>(stream, CancellationToken.None);

                    actual.PropOne.ShouldBe(0);
                    actual.PropOne.ShouldNotBe(expected.PropOne);
                }
            }

            [Fact]
            public async Task Should_Cancel_Deserialize()
            {
                var expected = Create<DummyType>();
                expected.Child2 = null;

                var value = $@"{{""propOne"":{expected.PropOne},""prop2"":""{expected.Prop2}"",""child1"":{{""prop1"":{expected.Child1.Prop1},""prop2"":""{expected.Child1.Prop2}""}},""child2"":null}}";

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
                {
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    await Invoking(async () =>
                        {
                            _ = await _serializer.DeserializeObjectAsync<DummyType>(stream, cts.Token);
                        })
                        .ShouldThrowAsync<OperationCanceledException>();
                }
            }
        }

        public class Serialize_Deserialize : NewtonsoftJsonSerializerFixture
        {
            private class DummyEnrichedEnum : EnrichedEnum<DummyEnrichedEnum>
            {
                public static readonly DummyEnrichedEnum Value1 = new(1);
                public static readonly DummyEnrichedEnum Value2 = new(2, "Value 2");
                public static readonly DummyEnrichedEnum Value3 = new(3);

                private DummyEnrichedEnum(int value, [CallerMemberName] string name = "")
                    : base(value, name)
                {
                }
            }

            private class DummyWithEnum
            {
                public DummyEnrichedEnum Prop1 { get; init; }
                public DummyEnrichedEnum Prop2 { get; init; }
                public DummyEnrichedEnum Prop3 { get; init; }
            }

            [Fact]
            public void Should_Serialize_Deserialize_EnrichedEnum()
            {
                var value = new DummyWithEnum
                {
                    Prop1 = DummyEnrichedEnum.Value1,
                    Prop2 = DummyEnrichedEnum.Value2,
                    Prop3 = DummyEnrichedEnum.Value3
                };

                var expected = "{\"Prop1\":\"Value1\",\"Prop2\":\"Value 2\",\"Prop3\":\"Value3\"}";

                var serializer = new NewtonsoftJsonSerializer();

                serializer.Configure(new JsonSerializerConfiguration
                {
                    SupportEnrichedEnums = true
                });

                var actual = serializer.SerializeObject(value);
                actual.ShouldBeEquivalentTo(expected);

                var deserialized = new DummyWithEnum();
                deserialized.Prop1.ShouldBeNull();
                deserialized.Prop2.ShouldBeNull();
                deserialized.Prop3.ShouldBeNull();

                deserialized = serializer.DeserializeObject<DummyWithEnum>(actual);

                deserialized.Prop1.ShouldBeSameAs(DummyEnrichedEnum.Value1);
                deserialized.Prop2.ShouldBeSameAs(DummyEnrichedEnum.Value2);
                deserialized.Prop3.ShouldBeSameAs(DummyEnrichedEnum.Value3);
            }
        }
    }
}


