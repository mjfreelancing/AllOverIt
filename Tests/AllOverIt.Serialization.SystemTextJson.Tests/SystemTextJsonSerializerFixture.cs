using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Serialization.Abstractions;
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

        private readonly SystemTextJsonSerializer _serializer;

        protected SystemTextJsonSerializerFixture()
        {
            _serializer = new SystemTextJsonSerializer();
        }

        public class Constructor : SystemTextJsonSerializerFixture
        {
            [Fact]
            public void Should_Set_Default_Options()
            {
                var serializer = new SystemTextJsonSerializer(null);

                serializer.Options
                    .Should()
                    .BeEquivalentTo(new JsonSerializerOptions());
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

        public class Configure : SystemTextJsonSerializerFixture
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
                    _serializer.Options
                        .PropertyNamingPolicy
                        .Should()
                        .BeSameAs(JsonNamingPolicy.CamelCase);
                }
                else
                {
                    _serializer.Options.PropertyNamingPolicy.Should().BeNull();
                }
            }

            [Theory]
            [InlineData(false)]
            [InlineData(true)]
            public void Should_Apply_CaseSensitive(bool useCaseSensitive)
            {
                var config = new JsonSerializerConfiguration
                {
                    CaseSensitive = useCaseSensitive
                };

                _serializer.Configure(config);

                _serializer.Options.PropertyNameCaseInsensitive
                    .Should()
                    .Be(!useCaseSensitive);
            }
        }

        public class SerializeObject : SystemTextJsonSerializerFixture
        {
            [Fact]
            public void Should_Serialize_Type_With_Default_Options()
            {
                var value = Create<DummyType>();
                value.Child2 = null;

                // Child2 will be included by default
                var actual = _serializer.SerializeObject(value);

                var expected = $@"{{""prop1"":{value.Prop1},""prop2"":""{value.Prop2}"",""child1"":{{""prop1"":{value.Child1.Prop1},""prop2"":""{value.Child1.Prop2}""}},""child2"":null}}";

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Serialize_Type_With_Custom_Options()
            {
                var value = Create<DummyType>();
                value.Child1 = null;
                value.Child2 = null;

                var settings = new JsonSerializerOptions
                {
                    // IgnoreNullValues = true (now obsolete)
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var serializer = new SystemTextJsonSerializer(settings);

                var actual = serializer.SerializeObject(value);

                var expected = $@"{{""prop1"":{value.Prop1},""prop2"":""{value.Prop2}""}}";

                expected.Should().BeEquivalentTo(actual);
            }
        }

        public class SerializeToUtf8Bytes : SystemTextJsonSerializerFixture
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

        public class DeserializeObject : SystemTextJsonSerializerFixture
        {
            [Fact]
            public void Should_Deserialize()
            {
                var expected = Create<DummyType>();
                expected.Child2 = null;

                var value = $@"{{""Prop1"":{expected.Prop1},""Prop2"":""{expected.Prop2}"",""Child1"":{{""Prop1"":{expected.Child1.Prop1},""Prop2"":""{expected.Child1.Prop2}""}},""Child2"":null}}";
                
                var actual = _serializer.DeserializeObject<DummyType>(value);

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Deserialize_With_Custom_Options()
            {
                var expected = Create<DummyType>();
                expected.Child2 = null;

                // prop1 should be Prop1
                var value = $@"{{""prop1"":{expected.Prop1},""Prop2"":""{expected.Prop2}"",""Child1"":{{""Prop1"":{expected.Child1.Prop1},""Prop2"":""{expected.Child1.Prop2}""}},""Child2"":null}}";

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

                var value = $@"{{""Prop1"":{expected.Prop1},""Prop2"":""{expected.Prop2}"",""Child1"":{{""Prop1"":{expected.Child1.Prop1},""Prop2"":""{expected.Child1.Prop2}""}},""Child2"":null}}";

                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(value)))
                {
                    var actual = await _serializer.DeserializeObjectAsync<DummyType>(stream, CancellationToken.None);

                    expected.Should().BeEquivalentTo(actual);
                }
            }

            [Fact]
            public async Task Should_Deserialize_With_Custom_Options()
            {
                var expected = Create<DummyType>();
                expected.Child2 = null;

                // prop1 should be Prop1
                var value = $@"{{""prop1"":{expected.Prop1},""Prop2"":""{expected.Prop2}"",""Child1"":{{""Prop1"":{expected.Child1.Prop1},""Prop2"":""{expected.Child1.Prop2}""}},""Child2"":null}}";

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

                var value = $@"{{""PropOne"":{expected.Prop1},""Prop2"":""{expected.Prop2}"",""Child1"":{{""Prop1"":{expected.Child1.Prop1},""Prop2"":""{expected.Child1.Prop2}""}},""Child2"":null}}";

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
