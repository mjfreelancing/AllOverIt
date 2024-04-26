using AllOverIt.Fixture;
using AllOverIt.Serialization.Json.Abstractions.Exceptions;

namespace AllOverIt.Serialization.Json.Abstractions.Tests.Exceptions
{
    public class JsonHelperExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<JsonHelperException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<JsonHelperException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<JsonHelperException>();
        }
    }
}