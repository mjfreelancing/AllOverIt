using AllOverIt.Cryptography.Hybrid.Exceptions;
using AllOverIt.Fixture;

namespace AllOverIt.Cryptography.Tests.Hybrid.Exceptions
{
    public class RsaAesHybridExceptionFixture : FixtureBase
    {
        [Fact]
        public void Should_Have_Default_Constructor()
        {
            AssertDefaultConstructor<RsaAesHybridException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message()
        {
            AssertConstructorWithMessage<RsaAesHybridException>();
        }

        [Fact]
        public void Should_Have_Constructor_With_Message_And_InnerException()
        {
            AssertConstructorWithMessageAndInnerException<RsaAesHybridException>();
        }
    }
}
