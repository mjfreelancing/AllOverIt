using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AllOverIt.Fixture;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Serialization.SystemTextJson.Tests
{
    public class SystemTextJsonSerializerFixture : FixtureBase
    {
        private class DummyChildType
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
        }

        private class DummyType
        {
            public int Prop1 { get; set; }
            public string Prop2 { get; set; }
            public DummyChildType Child1 { get; set; }
            public DummyChildType Child2 { get; set; }
        }

        public class Constructor : SystemTextJsonSerializerFixture
        {
            [Fact]
            public void Should_Set_Default_Options()
            {
                var serializer = new SystemTextJsonSerializer(null);

                serializer.Options
                    .Should()
                    .BeEquivalentTo(new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    });
            }

            [Fact]
            public void Should_Set_Custom_Options()
            {
                var options = new JsonSerializerOptions();
                var serializer = new SystemTextJsonSerializer(options);

                serializer.Options
                    .Should()
                    .Be(options);
            }
        }

        public class SerializeObject : SystemTextJsonSerializerFixture
        {
            [Fact]
            public void Should_Serialize_Type_With_Default_Options()
            {
                var value = Create<DummyType>();
                value.Child2 = null;

                var serializer = new SystemTextJsonSerializer();

                // Child2 will be included by default
                var actual = serializer.SerializeObject(value);

                var expected = $@"{{""prop1"":{value.Prop1},""prop2"":""{value.Prop2}"",""child1"":{{""prop1"":{value.Child1.Prop1},""prop2"":""{value.Child1.Prop2}""}},""child2"":null}}";
                
                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Serialize_Type_With_Custom_Options()
            {
                var value = Create<DummyType>();
                value.Child1 = null;
                value.Child2 = null;

                var settings = new JsonSerializerOptions
                {
                    IgnoreNullValues = true
                };

                var serializer = new SystemTextJsonSerializer(settings);

                var actual = serializer.SerializeObject(value);

                var expected = $@"{{""prop1"":{value.Prop1},""prop2"":""{value.Prop2}""}}";

                actual.Should().BeEquivalentTo(expected);
            }
        }

        public class SerializeToUtf8Bytes : SystemTextJsonSerializerFixture
        {
            [Fact]
            public void Should_Serialize()
            {
                var value = Create<DummyType>();
                var serializer = new SystemTextJsonSerializer();

                var actual = serializer.SerializeToUtf8Bytes(value);

                var expected = Encoding.UTF8.GetBytes(serializer.SerializeObject(value));

                actual.Should().BeEquivalentTo(expected);
            }
        }

        public class DeserializeObject : SystemTextJsonSerializerFixture
        {
            [Fact]
            public void Should_Deserialize()
            {
                var expected = Create<DummyType>();
                expected.Child2 = null;

                var value = $@"{{""prop1"":{expected.Prop1},""prop2"":""{expected.Prop2}"",""child1"":{{""prop1"":{expected.Child1.Prop1},""prop2"":""{expected.Child1.Prop2}""}},""child2"":null}}";

                var serializer = new SystemTextJsonSerializer();

                var actual = serializer.DeserializeObject<DummyType>(value);

                actual.Should().BeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Deserialize_With_Custom_Options()
            {
                var expected = Create<DummyType>();
                expected.Child2 = null;

                // Prop1 should be prop1
                var value = $@"{{""Prop1"":{expected.Prop1},""prop2"":""{expected.Prop2}"",""child1"":{{""prop1"":{expected.Child1.Prop1},""prop2"":""{expected.Child1.Prop2}""}},""child2"":null}}";

                var serializer = new SystemTextJsonSerializer
                {
                    Options =
                    {
                        PropertyNameCaseInsensitive = false
                    }
                };

                var actual = serializer.DeserializeObject<DummyType>(value);

                actual.Prop1.Should().Be(0);
                actual.Prop1.Should().NotBe(expected.Prop1);
            }
        }

        public class DeserializeObjectAsync : SystemTextJsonSerializerFixture
        {
            [Fact]
            public async Task Should_Deserialize()
            {
                var expected = Create<DummyType>();
                expected.Child2 = null;

                var value = $@"{{""prop1"":{expected.Prop1},""prop2"":""{expected.Prop2}"",""child1"":{{""prop1"":{expected.Child1.Prop1},""prop2"":""{expected.Child1.Prop2}""}},""child2"":null}}";

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
                {
                    var serializer = new SystemTextJsonSerializer();

                    var actual = await serializer.DeserializeObjectAsync<DummyType>(stream, CancellationToken.None);

                    actual.Should().BeEquivalentTo(expected);
                }
            }

            [Fact]
            public async Task Should_Deserialize_With_Custom_Options()
            {
                var expected = Create<DummyType>();
                expected.Child2 = null;

                // Prop1 should be prop1
                var value = $@"{{""Prop1"":{expected.Prop1},""prop2"":""{expected.Prop2}"",""child1"":{{""prop1"":{expected.Child1.Prop1},""prop2"":""{expected.Child1.Prop2}""}},""child2"":null}}";

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
                {
                    var serializer = new SystemTextJsonSerializer
                    {
                        Options =
                        {
                            PropertyNameCaseInsensitive = false
                        }
                    };

                    var actual = await serializer.DeserializeObjectAsync<DummyType>(stream, CancellationToken.None);

                    actual.Prop1.Should().Be(0);
                    actual.Prop1.Should().NotBe(expected.Prop1);
                }
            }

            [Fact]
            public async Task Should_Cancel_Deserialize()
            {
                var expected = Create<DummyType>();
                expected.Child2 = null;

                var value = $@"{{""propOne"":{expected.Prop1},""prop2"":""{expected.Prop2}"",""child1"":{{""prop1"":{expected.Child1.Prop1},""prop2"":""{expected.Child1.Prop2}""}},""child2"":null}}";

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
                {
                    var serializer = new SystemTextJsonSerializer();

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
