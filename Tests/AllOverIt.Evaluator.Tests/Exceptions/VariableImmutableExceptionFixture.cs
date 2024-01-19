using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Fixture;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Exceptions
{
    public class VariableImmutableExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<VariableImmutableException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<VariableImmutableException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<VariableImmutableException>();
        }
    }
}