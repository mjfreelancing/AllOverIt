using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Logging.Extensions;
using AllOverIt.Logging.Testing;
using AllOverIt.Logging.Testing.Extensions;
using FluentAssertions;
using NSubstitute;

using ILogger = Microsoft.Extensions.Logging.ILogger;
using LoggerExtensions = AllOverIt.Logging.Extensions.LoggerExtensions;

namespace AllOverIt.Logging.Tests.Extensions
{
    public class LoggerExtensionsFixture : FixtureBase
    {
        private readonly ILogger _loggerFake;

        public LoggerExtensionsFixture()
        {
            _loggerFake = Substitute.For<ILogger>();
        }

        [Collection("LogCall")]
        public class LogCall : LoggerExtensionsFixture
        {
            private sealed class LogTester
            {
                private readonly ILogger _logger;

                public LogTester(ILogger logger)
                {
                    _logger = logger;
                }

                public void LogStaticCallWithNoArguments()
                {
                    _logger.LogCall(null);
                }

                public void LogStaticCallWithArguments(object arguments)
                {
                    _logger.LogCall(null, arguments);
                }

                public void LogCallWithNoArguments()
                {
                    _logger.LogCall(this);
                }

                public void LogCallWithArguments(string arg1, int arg2)
                {
                    _logger.LogCall(this, new { arg1, arg2 });
                }
            }

            private readonly LogTester _logTester;

            public LogCall()
            {
                _logTester = new(_loggerFake);
            }

            [Fact]
            public void Should_Throw_When_Logger_Null()
            {
                Invoking(() =>
                {
                    AllOverIt.Logging.Extensions.LoggerExtensions.LogCall(null!, this);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("logger");
            }

            [Fact]
            public void Should_Throw_When_CallerName_Null()
            {
                Invoking(() =>
                {
                    _loggerFake.LogCall(this, new { }, callerName: null!);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("callerName");
            }

            [Fact]
            public void Should_Throw_When_Caller_Empty()
            {
                Invoking(() =>
                {
                    _loggerFake.LogCall(this, new { }, callerName: string.Empty);
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("callerName");
            }

            [Fact]
            public void Should_Throw_When_Caller_Whitespace()
            {
                Invoking(() =>
                {
                    _loggerFake.LogCall(this, new { }, callerName: " ");
                })
                    .Should()
                    .Throw<ArgumentException>()
                    .WithNamedMessageWhenEmpty("callerName");
            }

            [Fact]
            public void Should_LogCall_Static_No_Arguments()
            {
                _loggerFake.AssertStaticLogCall(() =>
                {
                    _logTester.LogStaticCallWithNoArguments();
                }, nameof(LogTester.LogStaticCallWithNoArguments));
            }

            [Fact]
            public void Should_LogCall_No_Arguments()
            {
                _loggerFake.AssertLogCall<LogTester>(() =>
                {
                    _logTester.LogCallWithNoArguments();
                }, nameof(LogTester.LogCallWithNoArguments));
            }

            [Fact]
            public void Should_LogCall_Static_With_Arguments()
            {
                _loggerFake.AssertStaticLogCall(() =>
                {
                    _logTester.LogStaticCallWithNoArguments();
                }, nameof(LogTester.LogStaticCallWithNoArguments));
            }

            [Fact]
            public void Should_LogCall_With_Arguments()
            {
                var arg1 = Create<string>();
                var arg2 = Create<int>();

                _loggerFake.AssertLogCallWithArguments<LogTester>(() =>
                {
                    _logTester.LogCallWithArguments(arg1, arg2);
                }, nameof(LogTester.LogCallWithArguments), new { arg1, arg2 });
            }
        }

        [Collection("LogCall")]
        public class LogException : LoggerExtensionsFixture
        {
            private readonly Exception _exception;

            public LogException()
            {
                _exception = new Exception(Create<string>());
            }

            [Fact]
            public void Should_Throw_When_Logger_Null()
            {
                Invoking(() =>
                {
                    LoggerExtensions.LogException(null!, _exception);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("logger");
            }

            [Fact]
            public void Should_Throw_When_Exception_Null()
            {
                Invoking(() =>
                {
                    LoggerExtensions.LogException(_loggerFake, null!);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("exception");
            }

            [Fact]
            public void Should_Log_With_Default_Template()
            {
                var actual = _loggerFake.CaptureLogCalls(() =>
                {
                    LoggerExtensions.LogException(_loggerFake, _exception);
                });

                var states = actual.States;

                states.Should().BeEquivalentTo(
                [
                    LogCallExpectation.GetExpectedLogEntryWithTemplateAndArguments(
                        LogCallOptions.Instance.LogExceptionTemplate,
                        new
                        {
                            ErrorMessage = _exception.Message
                        })
                ]);
            }

            [Fact]
            public void Should_Log_With_Custom_Template()
            {
                var template = $"{Create<string>()} Error = {{CustomErrorMessage}}, Count = {{Count}}";
                var otherArg = Create<int>();

                var actual = _loggerFake.CaptureLogCalls(() =>
                {
                    LoggerExtensions.LogException(_loggerFake, _exception, template, _exception.Message, otherArg);
                });

                var states = actual.States;

                states.Should().BeEquivalentTo(
                [
                    LogCallExpectation.GetExpectedLogEntryWithTemplateAndArguments(
                        template,
                        new
                        {
                            CustomErrorMessage = _exception.Message,
                            Count = otherArg
                        })
                ]);
            }
        }

        [Collection("LogCall")]
        public class LogAllExceptions : LoggerExtensionsFixture
        {
            private readonly Exception _exception;
            private readonly Exception _innerException1;
            private readonly Exception _innerException2;

            public LogAllExceptions()
            {
                _innerException2 = new Exception(Create<string>());
                _innerException1 = new Exception(Create<string>(), _innerException2);
                _exception = new Exception(Create<string>(), _innerException1);
            }

            [Fact]
            public void Should_Throw_When_Logger_Null()
            {
                Invoking(() =>
                {
                    LoggerExtensions.LogAllExceptions(null!, _exception);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("logger");
            }

            [Fact]
            public void Should_Throw_When_Exception_Null()
            {
                Invoking(() =>
                {
                    LoggerExtensions.LogAllExceptions(_loggerFake, null!);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("exception");
            }

            [Fact]
            public void Should_Log_With_Default_Template()
            {
                var actual = _loggerFake.CaptureLogCalls(() =>
                {
                    LoggerExtensions.LogAllExceptions(_loggerFake, _exception);
                });

                var states = actual.States;

                states.Should().BeEquivalentTo(
                [
                    LogCallExpectation.GetExpectedLogEntryWithTemplateAndArguments(
                        LogCallOptions.Instance.LogExceptionTemplate,
                        new
                        {
                            ErrorMessage = _innerException2.Message
                        }),

                    LogCallExpectation.GetExpectedLogEntryWithTemplateAndArguments(
                        LogCallOptions.Instance.LogExceptionTemplate,
                        new
                        {
                            ErrorMessage = _innerException1.Message
                        }),

                    LogCallExpectation.GetExpectedLogEntryWithTemplateAndArguments(
                        LogCallOptions.Instance.LogExceptionTemplate,
                        new
                        {
                            ErrorMessage = _exception.Message
                        })
                ]);
            }

            [Fact]
            public void Should_Log_With_Custom_Template()
            {
                var template = $"{Create<string>()} Error = {{CustomErrorMessage}}, Count = {{Count}}";
                var otherArg = Create<int>();

                var actual = _loggerFake.CaptureLogCalls(() =>
                {
                    LoggerExtensions.LogAllExceptions(_loggerFake, _exception, template, _exception.Message, otherArg);
                });

                var states = actual.States;

                states.Should().BeEquivalentTo(
                [
                    LogCallExpectation.GetExpectedLogEntryWithTemplateAndArguments(
                        LogCallOptions.Instance.LogExceptionTemplate,
                        new
                        {
                            ErrorMessage = _innerException2.Message
                        }),

                    LogCallExpectation.GetExpectedLogEntryWithTemplateAndArguments(
                        LogCallOptions.Instance.LogExceptionTemplate,
                        new
                        {
                            ErrorMessage = _innerException1.Message
                        }),

                    LogCallExpectation.GetExpectedLogEntryWithTemplateAndArguments(
                        template,
                        new
                        {
                            CustomErrorMessage = _exception.Message,
                            Count = otherArg
                        })
                ]);
            }
        }
    }
}