using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Serialization.Abstractions;
using AllOverIt.Serialization.Abstractions.Exceptions;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Serialization.NewtonsoftJson.Tests
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
                    .Should()
                    .BeEquivalentTo(serializer.Settings);
            }

            [Fact]
            public void Should_Set_Custom_Settings()
            {
                var settings = new JsonSerializerSettings();
                var serializer = new NewtonsoftJsonSerializer(settings);

                serializer.Settings
                    .Should()
                    .Be(settings);
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
                    .Should()
                    .Throw<ArgumentNullException>()
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
                        .Should()
                        .BeEquivalentTo(new CamelCasePropertyNamesContractResolver());
                }
                else
                {
                    _serializer.Settings.ContractResolver.Should().BeNull();
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
                    .Should()
                    .Throw<SerializerConfigurationException>()
                    .WithMessage("Newtonsoft requires a custom converter to support case sensitivity.");
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

                expected.Should().BeEquivalentTo(actual);
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

                expected.Should().BeEquivalentTo(actual);
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

                expected.Should().BeEquivalentTo(actual);
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

                expected.Should().BeEquivalentTo(actual);
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

                actual.PropOne.Should().Be(0);
                actual.PropOne.Should().NotBe(expected.PropOne);
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

                    expected.Should().BeEquivalentTo(actual);
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

                    actual.PropOne.Should().Be(0);
                    actual.PropOne.Should().NotBe(expected.PropOne);
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
                        .Should()
                        .ThrowAsync<OperationCanceledException>();
                }
            }
        }
    }
}