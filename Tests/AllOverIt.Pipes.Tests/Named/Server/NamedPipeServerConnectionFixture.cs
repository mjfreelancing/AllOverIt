using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pipes.Exceptions;
using AllOverIt.Pipes.Named.Serialization;
using AllOverIt.Pipes.Named.Server;
using FakeItEasy;
using FluentAssertions;
using System;
using System.IO.Pipes;
using Xunit;

namespace AllOverIt.Pipes.Tests.Named.Server
{
    public class NamedPipeServerConnectionFixture : FixtureBase
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

        [Collection("Pipes")]
        public class Constructor : NamedPipeServerConnectionFixture
        {
            [Fact]
            public void Should_Throw_When_PipeStream_Null()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeServerConnection<DummyMessage>(null, Create<string>(), A.Fake<INamedPipeSerializer<DummyMessage>>());
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
                    _ = new NamedPipeServerConnection<DummyMessage>(new DummyStream(), null, A.Fake<INamedPipeSerializer<DummyMessage>>());
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
                    _ = new NamedPipeServerConnection<DummyMessage>(new DummyStream(), string.Empty, A.Fake<INamedPipeSerializer<DummyMessage>>());
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
                    _ = new NamedPipeServerConnection<DummyMessage>(new DummyStream(), "  ", A.Fake<INamedPipeSerializer<DummyMessage>>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("connectionId");
            }

            [Fact]
            public void Should_Throw_When_Serializer_Null()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeServerConnection<DummyMessage>(new DummyStream(), Create<string>(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("serializer");
            }
        }

        [Collection("Pipes")]
        public class GetImpersonationUserName : NamedPipeServerConnectionFixture
        {
            [Fact]
            public void Should_Throw_When_Not_NamedPipeServerStream()
            {
                var connection = new NamedPipeServerConnection<DummyMessage>(new DummyStream(), Create<string>(), A.Fake<INamedPipeSerializer<DummyMessage>>());

                Invoking(() =>
                {
                    connection.GetImpersonationUserName();
                })
               .Should()
               .Throw<PipeException>()
               .WithMessage($"The pipe stream must be a {nameof(NamedPipeServerStream)}.");
            }
        }
    }
}