using AllOverIt.Fixture;
using AllOverIt.Mapping.Exceptions;
using Xunit;

namespace AllOverIt.Mapping.Tests.Exceptions
{
    public class ObjectMapperExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<ObjectMapperException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<ObjectMapperException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<ObjectMapperException>();
        }
    }
}