using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Process;
using AllOverIt.Process.Extensions;
using FakeItEasy.Sdk;
using FluentAssertions;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Process
{
    public class ProcessExecutorOptionsFixture : FixtureBase
    {
        private readonly ProcessExecutorOptions _sut;

        public ProcessExecutorOptionsFixture()
        {
            // Create<ProcessExecutorOptions>() won't set any properties as they all have internal setters.
            // We could rely on just checking individual properties but this ensures all properties remain
            // unchanged when a single WithXXX() call is made.
            _sut = new ProcessExecutorOptions(Create<string>())
                .WithWorkingDirectory(Create<string>())
                .WithArguments(Create<string>(), Create<string>(), Create<string>())
                .WithTimeout(Create<TimeSpan>())
                .WithEnvironmentVariables(Create<Dictionary<string, string>>())
                .WithStandardOutputHandler(LogStandardOutput1)
                .WithErrorOutputHandler(LogErrorOutput1);
        }

        public class SetupCheck : ProcessExecutorOptionsFixture
        {
            [Fact]
            public void Should_Have_Populated_Properties()
            {
                _sut.ProcessFileName.Should().NotBeNull();
                _sut.WorkingDirectory.Should().NotBeNull();
                _sut.Arguments.Should().NotBeNull();
                _sut.Timeout.Should().NotBe(default);
                _sut.EnvironmentVariables.Should().NotBeNull();
                _sut.StandardOutputHandler.Should().NotBeNull();
                _sut.ErrorOutputHandler.Should().NotBeNull();
            }
        }

        public class Constructor : ProcessExecutorOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_ProcessFileName_Null()
            {
                Invoking(() =>
                {
                    _ = new ProcessExecutorOptions(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("processFileName");
            }

            [Fact]
            public void Should_Set_ProcessFileName()
            {
                var expected = Create<string>();

                var options = new ProcessExecutorOptions(expected);

                var actual = options.ProcessFileName;

                actual.Should().Be(expected);
            }
        }

        private static void LogStandardOutput1(object sender, DataReceivedEventArgs e)
        {
        }

        private static void LogErrorOutput1(object sender, DataReceivedEventArgs e)
        {
        }

        private static void LogStandardOutput2(object sender, DataReceivedEventArgs e)
        {
        }

        private static void LogErrorOutput2(object sender, DataReceivedEventArgs e)
        {
        }
    }
}