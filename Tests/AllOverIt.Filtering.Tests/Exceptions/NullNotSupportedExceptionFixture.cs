using AllOverIt.Filtering.Exceptions;
using AllOverIt.Fixture;
using Xunit;

namespace AllOverIt.Filtering.Tests.Exceptions
{
    public class NullNotSupportedExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<NullNotSupportedException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<NullNotSupportedException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<NullNotSupportedException>();
        }
    }
}