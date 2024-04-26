using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Fixture;

namespace AllOverIt.Evaluator.Tests.Exceptions
{
    public class OperationFactoryExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<OperationFactoryException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<OperationFactoryException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<OperationFactoryException>();
        }
    }
}