using AllOverIt.DependencyInjection.Exceptions;
using AllOverIt.Fixture;
using Xunit;

namespace AllOverIt.DependencyInjection.Tests.Exceptions
{
    public class DependencyRegistrationExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<DependencyRegistrationException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<DependencyRegistrationException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<DependencyRegistrationException>();
        }
    }
}