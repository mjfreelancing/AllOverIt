using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pipes.Named.Client;
using AllOverIt.Pipes.Named.Serialization;
using FakeItEasy;
using FluentAssertions;

namespace AllOverIt.Pipes.Tests.Named.Client
{
    public class NamedPipeClientFactoryFixture : FixtureBase
    {
        public sealed class DummyMessage
        {
            public int Id { get; set; }
        }

        private readonly NamedPipeClientFactory<DummyMessage> _factory = new(A.Fake<INamedPipeSerializer<DummyMessage>>());

        [Collection("NamedPipes")]
        public class Constructor : NamedPipeClientFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Serializer_Null()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeClientFactory<DummyMessage>(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("serializer");
            }
        }

        [Collection("NamedPipes")]
        public class CreateNamedPipeClient_PipeName : NamedPipeClientFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_PipeName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = _factory.CreateNamedPipeClient(stringValue);
                    }, "pipeName");
            }

            [Fact]
            public void Should_Return_Named_Pipe_Client()
            {
                var client = _factory.CreateNamedPipeClient(Create<string>());

                client.Should().BeAssignableTo<INamedPipeClient<DummyMessage>>();
            }

            [Fact]
            public void Should_Return_Named_Pipe_Client_With_Local_Server_And_PipeName()
            {
                var pipeName = Create<string>();

                var client = _factory.CreateNamedPipeClient(pipeName);

                client.ServerName.Should().Be(".");
                client.PipeName.Should().Be(pipeName);
            }
        }

        [Collection("NamedPipes")]
        public class CreateNamedPipeClient_PipeName_ServerName : NamedPipeClientFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_PipeName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = _factory.CreateNamedPipeClient(stringValue, Create<string>());
                    }, "pipeName");
            }

            [Fact]
            public void Should_Throw_When_ServerName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = _factory.CreateNamedPipeClient(Create<string>(), stringValue);
                    }, "serverName");
            }

            [Fact]
            public void Should_Return_Named_Pipe_Client()
            {
                var client = _factory.CreateNamedPipeClient(Create<string>(), Create<string>());

                client.Should().BeAssignableTo<INamedPipeClient<DummyMessage>>();
            }

            [Fact]
            public void Should_Return_Named_Pipe_Client_With_ServerName_And_PipeName()
            {
                var pipeName = Create<string>();
                var serverName = Create<string>();

                var client = _factory.CreateNamedPipeClient(pipeName, serverName);

                client.PipeName.Should().Be(pipeName);
                client.ServerName.Should().Be(serverName);
            }
        }
    }
}