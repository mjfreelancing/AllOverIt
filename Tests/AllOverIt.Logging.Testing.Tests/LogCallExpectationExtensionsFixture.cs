using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

namespace AllOverIt.Logging.Testing.Tests
{
    public class LogCallExpectationExtensionsFixture : FixtureBase
    {
        private const string OriginalFormat = "{OriginalFormat}";

        private static readonly string MethodNameProperty = LogCallOptions.Instance._methodNameProperty;
        private static readonly string ArgumentsProperty = LogCallOptions.Instance._argumentsDestructureProperty;

        internal sealed class DummyClass;

        public class GetExpectedStaticLogCallMetadata : LogCallExpectationExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_CallerName_Null()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedStaticLogCallMetadata(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("callerName");
            }

            [Fact]
            public void Should_Throw_When_CallerName_Empty()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedStaticLogCallMetadata(string.Empty);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("callerName");
            }

            [Fact]
            public void Should_Throw_When_CallerName_Whitespace()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedStaticLogCallMetadata("  ");
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("callerName");
            }

            [Fact]
            public void Should_Return_Expectation()
            {
                var callerName = Create<string>();

                var actual = LogCallExpectation.GetExpectedStaticLogCallMetadata(callerName);

                actual.Should().BeEquivalentTo(new Dictionary<string, object>
                {
                    { MethodNameProperty, callerName },
                    { OriginalFormat, LogCallOptions.Instance.LogTemplateWithNoArguments }
                });
            }
        }

        public class GetExpectedLogCallMetadata : LogCallExpectationExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_CallerName_Null()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedLogCallMetadata<DummyClass>(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("callerName");
            }

            [Fact]
            public void Should_Throw_When_CallerName_Empty()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedLogCallMetadata<DummyClass>(string.Empty);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("callerName");
            }

            [Fact]
            public void Should_Throw_When_CallerName_Whitespace()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedLogCallMetadata<DummyClass>("  ");
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("callerName");
            }

            [Fact]
            public void Should_Return_Expectation()
            {
                var callerName = Create<string>();

                var actual = LogCallExpectation.GetExpectedLogCallMetadata<DummyClass>(callerName);

                var callerType = typeof(DummyClass);

                actual.Should().BeEquivalentTo(new Dictionary<string, object>
                {
                    { MethodNameProperty, $"{callerType.Namespace}.{callerType.GetFriendlyName()}.{callerName}" },
                    { OriginalFormat, LogCallOptions.Instance.LogTemplateWithNoArguments }
                });
            }
        }

        public class GetExpectedStaticLogCallWithArgumentsMetadata : LogCallExpectationExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_CallerName_Null()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedStaticLogCallWithArgumentsMetadata(null, new { });
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("callerName");
            }

            [Fact]
            public void Should_Throw_When_CallerName_Empty()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedStaticLogCallWithArgumentsMetadata(string.Empty, new { });
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("callerName");
            }

            [Fact]
            public void Should_Throw_When_CallerName_Whitespace()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedStaticLogCallWithArgumentsMetadata("  ", new { });
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("callerName");
            }

            [Fact]
            public void Should_Throw_When_Arguments_Null()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedStaticLogCallWithArgumentsMetadata(Create<string>(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("arguments");
            }

            [Fact]
            public void Should_Return_Expectation()
            {
                var callerName = Create<string>();
                var arguments = new { arg1 = Create<string>(), arg2 = Create<int>() };

                var actual = LogCallExpectation.GetExpectedStaticLogCallWithArgumentsMetadata(callerName, arguments);

                actual.Should().BeEquivalentTo(new Dictionary<string, object>
                {
                    { MethodNameProperty, callerName},
                    { $"@{ArgumentsProperty}", arguments },
                    { OriginalFormat, LogCallOptions.Instance.LogTemplateWithArguments }
                });
            }
        }

        public class GetExpectedLogCallWithArgumentsMetadata : LogCallExpectationExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_CallerName_Null()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedLogCallWithArgumentsMetadata<DummyClass>(null, new { });
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("callerName");
            }

            [Fact]
            public void Should_Throw_When_CallerName_Empty()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedLogCallWithArgumentsMetadata<DummyClass>(string.Empty, new { });
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("callerName");
            }

            [Fact]
            public void Should_Throw_When_CallerName_Whitespace()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedLogCallWithArgumentsMetadata<DummyClass>("  ", new { });
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("callerName");
            }

            [Fact]
            public void Should_Throw_When_Arguments_Null()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedLogCallWithArgumentsMetadata<DummyClass>(Create<string>(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("arguments");
            }

            [Fact]
            public void Should_Return_Expectation()
            {
                var callerName = Create<string>();
                var arguments = new { arg1 = Create<string>(), arg2 = Create<int>() };

                var actual = LogCallExpectation.GetExpectedLogCallWithArgumentsMetadata<DummyClass>(callerName, arguments);

                var callerType = typeof(DummyClass);

                actual.Should().BeEquivalentTo(new Dictionary<string, object>
                {
                    { MethodNameProperty, $"{callerType.Namespace}.{callerType.GetFriendlyName()}.{callerName}" },
                    { $"@{ArgumentsProperty}", arguments },
                    { OriginalFormat, LogCallOptions.Instance.LogTemplateWithArguments }
                });
            }
        }

        public class GetExpectedExceptionMetadata : LogCallExpectationExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Exception_Null()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedExceptionMetadata(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("exception");
            }

            [Fact]
            public void Should_Return_Expectation()
            {
                var exception = new Exception(Create<string>());

                var actual = LogCallExpectation.GetExpectedExceptionMetadata(exception);

                actual.Should().BeEquivalentTo(new Dictionary<string, object>
                {
                    { LogCallOptions.Instance._exceptionMessageProperty, exception.Message },
                    { OriginalFormat, LogCallOptions.Instance.LogExceptionTemplate }
                });
            }
        }

        public class GetExpectedLogMessageMetadata : LogCallExpectationExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Message_Null()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedLogMessageMetadata(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("message");
            }

            [Fact]
            public void Should_Throw_When_Message_Empty()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedLogMessageMetadata(string.Empty);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("message");
            }

            [Fact]
            public void Should_Throw_When_Message_Whitespace()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedLogMessageMetadata("  ");
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("message");
            }

            [Fact]
            public void Should_Return_Expectation()
            {
                var message = Create<string>();

                var actual = LogCallExpectation.GetExpectedLogMessageMetadata(message);

                actual.Should().BeEquivalentTo(new Dictionary<string, object>
                {
                    { OriginalFormat, message }
                });
            }
        }

        public class GetExpectedLogTemplateWithArgumentsMetadata : LogCallExpectationExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_LogTemplate_Null()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedLogTemplateWithArgumentsMetadata(null, new { });
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("logTemplate");
            }

            [Fact]
            public void Should_Throw_When_LogTemplate_Empty()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedLogTemplateWithArgumentsMetadata(string.Empty, new { });
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("logTemplate");
            }

            [Fact]
            public void Should_Throw_When_LogTemplate_Whitespace()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedLogTemplateWithArgumentsMetadata("  ", new { });
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("logTemplate");
            }

            [Fact]
            public void Should_Throw_When_Arguments_Null()
            {
                Invoking(() =>
                {
                    _ = LogCallExpectation.GetExpectedLogTemplateWithArgumentsMetadata(Create<string>(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("arguments");
            }

            [Fact]
            public void Should_Return_Expectation()
            {
                var logTemplate = "{Value1} and {Value2}";
                var arguments = new { Value1 = Create<string>(), Value2 = Create<int>() };

                var actual = LogCallExpectation.GetExpectedLogTemplateWithArgumentsMetadata(logTemplate, arguments);

                var expected = arguments.ToPropertyDictionary();

                expected.Add(OriginalFormat, logTemplate);

                actual.Should().BeEquivalentTo(expected);
            }
        }
    }
}