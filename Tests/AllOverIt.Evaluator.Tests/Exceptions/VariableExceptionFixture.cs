using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Fixture;

namespace AllOverIt.Evaluator.Tests.Exceptions
{
    public class VariableExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<VariableException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<VariableException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<VariableException>();
        }
    }
}