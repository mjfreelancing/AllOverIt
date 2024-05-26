using AllOverIt.Fixture;
using AllOverIt.Process;
using FluentAssertions;
using System.Diagnostics;

namespace AllOverIt.Tests.Process
{
    public class ProcessBuilderFixture : FixtureBase
    {
        [Fact]
        public void Should_Throw_When_Process_FileName_Is_Null_Empty_Whitespace()
        {
            AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                stringValue =>
                {
                    _ = ProcessBuilder.For(stringValue);
                }, "processFileName");
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