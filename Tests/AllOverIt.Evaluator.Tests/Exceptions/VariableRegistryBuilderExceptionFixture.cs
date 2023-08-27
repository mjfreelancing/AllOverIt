using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Fixture;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Exceptions
{
    public class VariableRegistryBuilderExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<VariableRegistryBuilderException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<VariableRegistryBuilderException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<VariableRegistryBuilderException>();
        }
    }
}