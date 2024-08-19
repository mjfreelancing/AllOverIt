using AllOverIt.Fixture;
using FluentAssertions;

internal sealed class DummyClassNoNamespace
{
}

namespace AllOverIt.Logging.Tests
{
    public class LogCallOptionsFixture : FixtureBase
    {
        public LogCallOptionsFixture()
        {
            // Restore state between each test
            LogCallOptions.Instance.Reset();
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
            public void Should_Throw_When_Prefix_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        LogCallOptions.UseCallPrefix(stringValue);
                    }, "callPrefix");
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
            public void Should_Throw_When_Prefix_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        LogCallOptions.UseExceptionPrefix(stringValue);
                    }, "exceptionPrefix");
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
            public void Should_Throw_When_MethodName_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        LogCallOptions.UseMethodNameProperty(stringValue);
                    }, "methodNameProperty");
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
            public void Should_Throw_When_ExceptionMessage_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        LogCallOptions.UseExceptionMessageProperty(stringValue);
                    }, "exceptionMessageProperty");
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
            public void Should_Throw_When_Prefix_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        LogCallOptions.UseArgumentsPrefix(stringValue);
                    }, "argumentsPrefix");
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
            public void Should_Throw_When_Arguments_Null_Empty_Whitespace()
            {
                AssertThrowsWhenStringNullOrEmptyOrWhitespace(
                    stringValue =>
                    {
                        LogCallOptions.UseArgumentsDestructureProperty(stringValue);
                    }, "argumentsProperty");
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