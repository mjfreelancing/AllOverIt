using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pipes.Named.Connection;
using AllOverIt.Pipes.Named.Events;
using FluentAssertions;

namespace AllOverIt.Pipes.Tests.Events
{

    public class NamedPipeConnectionMessageEventArgsFixture : FixtureBase
    {
        public sealed class DummyMessage
        {
            public int Id { get; set; }
        }

        public class DummyNamedPipeConnection : INamedPipeConnection<DummyMessage>
        {
            public string ConnectionId => throw new NotImplementedException();

            public bool IsConnected => throw new NotImplementedException();

            public ValueTask DisposeAsync()
            {
                throw new NotImplementedException();
            }

            public Task WriteAsync(DummyMessage message, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }

        public class Constructor : NamedPipeConnectionMessageEventArgsFixture
        {
            [Fact]
            public void Should_Throw_When_Connection_Null()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeConnectionMessageEventArgs<DummyMessage, DummyNamedPipeConnection>(null, Create<DummyMessage>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("connection");
            }

            [Fact]
            public void Should_Throw_When_Message_Null()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeConnectionMessageEventArgs<DummyMessage, DummyNamedPipeConnection>(Create<DummyNamedPipeConnection>(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("message");
            }

            [Fact]
            public void Should_Return_Connection()
            {
                var expected = Create<DummyNamedPipeConnection>();

                var eventArgs = new NamedPipeConnectionMessageEventArgs<DummyMessage, DummyNamedPipeConnection>(expected, Create<DummyMessage>());

                eventArgs.Connection.Should().BeSameAs(expected);
            }

            [Fact]
            public void Should_Return_Message()
            {
                var expected = Create<DummyMessage>();

                var eventArgs = new NamedPipeConnectionMessageEventArgs<DummyMessage, DummyNamedPipeConnection>(Create<DummyNamedPipeConnection>(), expected);

                eventArgs.Message.Should().BeSameAs(expected);
            }
        }
    }
}