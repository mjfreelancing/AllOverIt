using AllOverIt.Fixture;
using AllOverIt.Serilog.Enrichers.ThreadId;
using AllOverIt.Serilog.Extensions;
using AllOverIt.Serilog.Sinks.CircularBuffer;
using FluentAssertions;
using Serilog;
using Shouldly;

namespace AllOverIt.Serilog.Tests.Enrichers.ThreadId
{
    public class ThreadIdEnricherFixture : FixtureBase
    {
        [Fact]
        public void Should_Add_ThreadId_To_Properties()
        {
            var sink = new CircularBufferSinkMessages(2);

            var logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.With<ThreadIdEnricher>()
                .WriteTo.CircularBuffer(sink)
                .CreateLogger();

            logger.Information(Create<string>());
            logger.Information(Create<string>());

            sink.Count().ShouldBe(2);

            foreach (var item in sink)
            {
                item.LogEvent.Properties[ThreadIdEnricher.ThreadIdPropertyName].ToString()
                    .Should()
                    .NotBeNullOrEmpty();
            }
        }
    }
}
