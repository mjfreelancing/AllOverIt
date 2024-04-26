using AllOverIt.Fixture;
using AllOverIt.Process.Exceptions;

namespace AllOverIt.Tests.Process.Exceptions
{
    public class ProcessExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<ProcessException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<ProcessException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<ProcessException>();
        }
    }
}