using AllOverIt.Fixture;
using AllOverIt.Validation.Exceptions;

namespace AllOverIt.Validation.Tests.Exceptions
{
    public class ValidationRegistryExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<ValidationRegistryException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<ValidationRegistryException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<ValidationRegistryException>();
        }
    }
}