using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Serilog.Extensions;
using AllOverIt.Serilog.Sinks.CircularBuffer;
using FluentAssertions;
using Serilog;
using Serilog.Formatting.Display;
using System;
using System.Linq;

namespace AllOverIt.Serilog.Tests.Sinks.CircularBuffer
{
    public class CircularBufferSinkFixture : FixtureBase
    {
        [Fact]
        public void Should_Throw_When_SinkMessages_Null()
        {
            Invoking(() =>
            {
                var formatter = new MessageTemplateTextFormatter("Message", null);

                _ = new CircularBufferSink(null, formatter);
            })
            .Should()
            .Throw<ArgumentNullException>()
            .WithNamedMessageWhenNull("sinkMessages");
        }

        [Fact]
        public void Should_Throw_When_Formatter_Null()
        {
            Invoking(() =>
            {
                _ = new CircularBufferSink(new CircularBufferSinkMessages(1), null);
            })
            .Should()
            .Throw<ArgumentNullException>()
            .WithNamedMessageWhenNull("formatter");
        }

        [Fact]
        public void Should_Capture_Log_Events()
        {
            var messages = CreateMany<string>(5).ToArray();

            var sink = new CircularBufferSinkMessages(3);

            var logger = new LoggerConfiguration().WriteTo.CircularBuffer(sink).CreateLogger();

            logger.Information(messages[0]);

            sink.Length.Should().Be(1);
            sink.Single().FormattedMessage.Should().Contain(messages[0]);

            logger.Information(messages[1]);

            sink.Length.Should().Be(2);
            sink.Take(1).Single().FormattedMessage.Should().Contain(messages[0]);
            sink.Skip(1).Take(1).Single().FormattedMessage.Should().Contain(messages[1]);

            logger.Information(messages[2]);

            sink.Length.Should().Be(3);
            sink.Take(1).Single().FormattedMessage.Should().Contain(messages[0]);
            sink.Skip(1).Take(1).Single().FormattedMessage.Should().Contain(messages[1]);
            sink.Skip(2).Take(1).Single().FormattedMessage.Should().Contain(messages[2]);

            logger.Information(messages[3]);

            sink.Length.Should().Be(3);
            sink.Take(1).Single().FormattedMessage.Should().Contain(messages[1]);
            sink.Skip(1).Take(1).Single().FormattedMessage.Should().Contain(messages[2]);
            sink.Skip(2).Take(1).Single().FormattedMessage.Should().Contain(messages[3]);

            logger.Information(messages[4]);

            sink.Length.Should().Be(3);
            sink.Take(1).Single().FormattedMessage.Should().Contain(messages[2]);
            sink.Skip(1).Take(1).Single().FormattedMessage.Should().Contain(messages[3]);
            sink.Skip(2).Take(1).Single().FormattedMessage.Should().Contain(messages[4]);
        }
    }
}