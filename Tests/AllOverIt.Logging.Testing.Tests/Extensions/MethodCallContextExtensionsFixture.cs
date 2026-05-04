using AllOverIt.Fixture;
using AllOverIt.Logging.Extensions;
using AllOverIt.Logging.Testing.Extensions;
using Shouldly;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AllOverIt.Logging.Testing.Tests.Extensions
{
    public class MethodCallContextExtensionsFixture : FixtureBase
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

        public MethodCallContextExtensionsFixture()
        {
            _loggerFake = Substitute.For<ILogger>();
            _dummyClass = new DummyClass(_loggerFake);
        }

        public class AssertStaticLogCallEntry : MethodCallContextExtensionsFixture
        {
            [Fact]
            public void Should_Not_Throw()
            {
                var exception = new Exception(Create<String>());

                Should.NotThrow(() =>
                {
                    var methodCallContext = _loggerFake
                        .CaptureLogCalls(() =>
                        {
                            DummyClass.CallStaticMethod(_loggerFake);
                            _dummyClass.CallMethodWithException(exception);
                        });

                    methodCallContext.Count.ShouldBe(2);

                    methodCallContext.Metadata[0].ShouldNotBeNull();
                    methodCallContext.Metadata[1].ShouldNotBeNull();

                    methodCallContext.Exceptions[0].ShouldBeNull();
                    methodCallContext.Exceptions[1].ShouldNotBeNull();

                    methodCallContext.AssertStaticLogCallEntry(
                        0,
                        nameof(DummyClass.CallStaticMethod),
                        LogLevel.Information);

                    // Creating another exception instance so it is compared using equivalence and not reference
                    methodCallContext.AssertExceptionLogEntry(1, new Exception(exception.Message));
                });
            }
        }

        public class AssertStaticLogCallWithArgumentsEntry : MethodCallContextExtensionsFixture
        {
            [Fact]
            public void Should_Not_Throw()
            {
                var value1 = Create<int>();
                var value2 = Create<string>();

                Should.NotThrow(() =>
                {
                    var methodCallContext = _loggerFake
                        .CaptureLogCalls(() =>
                        {
                            DummyClass.CallStaticMethodWithArguments(_loggerFake, value1, value2);
                        });

                    methodCallContext.Count.ShouldBe(1);

                    methodCallContext.Metadata[0].ShouldNotBeNull();
                    methodCallContext.Exceptions[0].ShouldBeNull();

                    methodCallContext.AssertStaticLogCallWithArgumentsEntry(
                        0,
                        nameof(DummyClass.CallStaticMethodWithArguments),
                        new { value1, value2 },
                        LogLevel.Information);
                });
            }
        }

        public class AssertLogCallEntry : MethodCallContextExtensionsFixture
        {
            [Fact]
            public void Should_Not_Throw()
            {
                var exception = new Exception(Create<String>());

                Should.NotThrow(() =>
                {
                    var methodCallContext = _loggerFake
                        .CaptureLogCalls(() =>
                        {
                            _dummyClass.CallMethodWithException(exception);
                            _dummyClass.CallMethod();
                        });

                    methodCallContext.Count.ShouldBe(2);

                    methodCallContext.Metadata[0].ShouldNotBeNull();
                    methodCallContext.Metadata[1].ShouldNotBeNull();

                    methodCallContext.Exceptions[0].ShouldNotBeNull();
                    methodCallContext.Exceptions[1].ShouldBeNull();

                    // Creating another exception instance so it is compared using equivalence and not reference
                    methodCallContext.AssertExceptionLogEntry(0, new Exception(exception.Message));

                    methodCallContext.AssertLogCallEntry<DummyClass>(
                        1,
                        nameof(DummyClass.CallMethod),
                        LogLevel.Information);
                });
            }
        }

        public class AssertLogCallWithArgumentsEntry : MethodCallContextExtensionsFixture
        {
            [Fact]
            public void Should_Not_Throw()
            {
                var value1 = Create<int>();
                var value2 = Create<string>();

                Should.NotThrow(() =>
                {
                    var methodCallContext = _loggerFake
                        .CaptureLogCalls(() =>
                        {
                            _dummyClass.CallMethodWithArguments(value1, value2);
                        });

                    methodCallContext.Count.ShouldBe(1);

                    methodCallContext.Metadata[0].ShouldNotBeNull();
                    methodCallContext.Exceptions[0].ShouldBeNull();

                    methodCallContext.AssertLogCallWithArgumentsEntry<DummyClass>(
                        0,
                        nameof(DummyClass.CallMethodWithArguments),
                        new { value1, value2 },
                        LogLevel.Information);
                });
            }
        }
    }
}