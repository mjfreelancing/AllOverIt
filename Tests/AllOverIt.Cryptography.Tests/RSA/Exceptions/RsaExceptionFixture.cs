using AllOverIt.Cryptography.RSA.Exceptions;
using AllOverIt.Fixture;

namespace AllOverIt.Cryptography.Tests.RSA.Exceptions
{
    public class RsaExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<RsaException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<RsaException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<RsaException>();
        }
    }
}
