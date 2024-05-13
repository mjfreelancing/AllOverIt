using AllOverIt.Fixture;
using AllOverIt.Logging.Extensions;
using AllOverIt.Logging.Testing.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AllOverIt.Logging.Testing.Tests.Extensions
{
    public class MethodCallsWithArgumentsExtensionsFixture : FixtureBase
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

            public void CallMethodWithException(Exception exception)
            {
                _logger.LogException(exception);
            }
        }

        private readonly DummyClass _dummyClass;
        private readonly ILogger _loggerFake;

        public MethodCallsWithArgumentsExtensionsFixture()
        {
            _loggerFake = Substitute.For<ILogger>();
            _dummyClass = new DummyClass(_loggerFake);
        }

        public class AssertStaticLogCallEntry : MethodCallsWithArgumentsExtensionsFixture
        {
            [Fact]
            public void Should_Not_Throw()
            {
                var exception = new Exception(Create<String>());

                Invoking(() =>
                {
                    var methodCallsWithArguments = _loggerFake
                        .CaptureLogCalls(() =>
                        {
                            DummyClass.CallStaticMethod(_loggerFake);
                            _dummyClass.CallMethodWithException(exception);
                        });

                    methodCallsWithArguments.Should().HaveCount(2);

                    methodCallsWithArguments.States[0].Should().NotBeNull();
                    methodCallsWithArguments.States[1].Should().NotBeNull();

                    methodCallsWithArguments.Exceptions[0].Should().BeNull();
                    methodCallsWithArguments.Exceptions[1].Should().NotBeNull();

                    methodCallsWithArguments.AssertStaticLogCallEntry(0, nameof(DummyClass.CallStaticMethod));
                    methodCallsWithArguments.AssertExceptionLogEntry(1, exception);
                })
                .Should()
                .NotThrow();
            }
        }

        public class AssertStaticLogCallWithArgumentsEntry : MethodCallsWithArgumentsExtensionsFixture
        {
            [Fact]
            public void Should_Not_Throw()
            {
                var value1 = Create<int>();
                var value2 = Create<string>();

                Invoking(() =>
                {
                    var methodCallsWithArguments = _loggerFake
                        .CaptureLogCalls(() =>
                        {
                            DummyClass.CallStaticMethodWithArguments(_loggerFake, value1, value2);
                        });

                    methodCallsWithArguments.Should().HaveCount(1);

                    methodCallsWithArguments.States[0].Should().NotBeNull();
                    methodCallsWithArguments.Exceptions[0].Should().BeNull();

                    methodCallsWithArguments.AssertStaticLogCallWithArgumentsEntry(
                        0,
                        nameof(DummyClass.CallStaticMethodWithArguments),
                        new { value1, value2 });
                })
                .Should()
                .NotThrow();
            }
        }

        public class AssertLogCallEntry : MethodCallsWithArgumentsExtensionsFixture
        {
            [Fact]
            public void Should_Not_Throw()
            {
                var exception = new Exception(Create<String>());

                Invoking(() =>
                {
                    var methodCallsWithArguments = _loggerFake
                        .CaptureLogCalls(() =>
                        {
                            _dummyClass.CallMethodWithException(exception);
                            _dummyClass.CallMethod();
                        });

                    methodCallsWithArguments.Should().HaveCount(2);

                    methodCallsWithArguments.States[0].Should().NotBeNull();
                    methodCallsWithArguments.States[1].Should().NotBeNull();

                    methodCallsWithArguments.Exceptions[0].Should().NotBeNull();
                    methodCallsWithArguments.Exceptions[1].Should().BeNull();

                    methodCallsWithArguments.AssertExceptionLogEntry(0, exception);

                    methodCallsWithArguments.AssertLogCallEntry<DummyClass>(1, nameof(DummyClass.CallMethod));
                })
                .Should()
                .NotThrow();
            }
        }

        public class AssertLogCallWithArgumentsEntry : MethodCallsWithArgumentsExtensionsFixture
        {
            [Fact]
            public void Should_Not_Throw()
            {
                var value1 = Create<int>();
                var value2 = Create<string>();

                Invoking(() =>
                {
                    var methodCallsWithArguments = _loggerFake
                        .CaptureLogCalls(() =>
                        {
                            _dummyClass.CallMethodWithArguments(value1, value2);
                        });

                    methodCallsWithArguments.Should().HaveCount(1);

                    methodCallsWithArguments.States[0].Should().NotBeNull();
                    methodCallsWithArguments.Exceptions[0].Should().BeNull();

                    methodCallsWithArguments.AssertLogCallWithArgumentsEntry<DummyClass>(
                        0,
                        nameof(DummyClass.CallMethodWithArguments),
                        new { value1, value2 });
                })
                .Should()
                .NotThrow();
            }
        }
    }
}