using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pipes.Named.Client;
using AllOverIt.Pipes.Named.Serialization;
using FakeItEasy;
using FluentAssertions;
using System;
using System.IO.Pipes;
using Xunit;

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
            public void Should_Throw_When_ConnectionId_Null()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeClientConnection<DummyMessage>(new DummyStream(), null, Create<string>(), A.Fake<INamedPipeSerializer<DummyMessage>>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("connectionId");
            }

            [Fact]
            public void Should_Throw_When_ConnectionId_Empty()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeClientConnection<DummyMessage>(new DummyStream(), string.Empty, Create<string>(), A.Fake<INamedPipeSerializer<DummyMessage>>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("connectionId");
            }

            [Fact]
            public void Should_Throw_When_ConnectionId_Whitespace()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeClientConnection<DummyMessage>(new DummyStream(), "  ", Create<string>(), A.Fake<INamedPipeSerializer<DummyMessage>>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("connectionId");
            }

            [Fact]
            public void Should_Throw_When_ServerName_Null()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeClientConnection<DummyMessage>(new DummyStream(), Create<string>(), null, A.Fake<INamedPipeSerializer<DummyMessage>>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("serverName");
            }

            [Fact]
            public void Should_Throw_When_ServerName_Empty()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeClientConnection<DummyMessage>(new DummyStream(), Create<string>(), string.Empty, A.Fake<INamedPipeSerializer<DummyMessage>>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("serverName");
            }

            [Fact]
            public void Should_Throw_When_ServerName_Whitespace()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeClientConnection<DummyMessage>(new DummyStream(), Create<string>(), "  ", A.Fake<INamedPipeSerializer<DummyMessage>>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("serverName");
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