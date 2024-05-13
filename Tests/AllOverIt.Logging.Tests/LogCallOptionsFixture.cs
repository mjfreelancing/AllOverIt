using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;

internal sealed class DummyClassNoNamespace
{
}

namespace AllOverIt.Logging.Tests
{
    public class LogCallOptionsFixture : FixtureBase
    {
        private const string _callPrefixDefault = "Call: ";
        private const string _exceptionPrefixDefault = "Error: ";
        private const string _methodNamePropertyDefault = "MethodName";
        private const string _exceptionMessagePropertyDefault = "ErrorMessage";
        private const string _argumentsPrefixDefault = "Arguments = ";
        private const string _argumentsPropertyDefault = "Arguments";

        public LogCallOptionsFixture()
        {
            // Restore state between each test
            LogCallOptions.UseCallPrefix(_callPrefixDefault);
            LogCallOptions.UseExceptionPrefix(_exceptionPrefixDefault);
            LogCallOptions.UseMethodNameProperty(_methodNamePropertyDefault);
            LogCallOptions.UseExceptionMessageProperty(_exceptionMessagePropertyDefault);
            LogCallOptions.UseArgumentsPrefix(_argumentsPrefixDefault);
            LogCallOptions.UseArgumentsDestructureProperty(_argumentsPropertyDefault);
            LogCallOptions.IncludeCallerNamespace(true);
        }

        [Collection("LogCall")]
        public class LogTemplateWithNoArguments : LogCallOptionsFixture
        {
            [Fact]
            public void Should_Return_Expected_Default()
            {
                var actual = LogCallOptions.Instance.LogTemplateWithNoArguments;

                actual.Should().Be("Call: {MethodName}");
            }
        }

        [Collection("LogCall")]
        public class LogTemplateWithArguments : LogCallOptionsFixture
        {
            [Fact]
            public void Should_Return_Expected_Default()
            {
                var actual = LogCallOptions.Instance.LogTemplateWithArguments;

                actual.Should().Be("Call: {MethodName}, Arguments = {@Arguments}");
            }
        }

        [Collection("LogCall")]
        public class LogExceptionTemplate : LogCallOptionsFixture
        {
            [Fact]
            public void Should_Return_Expected_Default()
            {
                var actual = LogCallOptions.Instance.LogExceptionTemplate;

                actual.Should().Be("Error: {ErrorMessage}");
            }
        }

        [Collection("LogCall")]
        public class UseCallPrefix : LogCallOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prefix_Null()
            {
                Invoking(() =>
                {
                    _ = LogCallOptions.UseCallPrefix(null!);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("callPrefix");
            }

            [Fact]
            public void Should_Throw_When_Prefix_Empty()
            {
                Invoking(() =>
                {
                    _ = LogCallOptions.UseCallPrefix(string.Empty);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("callPrefix");
            }

            [Fact]
            public void Should_Throw_When_Prefix_Whitespace()
            {
                Invoking(() =>
                {
                    _ = LogCallOptions.UseCallPrefix("   ");
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("callPrefix");
            }

            [Fact]
            public void Should_Return_Same_LogCallOptions()
            {
                var actual1 = LogCallOptions.UseCallPrefix(Create<string>());
                var actual2 = LogCallOptions.UseCallPrefix(Create<string>());

                actual1.Should().BeOfType<LogCallOptions>();

                actual1.Should().BeSameAs(actual2);
            }

            [Fact]
            public void Should_Update_LogTemplateWithNoArguments()
            {
                var callPrefix = Create<string>();

                _ = LogCallOptions.UseCallPrefix(callPrefix);

                LogCallOptions.Instance.LogTemplateWithNoArguments.Should().Be($"{callPrefix}{{{LogCallOptions.Instance._methodNameProperty}}}");
            }

            [Fact]
            public void Should_Update_LogTemplateWithArguments()
            {
                var callPrefix = Create<string>();

                _ = LogCallOptions.UseCallPrefix(callPrefix);

                LogCallOptions.Instance.LogTemplateWithArguments
                    .Should()
                    .Be($"{callPrefix}{{{LogCallOptions.Instance._methodNameProperty}}}, {LogCallOptions.Instance._argumentsPrefix}{{@{LogCallOptions.Instance._argumentsProperty}}}");
            }
        }

        [Collection("LogCall")]
        public class UseExceptionPrefix : LogCallOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prefix_Null()
            {
                Invoking(() =>
                {
                    _ = LogCallOptions.UseExceptionPrefix(null!);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("exceptionPrefix");
            }

            [Fact]
            public void Should_Throw_When_Prefix_Empty()
            {
                Invoking(() =>
                {
                    _ = LogCallOptions.UseExceptionPrefix(string.Empty);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("exceptionPrefix");
            }

            [Fact]
            public void Should_Throw_When_Prefix_Whitespace()
            {
                Invoking(() =>
                {
                    _ = LogCallOptions.UseExceptionPrefix("   ");
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("exceptionPrefix");
            }

            [Fact]
            public void Should_Return_Same_LogCallOptions()
            {
                var actual1 = LogCallOptions.UseExceptionPrefix(Create<string>());
                var actual2 = LogCallOptions.UseExceptionPrefix(Create<string>());

                actual1.Should().BeOfType<LogCallOptions>();

                actual1.Should().BeSameAs(actual2);
            }

            [Fact]
            public void Should_Update_LogExceptionTemplate()
            {
                var exceptionPrefix = Create<string>();

                _ = LogCallOptions.UseExceptionPrefix(exceptionPrefix);

                LogCallOptions.Instance.LogExceptionTemplate.Should().Be($"{exceptionPrefix}{{{LogCallOptions.Instance._exceptionMessageProperty}}}");
            }
        }

        [Collection("LogCall")]
        public class UseMethodNameProperty : LogCallOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_MethodName_Null()
            {
                Invoking(() =>
                {
                    _ = LogCallOptions.UseMethodNameProperty(null!);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("methodNameProperty");
            }

            [Fact]
            public void Should_Throw_When_MethodName_Empty()
            {
                Invoking(() =>
                {
                    _ = LogCallOptions.UseMethodNameProperty(string.Empty);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("methodNameProperty");
            }

            [Fact]
            public void Should_Throw_When_MethodName_Whitespace()
            {
                Invoking(() =>
                {
                    _ = LogCallOptions.UseMethodNameProperty("   ");
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("methodNameProperty");
            }

            [Fact]
            public void Should_Return_Same_LogCallOptions()
            {
                var actual1 = LogCallOptions.UseMethodNameProperty(Create<string>());
                var actual2 = LogCallOptions.UseMethodNameProperty(Create<string>());

                actual1.Should().BeOfType<LogCallOptions>();

                actual1.Should().BeSameAs(actual2);
            }

            [Fact]
            public void Should_Update_LogTemplateWithNoArguments()
            {
                var methodNameProperty = Create<string>();

                _ = LogCallOptions.UseMethodNameProperty(methodNameProperty);

                LogCallOptions.Instance.LogTemplateWithNoArguments.Should().Be($"{LogCallOptions.Instance._callPrefix}{{{methodNameProperty}}}");
            }

            [Fact]
            public void Should_Update_LogTemplateWithArguments()
            {
                var methodNameProperty = Create<string>();

                _ = LogCallOptions.UseMethodNameProperty(methodNameProperty);

                LogCallOptions.Instance.LogTemplateWithArguments
                    .Should()
                    .Be($"{LogCallOptions.Instance._callPrefix}{{{methodNameProperty}}}, {LogCallOptions.Instance._argumentsPrefix}{{@{LogCallOptions.Instance._argumentsProperty}}}");
            }
        }

        [Collection("LogCall")]
        public class UseExceptionMessageProperty : LogCallOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_ExceptionMessage_Null()
            {
                Invoking(() =>
                {
                    _ = LogCallOptions.UseExceptionMessageProperty(null!);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("exceptionMessageProperty");
            }

            [Fact]
            public void Should_Throw_When_ExceptionMessage_Empty()
            {
                Invoking(() =>
                {
                    _ = LogCallOptions.UseExceptionMessageProperty(string.Empty);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("exceptionMessageProperty");
            }

            [Fact]
            public void Should_Throw_When_ExceptionMessage_Whitespace()
            {
                Invoking(() =>
                {
                    _ = LogCallOptions.UseExceptionMessageProperty("   ");
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("exceptionMessageProperty");
            }

            [Fact]
            public void Should_Return_Same_LogCallOptions()
            {
                var actual1 = LogCallOptions.UseExceptionMessageProperty(Create<string>());
                var actual2 = LogCallOptions.UseExceptionMessageProperty(Create<string>());

                actual1.Should().BeOfType<LogCallOptions>();

                actual1.Should().BeSameAs(actual2);
            }

            [Fact]
            public void Should_Update_LogExceptionTemplate()
            {
                var exceptionMessageProperty = Create<string>();

                _ = LogCallOptions.UseExceptionMessageProperty(exceptionMessageProperty);

                LogCallOptions.Instance.LogExceptionTemplate.Should().Be($"{LogCallOptions.Instance._exceptionPrefix}{{{exceptionMessageProperty}}}");
            }
        }

        [Collection("LogCall")]
        public class UseArgumentsPrefix : LogCallOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_Prefix_Null()
            {
                Invoking(() =>
                {
                    _ = LogCallOptions.UseArgumentsPrefix(null!);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("argumentsPrefix");
            }

            [Fact]
            public void Should_Throw_When_Prefix_Empty()
            {
                Invoking(() =>
                {
                    _ = LogCallOptions.UseArgumentsPrefix(string.Empty);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("argumentsPrefix");
            }

            [Fact]
            public void Should_Throw_When_Prefix_Whitespace()
            {
                Invoking(() =>
                {
                    _ = LogCallOptions.UseArgumentsPrefix("   ");
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("argumentsPrefix");
            }

            [Fact]
            public void Should_Return_Same_LogCallOptions()
            {
                var actual1 = LogCallOptions.UseArgumentsPrefix(Create<string>());
                var actual2 = LogCallOptions.UseArgumentsPrefix(Create<string>());

                actual1.Should().BeOfType<LogCallOptions>();

                actual1.Should().BeSameAs(actual2);
            }

            [Fact]
            public void Should_Update_LogTemplateWithArguments()
            {
                var argumentsPrefix = Create<string>();

                _ = LogCallOptions.UseArgumentsPrefix(argumentsPrefix);

                LogCallOptions.Instance.LogTemplateWithArguments
                    .Should()
                    .Be($"{LogCallOptions.Instance._callPrefix}{{{LogCallOptions.Instance._methodNameProperty}}}, {argumentsPrefix}{{@{LogCallOptions.Instance._argumentsProperty}}}");
            }
        }

        [Collection("LogCall")]
        public class UseArgumentsDestructureProperty : LogCallOptionsFixture
        {
            [Fact]
            public void Should_Throw_When_Arguments_Null()
            {
                Invoking(() =>
                {
                    _ = LogCallOptions.UseArgumentsDestructureProperty(null!);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("argumentsProperty");
            }

            [Fact]
            public void Should_Throw_When_Arguments_Empty()
            {
                Invoking(() =>
                {
                    _ = LogCallOptions.UseArgumentsDestructureProperty(string.Empty);
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("argumentsProperty");
            }

            [Fact]
            public void Should_Throw_When_Arguments_Whitespace()
            {
                Invoking(() =>
                {
                    _ = LogCallOptions.UseArgumentsDestructureProperty("   ");
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("argumentsProperty");
            }

            [Fact]
            public void Should_Return_Same_LogCallOptions()
            {
                var actual1 = LogCallOptions.UseArgumentsDestructureProperty(Create<string>());
                var actual2 = LogCallOptions.UseArgumentsDestructureProperty(Create<string>());

                actual1.Should().BeOfType<LogCallOptions>();

                actual1.Should().BeSameAs(actual2);
            }

            [Fact]
            public void Should_Update_LogTemplateWithArguments()
            {
                var argumentsProperty = Create<string>();

                _ = LogCallOptions.UseArgumentsDestructureProperty(argumentsProperty);

                LogCallOptions.Instance.LogTemplateWithArguments
                    .Should()
                    .Be($"{LogCallOptions.Instance._callPrefix}{{{LogCallOptions.Instance._methodNameProperty}}}, {LogCallOptions.Instance._argumentsPrefix}{{@{argumentsProperty}}}");
            }
        }

        [Collection("LogCall")]
        public class IncludeCallerNamespace : LogCallOptionsFixture
        {
            [Fact]
            public void Should_Include_Namespace()
            {
                _ = LogCallOptions.IncludeCallerNamespace(true);

                var callerName = Create<string>();

                var actual = LogCallOptions.GetCallerFullName(LogCallOptions.Instance, callerName);

                // We can use 'FullName' since the type is not a generic
                actual.Should().Be($"{typeof(LogCallOptions).FullName}.{callerName}");
            }

            [Fact]
            public void Should_Exclude_Namespace()
            {
                _ = LogCallOptions.IncludeCallerNamespace(false);

                var callerName = Create<string>();

                var actual = LogCallOptions.GetCallerFullName(LogCallOptions.Instance, callerName);

                // We can use 'Name' since the type is not a generic
                actual.Should().Be($"{typeof(LogCallOptions).Name}.{callerName}");
            }
        }

        [Collection("LogCall")]
        public class GetCallerFullName : LogCallOptionsFixture
        {
            [Fact]
            public void Should_Return_CallerName_When_Caller_Null()
            {
                var callerName = Create<string>();

                var actual = LogCallOptions.GetCallerFullName(null, callerName);

                actual.Should().Be(callerName);
            }
            [Fact]
            public void Should_Get_CallerName_With_Namespace()
            {
                _ = LogCallOptions.IncludeCallerNamespace(true);

                var callerName = Create<string>();

                var actual = LogCallOptions.GetCallerFullName(LogCallOptions.Instance, callerName);

                // We can use 'FullName' since the type is not a generic
                actual.Should().Be($"{typeof(LogCallOptions).FullName}.{callerName}");
            }

            [Fact]
            public void Should_Get_CallerName_Without_Namespace()
            {
                _ = LogCallOptions.IncludeCallerNamespace(false);

                var callerName = Create<string>();

                var actual = LogCallOptions.GetCallerFullName(LogCallOptions.Instance, callerName);

                // We can use 'Name' since the type is not a generic
                actual.Should().Be($"{typeof(LogCallOptions).Name}.{callerName}");
            }

            [Fact]
            public void Should_Get_CallerName_When_No_Namespace()
            {
                typeof(DummyClassNoNamespace).Namespace.Should().BeNull();

                _ = LogCallOptions.IncludeCallerNamespace(true);

                var callerName = Create<string>();

                var actual = LogCallOptions.GetCallerFullName(new DummyClassNoNamespace(), callerName);

                // We can use 'Name' since the type is not a generic
                actual.Should().Be($"{typeof(DummyClassNoNamespace).Name}.{callerName}");
            }
        }
    }
}