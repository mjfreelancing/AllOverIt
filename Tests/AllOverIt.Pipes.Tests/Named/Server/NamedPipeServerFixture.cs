using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pipes.Named.Connection;
using AllOverIt.Pipes.Named.Serialization;
using AllOverIt.Pipes.Named.Server;
using FakeItEasy;
using FluentAssertions;
using System;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Pipes.Tests.Named.Server
{
    public class NamedPipeServerFixture : FixtureBase
    {
        public sealed class DummyMessage
        {
            public int Id { get; set; }
        }

        [Collection("Pipes")]
        public class Constructor : NamedPipeServerFixture
        {
            [Fact]
            public void Should_Throw_When_PipeName_Null()
            {

                Invoking(() =>
                {
                    _ = new NamedPipeServer<DummyMessage>(null, A.Fake<INamedPipeSerializer<DummyMessage>>());
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
                    _ = new NamedPipeServer<DummyMessage>(string.Empty, A.Fake<INamedPipeSerializer<DummyMessage>>());
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
                    _ = new NamedPipeServer<DummyMessage>("  ", A.Fake<INamedPipeSerializer<DummyMessage>>());
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("pipeName");
            }

            [Fact]
            public void Should_Throw_When_Serializer_Null()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeServer<DummyMessage>(Create<string>(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("serializer");
            }
        }

        [Collection("Pipes")]
        public class Start_Action : NamedPipeServerFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Invoking(() =>
                {
                    var server = new NamedPipeServer<DummyMessage>(Create<string>(), A.Fake<INamedPipeSerializer<DummyMessage>>());

                    server.Start((Action<PipeSecurity>) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("pipeSecurity");
            }

            [Fact]
            public void Should_Configure_Security()
            {
                var server = new NamedPipeServer<DummyMessage>(Create<string>(), A.Fake<INamedPipeSerializer<DummyMessage>>());

                PipeSecurity security = null;

                server.Start(pipeSecurity =>
                {
                    security = pipeSecurity;
                });

                security.Should().BeOfType<PipeSecurity>();
            }

            [Fact]
            public void Should_Start_Server()
            {
                var serializer = new NamedPipeSerializer<DummyMessage>();

                var server = new NamedPipeServer<DummyMessage>(Create<string>(), serializer);

                server.Start();

                server.IsStarted.Should().BeTrue();
            }
        }

        [Collection("Pipes")]
        public class Start_PipeSecurity : NamedPipeServerFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Security_Null()
            {
                var server = new NamedPipeServer<DummyMessage>(Create<string>(), A.Fake<INamedPipeSerializer<DummyMessage>>());

                Invoking(() =>
                {
                    server.Start((PipeSecurity) null);
                })
                .Should()
                .NotThrow();
            }
        }

        [Collection("Pipes")]
        public class Stop : NamedPipeServerFixture
        {
            [Fact]
            public async Task Should_Stop_Server()
            {
                var serializer = new NamedPipeSerializer<DummyMessage>();

                var server = new NamedPipeServer<DummyMessage>(Create<string>(), serializer);

                server.Start();

                server.IsStarted.Should().BeTrue();

                await server.StopAsync();

                server.IsStarted.Should().BeFalse();
            }
        }

        [Collection("Pipes")]
        public class WriteAsync : NamedPipeServerFixture
        {
            private readonly string _pipeName;
            private readonly INamedPipeServer<DummyMessage> _server;

            public WriteAsync()
            {
                _pipeName = Create<string>();

                var serializer = new NamedPipeSerializer<DummyMessage>();

                _server = new NamedPipeServer<DummyMessage>(_pipeName, serializer);
            }

            public class WriteAsync_Message : WriteAsync
            {
                [Fact]
                public async Task Should_Throw_When_Message_Null()
                {
                    await Invoking(async () =>
                    {
                        await _server.WriteAsync(null);
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("message");
                }

                [Fact]
                public async Task Should_Throw_When_Cancelledl()
                {
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    await Invoking(async () =>
                    {
                        await _server.WriteAsync(Create<DummyMessage>(), cts.Token);
                    })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
                }
            }

            public class WriteAsync_Message_PipeName : WriteAsync
            {
                [Fact]
                public async Task Should_Throw_When_Message_Null()
                {
                    await Invoking(async () =>
                    {
                        await _server.WriteAsync(null, Create<string>());
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("message");
                }

                [Fact]
                public async Task Should_Throw_When_PipeName_Null()
                {
                    await Invoking(async () =>
                    {
                        await _server.WriteAsync(Create<DummyMessage>(), (string) null);
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("pipeName");
                }

                [Fact]
                public async Task Should_Throw_When_PipeName_Empty()
                {
                    await Invoking(async () =>
                    {
                        await _server.WriteAsync(Create<DummyMessage>(), string.Empty);
                    })
                    .Should()
                    .ThrowAsync<ArgumentException>()
                    .WithNamedMessageWhenEmpty("pipeName");
                }

                [Fact]
                public async Task Should_Throw_When_PipeName_Whitespace()
                {
                    await Invoking(async () =>
                    {
                        await _server.WriteAsync(Create<DummyMessage>(), "  ");
                    })
                    .Should()
                    .ThrowAsync<ArgumentException>()
                    .WithNamedMessageWhenEmpty("pipeName");
                }

                [Fact]
                public async Task Should_Throw_When_Cancelledl()
                {
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    await Invoking(async () =>
                    {
                        await _server.WriteAsync(Create<DummyMessage>(), Create<string>(), cts.Token);
                    })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
                }
            }

            public class WriteAsync_Message_Predicate : WriteAsync
            {
                [Fact]
                public async Task Should_Throw_When_Message_Null()
                {
                    await Invoking(async () =>
                    {
                        await _server.WriteAsync(null, _ => Create<bool>());
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("message");
                }

                [Fact]
                public async Task Should_Throw_When_Predicate_Null()
                {
                    await Invoking(async () =>
                    {
                        await _server.WriteAsync(Create<DummyMessage>(), (Func<INamedPipeConnection<DummyMessage>, bool>) null);
                    })
                    .Should()
                    .ThrowAsync<ArgumentNullException>()
                    .WithNamedMessageWhenNull("predicate");
                }

                [Fact]
                public async Task Should_Throw_When_Cancelledl()
                {
                    var cts = new CancellationTokenSource();
                    cts.Cancel();

                    await Invoking(async () =>
                    {
                        await _server.WriteAsync(Create<DummyMessage>(), _ => Create<bool>(), cts.Token);
                    })
                    .Should()
                    .ThrowAsync<OperationCanceledException>();
                }
            }
        }
    }

    // NamedPipeFixture_Functional contains additional tests
}