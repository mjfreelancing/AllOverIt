using AllOverIt.Fixture;
using AllOverIt.Formatters.Objects.Exceptions;
using Xunit;

namespace AllOverIt.Tests.Formatters.Objects.Exceptions
{
    public class ObjectPropertyFilterExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<ObjectPropertyFilterException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<ObjectPropertyFilterException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<ObjectPropertyFilterException>();
        }
    }
}