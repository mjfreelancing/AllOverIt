using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Process;
using AllOverIt.Process.Extensions;
using AllOverIt.Shouldly.Extensions;
using FluentAssertions.Extensions;
using System.Diagnostics;

namespace AllOverIt.Tests.Process.Extensions
{
    public class ProcessExecutorOptionsExtensionsFixture : FixtureBase
    {
        private readonly ProcessExecutorOptions _sut;

        public ProcessExecutorOptionsExtensionsFixture()
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

        // THIS TEST MUST BE UPDATED IF ADDITIONAL PROPERTIES ARE ADDED TO ProcessExecutorOptions
        public class SetupCheck : ProcessExecutorOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Have_Populated_Properties()
            {
                _sut.ProcessFileName.ShouldNotBeNull();
                _sut.WorkingDirectory.ShouldNotBeNull();
                _sut.Arguments.ShouldNotBeNull();
                _sut.NoWindow.ShouldBeFalse();
                _sut.Timeout.ShouldNotBe(default);
                _sut.EnvironmentVariables.ShouldNotBeNull();
                _sut.StandardOutputHandler.ShouldNotBeNull();
                _sut.ErrorOutputHandler.ShouldNotBeNull();
            }
        }

        public class WithWorkingDirectory : ProcessExecutorOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Not_Throw_When_WorkingDirectory_Null()
            {
                Invoking(() =>
                {
                    _ = ProcessExecutorOptionsExtensions.WithWorkingDirectory(_sut, null);
                })
               .ShouldNotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_WorkingDirectory_Empty()
            {
                Invoking(() =>
                {
                    _ = ProcessExecutorOptionsExtensions.WithWorkingDirectory(_sut, string.Empty);
                })
               .ShouldNotThrow();
            }

            [Fact]
            public void Should_Not_Throw_When_WorkingDirectory_Whitespace()
            {
                Invoking(() =>
                {
                    _ = ProcessExecutorOptionsExtensions.WithWorkingDirectory(_sut, " ");
                })
               .ShouldNotThrow();
            }

            [Fact]
            public void Should_Set_WorkingDirectory()
            {
                var value = Create<string>();

                var expected = new
                {
                    _sut.ProcessFileName,
                    WorkingDirectory = value,
                    _sut.Arguments,
                    _sut.NoWindow,
                    _sut.Timeout,
                    _sut.EnvironmentVariables,
                    _sut.StandardOutputHandler,
                    _sut.ErrorOutputHandler
                };

                var actual = ProcessExecutorOptionsExtensions.WithWorkingDirectory(_sut, value);

                actual.ShouldBeEquivalentTo(expected);
            }
        }

        public class WithArguments_Params : ProcessExecutorOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Arguments_Null()
            {
                Invoking(() =>
                {
                    _ = ProcessExecutorOptionsExtensions.WithArguments(_sut, null);
                })
                .ShouldThrow<ArgumentNullException>()
                .WithNamedMessageWhenNull("arguments");
            }

            [Fact]
            public void Should_Not_Throw_When_Arguments_Empty()
            {
                Invoking(() =>
                {
                    _ = ProcessExecutorOptionsExtensions.WithArguments(_sut, Array.Empty<string>());
                })
                .ShouldNotThrow();
            }

            [Fact]
            public void Should_Set_Arguments()
            {
                var values = CreateMany<string>(3).ToArray();

                var expected = new
                {
                    _sut.ProcessFileName,
                    _sut.WorkingDirectory,
                    Arguments = string.Join(" ", values),
                    _sut.NoWindow,
                    _sut.Timeout,
                    _sut.EnvironmentVariables,
                    _sut.StandardOutputHandler,
                    _sut.ErrorOutputHandler
                };

                var actual = ProcessExecutorOptionsExtensions.WithArguments(_sut, values[0], values[1], values[2]);

                actual.ShouldBeEquivalentTo(expected);
            }
        }

        public class WithArguments_Enumerable : ProcessExecutorOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Arguments_Null()
            {
                Invoking(() =>
                {
                    _ = ProcessExecutorOptionsExtensions.WithArguments(_sut, null, Create<bool>());
                })
                .ShouldThrow<ArgumentNullException>()
                .WithNamedMessageWhenNull("arguments");
            }

            [Fact]
            public void Should_Not_Throw_When_Arguments_Empty()
            {
                Invoking(() =>
                {
                    _ = ProcessExecutorOptionsExtensions.WithArguments(_sut, Array.Empty<string>(), Create<bool>());
                })
                .ShouldNotThrow();
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_Set_Arguments(bool escape)
            {
                var values = CreateMany<string>(3).ToArray();

                var expected = new
                {
                    _sut.ProcessFileName,
                    _sut.WorkingDirectory,
                    Arguments = string.Join(" ", values),
                    _sut.NoWindow,
                    _sut.Timeout,
                    _sut.EnvironmentVariables,
                    _sut.StandardOutputHandler,
                    _sut.ErrorOutputHandler
                };

                var actual = ProcessExecutorOptionsExtensions.WithArguments(_sut, values, escape);

                actual.ShouldBeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Set_Escaped_Arguments()
            {
                var values = CreateMany<string>(3).Concat(new[] { "a a" }).ToArray();

                var expected = new
                {
                    _sut.ProcessFileName,
                    _sut.WorkingDirectory,
                    Arguments = string.Join(" ", values.Take(3).Concat(new[] { "\"a a\"" })),
                    _sut.NoWindow,
                    _sut.Timeout,
                    _sut.EnvironmentVariables,
                    _sut.StandardOutputHandler,
                    _sut.ErrorOutputHandler
                };

                var actual = ProcessExecutorOptionsExtensions.WithArguments(_sut, values, true);

                actual.ShouldBeEquivalentTo(expected);
            }
        }

        public class WithNoWindow : ProcessExecutorOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Set_NoWindow()
            {
                var expected = new
                {
                    _sut.ProcessFileName,
                    _sut.WorkingDirectory,
                    _sut.Arguments,
                    NoWindow = true,
                    _sut.Timeout,
                    _sut.EnvironmentVariables,
                    _sut.StandardOutputHandler,
                    _sut.ErrorOutputHandler
                };

                var actual = ProcessExecutorOptionsExtensions.WithNoWindow(_sut);

                actual.ShouldBeEquivalentTo(expected);
            }
        }

        public class WithTimeout_Milliseconds : ProcessExecutorOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Set_Timeout()
            {
                var value = TimeSpan.FromSeconds(Create<int>());

                var expected = new
                {
                    _sut.ProcessFileName,
                    _sut.WorkingDirectory,
                    _sut.Arguments,
                    _sut.NoWindow,
                    Timeout = value,
                    _sut.EnvironmentVariables,
                    _sut.StandardOutputHandler,
                    _sut.ErrorOutputHandler
                };

                var actual = ProcessExecutorOptionsExtensions.WithTimeout(_sut, value.TotalMilliseconds);

                actual.ShouldBeEquivalentTo(expected, opts => opts
                    .UseComparer<TimeSpan>((a, e) => Math.Abs(a.TotalMilliseconds - e.TotalMilliseconds) <= 1));
            }
        }

        public class WithTimeout_TimeSpan : ProcessExecutorOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Set_Timeout()
            {
                var value = Create<TimeSpan>();

                var expected = new
                {
                    _sut.ProcessFileName,
                    _sut.WorkingDirectory,
                    _sut.Arguments,
                    _sut.NoWindow,
                    Timeout = value,
                    _sut.EnvironmentVariables,
                    _sut.StandardOutputHandler,
                    _sut.ErrorOutputHandler
                };

                var actual = ProcessExecutorOptionsExtensions.WithTimeout(_sut, value);

                actual.ShouldBeEquivalentTo(expected);
            }
        }

        public class WithEnvironmentVariables_Dictionary : ProcessExecutorOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_EnvironmentVariables_Null()
            {
                Invoking(() =>
                {
                    _ = ProcessExecutorOptionsExtensions.WithEnvironmentVariables(_sut, (IDictionary<string, string>) null);
                })
               .ShouldThrow<ArgumentNullException>()
                .WithNamedMessageWhenNull("environmentVariables");
            }

            [Fact]
            public void Should_Not_Throw_When_EnvironmentVariables_Empty()
            {
                Invoking(() =>
                {
                    _ = ProcessExecutorOptionsExtensions.WithEnvironmentVariables(_sut, new Dictionary<string, string>());
                })
                .ShouldNotThrow();
            }

            [Fact]
            public void Should_Set_EnvironmentVariables()
            {
                var value = Create<Dictionary<string, string>>();

                var expected = new
                {
                    _sut.ProcessFileName,
                    _sut.WorkingDirectory,
                    _sut.Arguments,
                    _sut.NoWindow,
                    _sut.Timeout,
                    EnvironmentVariables = value,
                    _sut.StandardOutputHandler,
                    _sut.ErrorOutputHandler
                };

                var actual = ProcessExecutorOptionsExtensions.WithEnvironmentVariables(_sut, value);

                actual.ShouldBeEquivalentTo(expected);
            }
        }

        public class WithEnvironmentVariables_Action : ProcessExecutorOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Action_Null()
            {
                Invoking(() =>
                {
                    _ = ProcessExecutorOptionsExtensions.WithEnvironmentVariables(_sut, (Action<IDictionary<string, string>>) null);
                })
               .ShouldThrow<ArgumentNullException>()
                .WithNamedMessageWhenNull("configure");
            }

            [Fact]
            public void Should_Not_Throw_When_EnvironmentVariables_Empty()
            {
                Invoking(() =>
                {
                    _ = ProcessExecutorOptionsExtensions.WithEnvironmentVariables(_sut, variables => { });
                })
                .ShouldNotThrow();
            }

            [Fact]
            public void Should_Set_EnvironmentVariables()
            {
                // we need an initially null environment variables dictionary
                var sut = new ProcessExecutorOptions(Create<string>());

                var values = Create<Dictionary<string, string>>();

                var expected = new
                {
                    sut.ProcessFileName,
                    sut.WorkingDirectory,
                    sut.Arguments,
                    _sut.NoWindow,
                    sut.Timeout,
                    EnvironmentVariables = values,
                    sut.StandardOutputHandler,
                    sut.ErrorOutputHandler
                };

                var actual = ProcessExecutorOptionsExtensions.WithEnvironmentVariables(sut, variables =>
                {
                    foreach (var (key, value) in values)
                    {
                        variables[key] = value;
                    }
                });

                actual.ShouldBeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Append_EnvironmentVariables()
            {
                var values = Create<Dictionary<string, string>>();

                var expected = new
                {
                    _sut.ProcessFileName,
                    _sut.WorkingDirectory,
                    _sut.Arguments,
                    _sut.NoWindow,
                    _sut.Timeout,
                    EnvironmentVariables = _sut.EnvironmentVariables.Concat(values).ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                    _sut.StandardOutputHandler,
                    _sut.ErrorOutputHandler
                };

                var actual = ProcessExecutorOptionsExtensions.WithEnvironmentVariables(_sut, variables =>
                {
                    foreach (var (key, value) in values)
                    {
                        variables[key] = value;
                    }
                });

                actual.ShouldBeEquivalentTo(expected);
            }
        }

        public class WithStandardOutputHandler : ProcessExecutorOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Handler_Null()
            {
                Invoking(() =>
                {
                    _ = ProcessExecutorOptionsExtensions.WithStandardOutputHandler(_sut, null);
                })
                .ShouldNotThrow();
            }

            [Fact]
            public void Should_Set_StandardOutputHandler()
            {
                var expected = new
                {
                    _sut.ProcessFileName,
                    _sut.WorkingDirectory,
                    _sut.Arguments,
                    _sut.NoWindow,
                    _sut.Timeout,
                    _sut.EnvironmentVariables,
                    StandardOutputHandler = (DataReceivedEventHandler) LogStandardOutput2,
                    _sut.ErrorOutputHandler
                };

                var actual = ProcessExecutorOptionsExtensions.WithStandardOutputHandler(_sut, LogStandardOutput2);

                actual.ShouldBeEquivalentTo(expected);
            }
        }

        public class WithErrorOutputHandler : ProcessExecutorOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Not_Throw_When_Handler_Null()
            {
                Invoking(() =>
                {
                    _ = ProcessExecutorOptionsExtensions.WithErrorOutputHandler(_sut, null);
                })
                .ShouldNotThrow();
            }

            [Fact]
            public void Should_Set_ErrorOutputHandler()
            {
                var expected = new
                {
                    _sut.ProcessFileName,
                    _sut.WorkingDirectory,
                    _sut.Arguments,
                    _sut.NoWindow,
                    _sut.Timeout,
                    _sut.EnvironmentVariables,
                    _sut.StandardOutputHandler,
                    ErrorOutputHandler = (DataReceivedEventHandler) LogErrorOutput2
                };

                var actual = ProcessExecutorOptionsExtensions.WithErrorOutputHandler(_sut, LogErrorOutput2);

                actual.ShouldBeEquivalentTo(expected);
            }
        }

        public class BuildProcessExecutor : ProcessExecutorOptionsExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Options_Null()
            {
                Invoking(() =>
                {
                    _ = ProcessExecutorOptionsExtensions.BuildProcessExecutor(null);
                })
                .ShouldThrow<ArgumentNullException>()
                .WithNamedMessageWhenNull("options");
            }

            [Fact]
            public void Should_Create_Executor()
            {
                var actual = ProcessExecutorOptionsExtensions.BuildProcessExecutor(_sut);

                actual.ShouldBeOfType<ProcessExecutor>();
            }

            [Fact]
            public void Should_Have_Options_On_Executor()
            {
                var actual = ProcessExecutorOptionsExtensions.BuildProcessExecutor(_sut) as ProcessExecutor;

                actual._options.ShouldBeSameAs(_sut);
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











