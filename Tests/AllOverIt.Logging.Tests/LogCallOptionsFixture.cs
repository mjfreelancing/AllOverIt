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
                    LogCallOptions.UseCallPrefix(null!);
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
                    LogCallOptions.UseCallPrefix(string.Empty);
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
                    LogCallOptions.UseCallPrefix("   ");
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("callPrefix");
            }

            [Fact]
            public void Should_Update_LogTemplateWithNoArguments()
            {
                var callPrefix = Create<string>();

                LogCallOptions.UseCallPrefix(callPrefix);

                LogCallOptions.Instance.LogTemplateWithNoArguments.Should().Be($"{callPrefix}{{{LogCallOptions.Instance._methodNameProperty}}}");
            }

            [Fact]
            public void Should_Update_LogTemplateWithArguments()
            {
                var callPrefix = Create<string>();

                LogCallOptions.UseCallPrefix(callPrefix);

                LogCallOptions.Instance.LogTemplateWithArguments
                    .Should()
                    .Be($"{callPrefix}{{{LogCallOptions.Instance._methodNameProperty}}}, {LogCallOptions.Instance._argumentsPrefix}{{@{LogCallOptions.Instance._argumentsDestructureProperty}}}");
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
                    LogCallOptions.UseExceptionPrefix(null!);
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
                    LogCallOptions.UseExceptionPrefix(string.Empty);
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
                    LogCallOptions.UseExceptionPrefix("   ");
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("exceptionPrefix");
            }

            [Fact]
            public void Should_Update_LogExceptionTemplate()
            {
                var exceptionPrefix = Create<string>();

                LogCallOptions.UseExceptionPrefix(exceptionPrefix);

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
                    LogCallOptions.UseMethodNameProperty(null!);
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
                    LogCallOptions.UseMethodNameProperty(string.Empty);
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
                    LogCallOptions.UseMethodNameProperty("   ");
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("methodNameProperty");
            }

            [Fact]
            public void Should_Update_LogTemplateWithNoArguments()
            {
                var methodNameProperty = Create<string>();

                LogCallOptions.UseMethodNameProperty(methodNameProperty);

                LogCallOptions.Instance.LogTemplateWithNoArguments.Should().Be($"{LogCallOptions.Instance._callPrefix}{{{methodNameProperty}}}");
            }

            [Fact]
            public void Should_Update_LogTemplateWithArguments()
            {
                var methodNameProperty = Create<string>();

                LogCallOptions.UseMethodNameProperty(methodNameProperty);

                LogCallOptions.Instance.LogTemplateWithArguments
                    .Should()
                    .Be($"{LogCallOptions.Instance._callPrefix}{{{methodNameProperty}}}, {LogCallOptions.Instance._argumentsPrefix}{{@{LogCallOptions.Instance._argumentsDestructureProperty}}}");
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
                    LogCallOptions.UseExceptionMessageProperty(null!);
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
                    LogCallOptions.UseExceptionMessageProperty(string.Empty);
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
                    LogCallOptions.UseExceptionMessageProperty("   ");
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("exceptionMessageProperty");
            }

            [Fact]
            public void Should_Update_LogExceptionTemplate()
            {
                var exceptionMessageProperty = Create<string>();

                LogCallOptions.UseExceptionMessageProperty(exceptionMessageProperty);

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
                    LogCallOptions.UseArgumentsPrefix(null!);
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
                    LogCallOptions.UseArgumentsPrefix(string.Empty);
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
                    LogCallOptions.UseArgumentsPrefix("   ");
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("argumentsPrefix");
            }

            [Fact]
            public void Should_Update_LogTemplateWithArguments()
            {
                var argumentsPrefix = Create<string>();

                LogCallOptions.UseArgumentsPrefix(argumentsPrefix);

                LogCallOptions.Instance.LogTemplateWithArguments
                    .Should()
                    .Be($"{LogCallOptions.Instance._callPrefix}{{{LogCallOptions.Instance._methodNameProperty}}}, {argumentsPrefix}{{@{LogCallOptions.Instance._argumentsDestructureProperty}}}");
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
                    LogCallOptions.UseArgumentsDestructureProperty(null!);
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
                    LogCallOptions.UseArgumentsDestructureProperty(string.Empty);
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
                    LogCallOptions.UseArgumentsDestructureProperty("   ");
                })
                .Should()
                .Throw<ArgumentException>()
                .WithNamedMessageWhenEmpty("argumentsProperty");
            }

            [Fact]
            public void Should_Update_LogTemplateWithArguments()
            {
                var argumentsProperty = Create<string>();

                LogCallOptions.UseArgumentsDestructureProperty(argumentsProperty);

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
                LogCallOptions.IncludeCallerNamespace(true);

                var callerName = Create<string>();

                var actual = LogCallOptions.GetCallerFullName(LogCallOptions.Instance, callerName);

                // We can use 'FullName' since the type is not a generic
                actual.Should().Be($"{typeof(LogCallOptions).FullName}.{callerName}");
            }

            [Fact]
            public void Should_Exclude_Namespace()
            {
                LogCallOptions.IncludeCallerNamespace(false);

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
                LogCallOptions.IncludeCallerNamespace(true);

                var callerName = Create<string>();

                var actual = LogCallOptions.GetCallerFullName(LogCallOptions.Instance, callerName);

                // We can use 'FullName' since the type is not a generic
                actual.Should().Be($"{typeof(LogCallOptions).FullName}.{callerName}");
            }

            [Fact]
            public void Should_Get_CallerName_Without_Namespace()
            {
                LogCallOptions.IncludeCallerNamespace(false);

                var callerName = Create<string>();

                var actual = LogCallOptions.GetCallerFullName(LogCallOptions.Instance, callerName);

                // We can use 'Name' since the type is not a generic
                actual.Should().Be($"{typeof(LogCallOptions).Name}.{callerName}");
            }

            [Fact]
            public void Should_Get_CallerName_When_No_Namespace()
            {
                typeof(DummyClassNoNamespace).Namespace.Should().BeNull();

                LogCallOptions.IncludeCallerNamespace(true);

                var callerName = Create<string>();

                var actual = LogCallOptions.GetCallerFullName(new DummyClassNoNamespace(), callerName);

                // We can use 'Name' since the type is not a generic
                actual.Should().Be($"{typeof(DummyClassNoNamespace).Name}.{callerName}");
            }
        }
    }
}