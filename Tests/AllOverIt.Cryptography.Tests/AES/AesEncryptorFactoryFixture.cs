using AllOverIt.Cryptography.AES;
using AllOverIt.Fixture;
using Shouldly;

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

                actual.ShouldBeOfType<AesEncryptor>();
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

                actual.ShouldBeOfType<AesEncryptor>();
                actual.Configuration.Key.ShouldBe(key);
                actual.Configuration.IV.ShouldBe(iv);
            }
        }
    }
}
