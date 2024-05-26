using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pipes.Named.Client;
using AllOverIt.Pipes.Named.Serialization;
using FakeItEasy;
using FluentAssertions;
using System.IO.Pipes;

namespace AllOverIt.Pipes.Tests.Named.Client
{
    public class NamedPipeClientConnectionFixture : FixtureBase
    {
        public sealed class DummyMessage
        {
            public int Id { get; set; }
        }

        public sealed class DummyStream : PipeStream
        {
            public DummyStream()
                : base(PipeDirection.InOut, 1024)
            {
            }
        }

        [Collection("NamedPipes")]
        public class Constructor : NamedPipeClientConnectionFixture
        {
            [Fact]
            public void Should_Throw_When_PipeStream_Null()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeClientConnection<DummyMessage>(null, Create<string>(), Create<string>(), A.Fake<INamedPipeSerializer<DummyMessage>>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("pipeStream");
            }

            [Fact]
            public void Should_Throw_When_ConnectionId_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = new NamedPipeClientConnection<DummyMessage>(new DummyStream(), stringValue, Create<string>(), A.Fake<INamedPipeSerializer<DummyMessage>>());
                    }, "connectionId");
            }

            [Fact]
            public void Should_Throw_When_ServerName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        _ = new NamedPipeClientConnection<DummyMessage>(new DummyStream(), Create<string>(), stringValue, A.Fake<INamedPipeSerializer<DummyMessage>>());
                    }, "serverName");
            }

            [Fact]
            public void Should_Throw_When_Serializer_Null()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeClientConnection<DummyMessage>(new DummyStream(), Create<string>(), Create<string>(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("serializer");
            }
        }

        [Collection("NamedPipes")]
        public class ServerName : NamedPipeClientConnectionFixture
        {
            [Fact]
            public void Should_Set_ServerName()
            {
                var expected = Create<string>();

                var connection = new NamedPipeClientConnection<DummyMessage>(new DummyStream(), Create<string>(), expected, A.Fake<INamedPipeSerializer<DummyMessage>>());

                connection.ServerName.Should().Be(expected);
            }
        }
    }
}