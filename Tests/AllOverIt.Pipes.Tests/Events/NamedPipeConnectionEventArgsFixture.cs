using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pipes.Named.Connection;
using AllOverIt.Pipes.Named.Events;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Pipes.Tests.Events
{

    public class NamedPipeConnectionEventArgsFixture : FixtureBase
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

        public class Constructor : NamedPipeConnectionEventArgsFixture
        {
            [Fact]
            public void Should_Throw_When_Connection_Null()
            {
                Invoking(() =>
                {
                    _ = new NamedPipeConnectionEventArgs<DummyMessage, DummyNamedPipeConnection>(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("connection");
            }

            [Fact]
            public void Should_Return_Connection()
            {
                var expected = Create<DummyNamedPipeConnection>();

                var eventArgs = new NamedPipeConnectionEventArgs<DummyMessage, DummyNamedPipeConnection>(expected);

                eventArgs.Connection.Should().BeSameAs(expected);
            }
        }
    }
}