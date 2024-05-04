using AllOverIt.Fixture;
using AllOverIt.Serialization.Binary.Exceptions;

namespace AllOverIt.Serialization.Binary.Tests.Exceptions
{
    public class BinaryReaderExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<BinaryReaderException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<BinaryReaderException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<BinaryReaderException>();
        }
    }
}