using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pipes.Named.Connection;
using FluentAssertions;
using System.IO.Pipes;

namespace AllOverIt.Pipes.Tests.Connection
{
    public class NamedPipeStreamWriterFixture : FixtureBase
    {
        internal sealed class DummyPipeStreamException : Exception
        {
        }

        public sealed class DummyPipeStream : PipeStream
        {
            private Func<Task> _action;

            public DummyPipeStream()
                : base(PipeDirection.InOut, 1024)
            {
            }

            public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
            {
                if (_action is not null)
                {
                    // Invoke re-entry...
                    await _action?.Invoke();

                    // short-circuit the hijacking of this method 
                    throw new DummyPipeStreamException();
                }

                await base.WriteAsync(buffer, cancellationToken);
            }

            internal void SetupRentry(Func<Task> action)
            {
                IsConnected = true;
                _action = action;
            }

            internal void SetupIOException()
            {
                IsConnected = true;

                _action = () =>
                {
                    IsConnected = false;

                    throw new IOException();
                };
            }
        }

        [Collection("NamedPipes")]
        public class Constructor : NamedPipeStreamWriterFixture
        {
            [Fact]
            public void Should_Throw_When_PipeStream_Null()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeStreamWriter(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("pipeStream");
            }
        }

        [Collection("NamedPipes")]
        public class WriteAsync : NamedPipeStreamWriterFixture
        {
            private PipeStream _pipeStream = new DummyPipeStream();

            [Fact]
            public async Task Should_Throw_When_Buffer_Null()
            {
                var writer = new NamedPipeStreamWriter(_pipeStream);

                await Invoking(async () =>
                {
                    await writer.WriteAsync(null, CancellationToken.None);
                })
                .Should()
                .ThrowAsync<ArgumentNullException>()
                .WithNamedMessageWhenNull("buffer");
            }

            [Fact]
            public async Task Should_Throw_When_Buffer_Empty()
            {
                var writer = new NamedPipeStreamWriter(_pipeStream);

                await Invoking(async () =>
                {
                    await writer.WriteAsync(Array.Empty<byte>(), CancellationToken.None);
                })
                .Should()
                .ThrowAsync<ArgumentException>()
                .WithNamedMessageWhenEmpty("buffer");
            }

            [Fact]
            public async Task Should_Throw_When_Cancelled()
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();

                var bytes = CreateMany<byte>().ToArray();

                var writer = new NamedPipeStreamWriter(_pipeStream);

                await Invoking(async () =>
                {
                    await writer.WriteAsync(bytes, cts.Token);
                })
                .Should()
                .ThrowAsync<OperationCanceledException>();
            }

            [Fact]
            public async Task Should_Not_Throw_When_Not_Connected()
            {
                var bytes = CreateMany<byte>().ToArray();

                var writer = new NamedPipeStreamWriter(_pipeStream);

                await Invoking(async () =>
                {
                    await writer.WriteAsync(bytes, CancellationToken.None);
                })
                .Should()
                .NotThrowAsync();
            }

            [Fact]
            public async Task Should_Disconnect_When_Reentry_Detected()
            {
                var bytes = CreateMany<byte>().ToArray();

                var dummyPipeStream = new DummyPipeStream();

                _pipeStream = dummyPipeStream;

                var writer = new NamedPipeStreamWriter(_pipeStream);

                dummyPipeStream.SetupRentry(() => writer.WriteAsync(bytes, CancellationToken.None));

                _pipeStream.IsConnected.Should().BeTrue();

                try
                {
                    await writer.WriteAsync(bytes, CancellationToken.None);
                }
                catch (DummyPipeStreamException)
                {
                }

                _pipeStream.IsConnected.Should().BeFalse();
            }

            [Fact]
            public async Task Should_Handle_IOException()
            {
                var bytes = CreateMany<byte>().ToArray();

                var dummyPipeStream = new DummyPipeStream();

                _pipeStream = dummyPipeStream;

                var writer = new NamedPipeStreamWriter(_pipeStream);

                dummyPipeStream.SetupIOException();

                _pipeStream.IsConnected.Should().BeTrue();

                await writer.WriteAsync(bytes, CancellationToken.None);

                _pipeStream.IsConnected.Should().BeFalse();
            }
        }
    }
}