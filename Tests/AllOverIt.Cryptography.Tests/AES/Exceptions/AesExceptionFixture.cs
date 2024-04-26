using AllOverIt.Cryptography.AES.Exceptions;
using AllOverIt.Fixture;

namespace AllOverIt.Cryptography.Tests.AES.Exceptions
{
    public class AesExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<AesException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<AesException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<AesException>();
        }
    }
}
