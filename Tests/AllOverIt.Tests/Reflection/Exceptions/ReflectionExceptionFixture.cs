using AllOverIt.Fixture;
using AllOverIt.Reflection.Exceptions;
using Xunit;

namespace AllOverIt.Tests.Reflection.Exceptions
{
    public class ReflectionExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<ReflectionException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<ReflectionException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<ReflectionException>();
        }
    }
}