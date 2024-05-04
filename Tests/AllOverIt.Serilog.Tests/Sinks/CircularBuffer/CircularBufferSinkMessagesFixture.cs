using AllOverIt.Fixture;
using AllOverIt.Serilog.Sinks.CircularBuffer;
using FluentAssertions;

namespace AllOverIt.Serilog.Tests.Sinks.CircularBuffer
{
    public class CircularBufferSinkMessagesFixture : FixtureBase
    {
        [Fact]
        public void Should_Throw_When_Invalid_Capacity()
        {
            Invoking(() =>
            {
                _ = new CircularBufferSinkMessages(0);
            })
            .Should()
            .Throw<ArgumentOutOfRangeException>()
            .WithMessage("The circular buffer requires a capacity of at least 1. (Parameter 'capacity')");
        }

        [Fact]
        public void Should_Not_Throw_When_Valid_Capacity()
        {
            Invoking(() =>
            {
                _ = new CircularBufferSinkMessages(1);
            })
            .Should()
            .NotThrow();
        }

        [Fact]
        public void Should_Have_Expected_Capacity()
        {
            var capacity = Create<int>();

            var messages = new CircularBufferSinkMessages(capacity);

            messages.Capacity.Should().Be(capacity);
        }
    }
}