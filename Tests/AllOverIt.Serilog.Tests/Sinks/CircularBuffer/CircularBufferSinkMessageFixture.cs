using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Serilog.Sinks.CircularBuffer;
using FluentAssertions;
using Serilog.Events;
using System.Diagnostics;

namespace AllOverIt.Serilog.Tests.Sinks.CircularBuffer
{
    public class CircularBufferSinkMessageFixture : FixtureBase
    {
        [Fact]
        public void Should_Throw_When_LogEvent_Null()
        {
            Invoking(() =>
            {
                _ = new CircularBufferSinkMessage(null, Create<string>());
            })
            .Should()
            .Throw<ArgumentNullException>()
            .WithNamedMessageWhenNull("logEvent");
        }

        [Fact]
        public void Should_Throw_When_FormattedMessage_Null()
        {
            Invoking(() =>
            {
                var logEvent = new LogEvent(
                    DateTime.Now,
                    Create<LogEventLevel>(),
                    null,
                    MessageTemplate.Empty,
                    Array.Empty<LogEventProperty>(),
                    new ActivityTraceId(),
                    new ActivitySpanId());

                _ = new CircularBufferSinkMessage(logEvent, null);
            })
            .Should()
            .Throw<ArgumentNullException>()
            .WithNamedMessageWhenNull("formattedMessage");
        }
    }
}