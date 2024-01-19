using AllOverIt.Cryptography.AES;
using AllOverIt.Fixture;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Cryptography.Tests.AES
{
    public class AesEncryptorFactoryFixture : FixtureBase
    {
        private readonly AesEncryptorFactory _factory = new();

        public class Create : AesEncryptorFactoryFixture
        {
            [Fact]
            public void Should_Create_AesEncryptor()
            {
                var actual = _factory.Create();

                actual.Should().BeOfType<AesEncryptor>();
            }
        }

        public class Create_Key_IV : AesEncryptorFactoryFixture
        {
            [Fact]
            public void Should_Create_AesEncryptor()
            {
                var key = AesUtils.GenerateKey(256);
                var iv = AesUtils.GenerateIV();

                var actual = _factory.Create(key, iv);

                actual.Should().BeOfType<AesEncryptor>();
                actual.Configuration.Key.Should().BeEquivalentTo(key);
                actual.Configuration.IV.Should().BeEquivalentTo(iv);
            }
        }
    }
}
