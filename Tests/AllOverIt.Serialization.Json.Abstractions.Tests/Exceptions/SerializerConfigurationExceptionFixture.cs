using AllOverIt.Fixture;
using AllOverIt.Serialization.Json.Abstractions.Exceptions;

namespace AllOverIt.Serialization.Json.Abstractions.Tests.Exceptions
{
    public class SerializerConfigurationExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<SerializerConfigurationException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<SerializerConfigurationException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<SerializerConfigurationException>();
        }
    }
}