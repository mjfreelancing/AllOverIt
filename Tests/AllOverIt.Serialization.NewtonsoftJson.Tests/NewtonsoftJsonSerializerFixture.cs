using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AllOverIt.Fixture;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace AllOverIt.Serialization.NewtonsoftJson.Tests
{
    public class NewtonsoftJsonSerializerFixture : FixtureBase
    {
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

        public class Constructor : NewtonsoftJsonSerializerFixture
        {
            [Fact]
            public void Should_Set_Default_Settings()
            {
                var serializer = new NewtonsoftJsonSerializer(null);

                serializer.Settings
                    .Should()
                    .BeEquivalentTo(new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver
                        {
                            NamingStrategy = new CamelCaseNamingStrategy()
                        }
                    });
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

        public class SerializeObject : NewtonsoftJsonSerializerFixture
        {
            [Fact]
            public void Should_Serialize_Type_With_Default_Settings()
            {
                var value = Create<DummyType>();
                value.Child2 = null;

                var serializer = new NewtonsoftJsonSerializer();

                // Child2 will be included by default
                var actual = serializer.SerializeObject(value);

                var expected = $@"{{""propOne"":{value.PropOne},""prop2"":""{value.Prop2}"",""child1"":{{""prop1"":{value.Child1.Prop1},""prop2"":""{value.Child1.Prop2}""}},""child2"":null}}";

                actual.Should().BeEquivalentTo(expected);
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

                actual.Should().BeEquivalentTo(expected);
            }
        }

        public class SerializeToUtf8Bytes : NewtonsoftJsonSerializerFixture
        {
            [Fact]
            public void Should_Serialize()
            {
                var value = Create<DummyType>();
                var serializer = new NewtonsoftJsonSerializer();

                var actual = serializer.SerializeToUtf8Bytes(value);

                var expected = Encoding.UTF8.GetBytes(serializer.SerializeObject(value));

                actual.Should().BeEquivalentTo(expected);
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

                var serializer = new NewtonsoftJsonSerializer();

                var actual = serializer.DeserializeObject<DummyType>(value);

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Deserialize_With_Custom_Settings()
            {
                var expected = Create<DummyType>();
                expected.Child2 = null;

                // PropOne should be prop_one
                var value = $@"{{""PropOne"":{expected.PropOne},""prop2"":""{expected.Prop2}"",""child1"":{{""prop1"":{expected.Child1.Prop1},""prop2"":""{expected.Child1.Prop2}""}},""child2"":null}}";

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
                    var serializer = new NewtonsoftJsonSerializer();

                    var actual = await serializer.DeserializeObjectAsync<DummyType>(stream, CancellationToken.None);

                    actual.Should().BeEquivalentTo(expected);
                }
            }

            [Fact]
            public async Task Should_Deserialize_With_Custom_Settings()
            {
                var expected = Create<DummyType>();
                expected.Child2 = null;

                // PropOne should be prop_one
                var value = $@"{{""PropOne"":{expected.PropOne},""prop2"":""{expected.Prop2}"",""child1"":{{""prop1"":{expected.Child1.Prop1},""prop2"":""{expected.Child1.Prop2}""}},""child2"":null}}";

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
                    var serializer = new NewtonsoftJsonSerializer();

                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    await Invoking(async () =>
                        {
                            _ = await serializer.DeserializeObjectAsync<DummyType>(stream, cts.Token);
                        })
                        .Should()
                        .ThrowAsync<OperationCanceledException>();
                }
            }
        }
    }
}