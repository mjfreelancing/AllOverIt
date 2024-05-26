using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Serilog.Sinks.CircularBuffer;
using FluentAssertions;
using Serilog;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace AllOverIt.Serilog.Tests.Extensions
{
    public class LoggerSinkConfigurationExtensionsFixture : FixtureBase
    {
        public class CircularBuffer_OutputTemplate : LoggerSinkConfigurationExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_SinkConfiguration_Null()
            {
                Invoking(() =>
                {
                    _ = AllOverIt.Serilog.Extensions.LoggerSinkConfigurationExtensions
                        .CircularBuffer(null, new CircularBufferSinkMessages(1), Create<string>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("sinkConfiguration");
            }

            [Fact]
            public void Should_Throw_When_OutputTemplate_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        var loggerSinkConfiguration = new LoggerConfiguration().WriteTo;

                        AllOverIt.Serilog.Extensions.LoggerSinkConfigurationExtensions
                            .CircularBuffer(loggerSinkConfiguration, new CircularBufferSinkMessages(1), stringValue);
                    }, "outputTemplate");
            }

            [Fact]
            public void Should_Return_LoggerConfiguration()
            {
                var loggerSinkConfiguration = new LoggerConfiguration().WriteTo;

                var actual = AllOverIt.Serilog.Extensions.LoggerSinkConfigurationExtensions
                    .CircularBuffer(loggerSinkConfiguration, new CircularBufferSinkMessages(1), Create<string>());

                actual.Should().BeOfType<LoggerConfiguration>();
            }
        }

        public class CircularBuffer_TextFormatter : LoggerSinkConfigurationExtensionsFixture
        {
            private static readonly ITextFormatter formatter = new MessageTemplateTextFormatter("Message", null);

            [Fact]
            public void Should_Throw_When_SinkConfiguration_Null()
            {
                Invoking(() =>
                {
                    _ = AllOverIt.Serilog.Extensions.LoggerSinkConfigurationExtensions
                        .CircularBuffer(null, new CircularBufferSinkMessages(1), formatter);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("sinkConfiguration");
            }

            [Fact]
            public void Should_Throw_When_Formatter_Null()
            {
                Invoking(() =>
                {
                    var loggerSinkConfiguration = new LoggerConfiguration().WriteTo;

                    AllOverIt.Serilog.Extensions.LoggerSinkConfigurationExtensions
                        .CircularBuffer(loggerSinkConfiguration, new CircularBufferSinkMessages(1), (ITextFormatter) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("formatter");
            }

            [Fact]
            public void Should_Return_LoggerConfiguration()
            {
                var loggerSinkConfiguration = new LoggerConfiguration().WriteTo;

                var actual = AllOverIt.Serilog.Extensions.LoggerSinkConfigurationExtensions
                    .CircularBuffer(loggerSinkConfiguration, new CircularBufferSinkMessages(1), formatter);

                actual.Should().BeOfType<LoggerConfiguration>();
            }
        }
    }
}