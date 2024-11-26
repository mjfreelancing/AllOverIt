using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Serilog.Sinks.Observable;
using FluentAssertions;
using Serilog;

namespace AllOverIt.Serilog.Tests.Extensions
{
    public partial class LoggerSinkConfigurationExtensionsFixture : FixtureBase
    {
        public class Observable : LoggerSinkConfigurationExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_SinkConfiguration_Null()
            {
                Invoking(() =>
                {
                    _ = AllOverIt.Serilog.Extensions.LoggerSinkConfigurationExtensions
                        .Observable(null, new ObservableSink());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("sinkConfiguration");
            }

            [Fact]
            public void Should_Throw_When_ObservableSink_Null()
            {
                Invoking(() =>
                {
                    var loggerSinkConfiguration = new LoggerConfiguration().WriteTo;

                    _ = AllOverIt.Serilog.Extensions.LoggerSinkConfigurationExtensions
                        .Observable(loggerSinkConfiguration, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("observableSink");
            }

            [Fact]
            public void Should_Return_LoggerConfiguration()
            {
                var loggerSinkConfiguration = new LoggerConfiguration().WriteTo;

                var actual = AllOverIt.Serilog.Extensions.LoggerSinkConfigurationExtensions
                    .Observable(loggerSinkConfiguration, new ObservableSink());

                actual.Should().BeOfType<LoggerConfiguration>();
            }
        }
    }
}