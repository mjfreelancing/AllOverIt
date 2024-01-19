using AllOverIt.Fixture;
using AllOverIt.Plugin.Exceptions;
using Xunit;

namespace AllOverIt.Tests.Plugin.Exceptions
{
    public class CannotLoadPluginTypeExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<CannotLoadPluginTypeException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<CannotLoadPluginTypeException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<CannotLoadPluginTypeException>();
        }
    }
}