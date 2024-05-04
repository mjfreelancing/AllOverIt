using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Process;
using FluentAssertions;
using System.Diagnostics;

namespace AllOverIt.Tests.Process
{
    public class ProcessBuilderFixture : FixtureBase
    {
        [Fact]
        public void Should_Throw_When_Process_FileName_Is_Null()
        {
            Invoking(() =>
            {
                _ = ProcessBuilder.For(null);
            })
            .Should()
            .Throw<ArgumentNullException>()
            .WithNamedMessageWhenNull("processFileName");
        }

        [Fact]
        public void Should_Throw_When_Process_FileName_Is_Empty()
        {
            Invoking(() =>
            {
                _ = ProcessBuilder.For(string.Empty);
            })
            .Should()
            .Throw<ArgumentException>()
            .WithNamedMessageWhenEmpty("processFileName");
        }

        [Fact]
        public void Should_Throw_When_Process_FileName_Is_Whitespace()
        {
            Invoking(() =>
            {
                _ = ProcessBuilder.For("  ");
            })
            .Should()
            .Throw<ArgumentException>()
            .WithNamedMessageWhenEmpty("processFileName");
        }

        [Fact]
        public void Should_Return_Expected_ProcessExecutorOptions()
        {
            var processFileName = Create<string>();

            var actual = ProcessBuilder.For(processFileName);

            var expected = new
            {
                ProcessFileName = processFileName,
                WorkingDirectory = (string) default,
                Arguments = (string) default,
                NoWindow = (bool) default,
                Timeout = (TimeSpan) default,
                EnvironmentVariables = (IDictionary<string, string>) default,
                StandardOutputHandler = (DataReceivedEventHandler) default,
                ErrorOutputHandler = (DataReceivedEventHandler) default
            };

            expected.Should().BeEquivalentTo(actual);
        }
    }
}