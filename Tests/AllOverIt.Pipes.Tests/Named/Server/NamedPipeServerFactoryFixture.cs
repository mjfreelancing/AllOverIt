using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pipes.Named.Serialization;
using AllOverIt.Pipes.Named.Server;
using FakeItEasy;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Pipes.Tests.Named.Server
{
    public class NamedPipeServerFactoryFixture : FixtureBase
    {
        public sealed class DummyMessage
        {
            public int Id { get; set; }
        }

        private readonly NamedPipeServerFactory<DummyMessage> _factory = new(A.Fake<INamedPipeSerializer<DummyMessage>>());

        [Collection("NamedPipes")]
        public class Constructor : NamedPipeServerFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_Serializer_Null()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeServerFactory<DummyMessage>(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("serializer");
            }
        }

        [Collection("NamedPipes")]
        public class CreateNamedPipeServer_PipeName : NamedPipeServerFactoryFixture
        {
            [Fact]
            public void Should_Throw_When_PipeName_Null()
            {
                Invoking(() =>
                {
                    _ = _factory.CreateNamedPipeServer(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("pipeName");
            }

            [Fact]
            public void Should_Throw_When_PipeName_Empty()
            {
                Invoking(() =>
                {
                    _ = _factory.CreateNamedPipeServer(string.Empty);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("pipeName");
            }

            [Fact]
            public void Should_Throw_When_PipeName_Whitespace()
            {
                Invoking(() =>
                {
                    _ = _factory.CreateNamedPipeServer("  ");
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("pipeName");
            }

            [Fact]
            public void Should_Return_Named_Pipe_Server()
            {
                var client = _factory.CreateNamedPipeServer(Create<string>());

                client.Should().BeAssignableTo<INamedPipeServer<DummyMessage>>();
            }

            [Fact]
            public void Should_Return_Named_Pipe_Server_With_PipeName()
            {
                var pipeName = Create<string>();

                var server = _factory.CreateNamedPipeServer(pipeName);

                server.PipeName.Should().Be(pipeName);
            }
        }
    }
}