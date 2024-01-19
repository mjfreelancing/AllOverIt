using AllOverIt.Fixture;
using AllOverIt.Serialization.Binary.Exceptions;
using Xunit;

namespace AllOverIt.Serialization.Binary.Tests.Exceptions
{
    public class BinaryWriterExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<BinaryWriterException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<BinaryWriterException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<BinaryWriterException>();
        }
    }
}