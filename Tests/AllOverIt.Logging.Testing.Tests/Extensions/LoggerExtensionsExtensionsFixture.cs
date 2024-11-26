using AllOverIt.Fixture;
using AllOverIt.Logging.Extensions;
using AllOverIt.Logging.Testing.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AllOverIt.Logging.Testing.Tests.Extensions
{
    public class LoggerExtensionsExtensionsFixture : FixtureBase
    {
        private sealed class DummyClass
        {
            private readonly ILogger _logger;

            public DummyClass(ILogger logger)
            {
                _logger = logger;
            }

            public static void CallStaticMethod(ILogger logger)
            {
                logger.LogCall(null);
            }

            public static void CallStaticMethodWithArguments(ILogger logger, int value1, string value2)
            {
                logger.LogCall(null, new { value1, value2 });
            }

            public void CallMethod()
            {
                _logger.LogCall(this);
            }

            public void CallMethodWithArguments(int value1, string value2)
            {
                _logger.LogCall(this, new { value1, value2 });
            }

            public Task CallMethodAsync()
            {
                _logger.LogCall(this);

                return Task.CompletedTask;
            }

            public Task CallMethodWithArgumentsAsync(int value1, string value2)
            {
                _logger.LogCall(this, new { value1, value2 });

                return Task.CompletedTask;
            }

            public void CallMethodWithException(Exception exception)
            {
                _logger.LogException(exception);
            }

            public void CallMethodWithExceptionAndArguments(Exception exception, string logTemplate, object arg1, object arg2)
            {
                _logger.LogException(exception, logTemplate, arg1, arg2);
            }

            public void LogDebugMethod(string message)
            {
                _logger.LogDebug(message);
            }

            public void LogInformationMethodWithArguments(string template, params object[] arguments)
            {
                _logger.LogInformation(template, arguments);
            }
        }

        private readonly DummyClass _dummyClass;
        private readonly ILogger _loggerFake;

        public LoggerExtensionsExtensionsFixture()
        {
            _loggerFake = Substitute.For<ILogger>();
            _dummyClass = new DummyClass(_loggerFake);
        }

        public class AssertStaticLogCall : LoggerExtensionsExtensionsFixture
        {
            [Fact]
            public void Should_Log_Call()
            {
                Invoking(() =>
                {
                    _loggerFake.AssertStaticLogCall(
                        () =>
                        {
                            DummyClass.CallStaticMethod(_loggerFake);
                        },
                        nameof(DummyClass.CallStaticMethod),
                        LogLevel.Information);
                })
                    .Should()
                    .NotThrow();
            }
        }

        public class AssertStaticLogCallWithArguments : LoggerExtensionsExtensionsFixture
        {
            [Fact]
            public void Should_Log_Call()
            {
                Invoking(() =>
                {
                    var value1 = Create<int>();
                    var value2 = Create<string>();

                    _loggerFake.AssertStaticLogCallWithArguments(
                        () =>
                        {
                            DummyClass.CallStaticMethodWithArguments(_loggerFake, value1, value2);
                        },
                        nameof(DummyClass.CallStaticMethodWithArguments),
                        new { value1, value2 },
                        LogLevel.Information);
                }).Should().NotThrow();
            }
        }

        public class AssertLogCall : LoggerExtensionsExtensionsFixture
        {
            [Fact]
            public void Should_Log_Call()
            {
                Invoking(() =>
                {
                    _loggerFake.AssertLogCall<DummyClass>(
                        () =>
                        {
                            _dummyClass.CallMethod();
                        },
                        nameof(DummyClass.CallMethod),
                        LogLevel.Information);
                }).Should().NotThrow();
            }
        }

        public class AssertLogCallWithArguments : LoggerExtensionsExtensionsFixture
        {
            [Fact]
            public void Should_Log_Call()
            {
                Invoking(() =>
                {
                    var value1 = Create<int>();
                    var value2 = Create<string>();

                    _loggerFake.AssertLogCallWithArguments<DummyClass>(
                        () =>
                        {
                            _dummyClass.CallMethodWithArguments(value1, value2);
                        },
                        nameof(DummyClass.CallMethodWithArguments),
                        new { value1, value2 },
                        LogLevel.Information);
                }).Should().NotThrow();
            }
        }

        public class AssertLogCallAsync : LoggerExtensionsExtensionsFixture
        {
            [Fact]
            public async Task Should_Log_Call()
            {
                await Invoking(async () =>
                {
                    await _loggerFake.AssertLogCallAsync<DummyClass>(
                        async () =>
                        {
                            await _dummyClass.CallMethodAsync();
                        },
                        nameof(DummyClass.CallMethodAsync),
                        LogLevel.Information);
                })
                    .Should()
                    .NotThrowAsync();
            }
        }

        public class AssertLogCallWithArgumentsAsync : LoggerExtensionsExtensionsFixture
        {
            [Fact]
            public async Task Should_Log_Call()
            {
                await Invoking(async () =>
                {
                    var value1 = Create<int>();
                    var value2 = Create<string>();

                    await _loggerFake.AssertLogCallWithArgumentsAsync<DummyClass>(
                        async () =>
                        {
                            await _dummyClass.CallMethodWithArgumentsAsync(value1, value2);
                        },
                        nameof(DummyClass.CallMethodWithArgumentsAsync),
                        new { value1, value2 },
                        LogLevel.Information);

                })
                    .Should()
                    .NotThrowAsync();
            }
        }

        public class CaptureLogCalls : LoggerExtensionsExtensionsFixture
        {
            [Fact]
            public void Should_Log_Message()
            {
                var message = Create<string>();

                Invoking(() =>
                {
                    var methodCallContext = _loggerFake.CaptureLogCalls(() =>
                    {
                        _dummyClass.LogDebugMethod(message);
                    });

                    methodCallContext.AssertMessageEntry(0, message, LogLevel.Debug);
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Log_Message_With_Arguments()
            {
                var logTemplate = "Message = {Value1} and {Value2}";

                var value1 = Create<int>();
                var value2 = Create<string>();

                Invoking(() =>
                {
                    var methodCallContext = _loggerFake.CaptureLogCalls(() =>
                    {
                        _dummyClass.LogInformationMethodWithArguments(logTemplate, value1, value2);
                    });

                    methodCallContext.AssertMessageWithArgumentsEntry(
                        0,
                        logTemplate,
                        new { Value1 = value1, Value2 = value2 },
                        LogLevel.Information);
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Log_Message_With_Destructured_Argument()
            {
                var logTemplate = "Message = {Value1} and {@Value2}";

                var value1 = Create<int>();
                var value2 = Create<string>();

                Invoking(() =>
                {
                    var methodCallContext = _loggerFake.CaptureLogCalls(() =>
                    {
                        _dummyClass.LogInformationMethodWithArguments(logTemplate, value1, value2);
                    });

                    var expectedArguments = new Dictionary<string, object>
                    {
                        { "Value1", value1 },
                        { "@Value2", value2 }
                    };

                    methodCallContext.AssertMessageWithArgumentsEntry(
                        0,
                        logTemplate,
                        expectedArguments,
                        LogLevel.Information);
                })
                    .Should()
                    .NotThrow();
            }

            [Fact]
            public void Should_Capture_Call_Logs()
            {
                Invoking(() =>
                {
                    var value1 = Create<int>();
                    var value2 = Create<string>();
                    var exception = new Exception(Create<string>());

                    var methodCallContext = _loggerFake.CaptureLogCalls(() =>
                    {
                        _dummyClass.CallMethod();
                        _dummyClass.CallMethodWithArguments(value1, value2);
                        _dummyClass.CallMethodWithException(exception);
                    });

                    methodCallContext.AssertLogCallEntry<DummyClass>(
                        0,
                        nameof(DummyClass.CallMethod),
                        LogLevel.Information);

                    methodCallContext.AssertLogCallWithArgumentsEntry<DummyClass>(
                        1,
                        nameof(DummyClass.CallMethodWithArguments),
                        new { value1, value2 },
                        LogLevel.Information);

                    methodCallContext.AssertExceptionLogEntry(2, exception);
                })
                    .Should()
                    .NotThrow();
            }
        }

        public class CaptureLogCallsAsync : LoggerExtensionsExtensionsFixture
        {
            [Fact]
            public async Task Should_Capture_Call_Logs()
            {
                await Invoking(async () =>
                {
                    var logTemplate = "{Value1} and {Value2}";
                    var value1 = Create<int>();
                    var value2 = Create<string>();
                    var exception = new Exception(Create<string>());

                    var methodCallContext = await _loggerFake.CaptureLogCallsAsync(async () =>
                    {
                        await _dummyClass.CallMethodAsync();
                        await _dummyClass.CallMethodWithArgumentsAsync(value1, value2);
                        _dummyClass.CallMethodWithException(exception);
                        _dummyClass.CallMethodWithExceptionAndArguments(exception, logTemplate, value1, value2);
                    });

                    methodCallContext.Should().HaveCount(4);

                    methodCallContext.AssertLogCallEntry<DummyClass>(
                        0,
                        nameof(DummyClass.CallMethodAsync),
                        LogLevel.Information);

                    methodCallContext.AssertLogCallWithArgumentsEntry<DummyClass>(
                        1,
                        nameof(DummyClass.CallMethodWithArgumentsAsync),
                        new { value1, value2 },
                        LogLevel.Information);

                    methodCallContext.AssertExceptionLogEntry(2, exception);

                    methodCallContext.AssertExceptionWithArgumentsLogEntry(3, exception, logTemplate, new { Value1 = value1, Value2 = value2 });
                })
                    .Should()
                    .NotThrowAsync();
            }
        }
    }
}