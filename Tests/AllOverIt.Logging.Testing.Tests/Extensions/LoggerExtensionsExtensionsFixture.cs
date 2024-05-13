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
                        }, nameof(DummyClass.CallStaticMethod));
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
                        }, nameof(DummyClass.CallStaticMethodWithArguments), new { value1, value2 });
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
                        }, nameof(DummyClass.CallMethod));
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
                        }, nameof(DummyClass.CallMethodWithArguments), new { value1, value2 });
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
                        }, nameof(DummyClass.CallMethodAsync));
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
                        }, nameof(DummyClass.CallMethodWithArgumentsAsync), new { value1, value2 });

                })
                    .Should()
                    .NotThrowAsync();
            }
        }

        public class CaptureLogCalls : LoggerExtensionsExtensionsFixture
        {
            [Fact]
            public void Should_Capture_Log_Calls()
            {
                Invoking(() =>
                {
                    var value1 = Create<int>();
                    var value2 = Create<string>();
                    var exception = new Exception(Create<string>());

                    var methodCallsWithArguments = _loggerFake.CaptureLogCalls(() =>
                    {
                        _dummyClass.CallMethod();
                        _dummyClass.CallMethodWithArguments(value1, value2);
                        _dummyClass.CallMethodWithException(exception);
                    });

                    methodCallsWithArguments.AssertLogCallEntry<DummyClass>(0, nameof(DummyClass.CallMethod));

                    methodCallsWithArguments.AssertLogCallWithArgumentsEntry<DummyClass>(1, nameof(DummyClass.CallMethodWithArguments), new { value1, value2 });

                    methodCallsWithArguments.AssertExceptionLogEntry(2, exception);
                }).Should().NotThrow();
            }
        }

        public class CaptureLogCallsAsync : LoggerExtensionsExtensionsFixture
        {
            [Fact]
            public async Task Should_Capture_Log_Calls()
            {
                await Invoking(async () =>
                {
                    var value1 = Create<int>();
                    var value2 = Create<string>();
                    var exception = new Exception(Create<string>());

                    var methodCallsWithArguments = await _loggerFake.CaptureLogCallsAsync(async () =>
                    {
                        await _dummyClass.CallMethodAsync();
                        await _dummyClass.CallMethodWithArgumentsAsync(value1, value2);
                        _dummyClass.CallMethodWithException(exception);
                    });

                    methodCallsWithArguments.AssertLogCallEntry<DummyClass>(0, nameof(DummyClass.CallMethodAsync));

                    methodCallsWithArguments.AssertLogCallWithArgumentsEntry<DummyClass>(
                        1,
                        nameof(DummyClass.CallMethodWithArgumentsAsync),
                        new { value1, value2 });

                    methodCallsWithArguments.AssertExceptionLogEntry(2, exception);
                })
                    .Should()
                    .NotThrowAsync();
            }
        }
    }
}