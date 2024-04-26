using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Fixture;

namespace AllOverIt.Evaluator.Tests.Exceptions
{
    public class OperatorExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<OperatorException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<OperatorException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<OperatorException>();
        }
    }
}