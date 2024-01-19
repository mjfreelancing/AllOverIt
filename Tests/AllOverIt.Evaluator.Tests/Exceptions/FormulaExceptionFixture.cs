using AllOverIt.Evaluator.Exceptions;
using AllOverIt.Fixture;
using Xunit;

namespace AllOverIt.Evaluator.Tests.Exceptions
{
    public class FormulaExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<FormulaException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<FormulaException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<FormulaException>();
        }
    }
}