using AllOverIt.Cryptography.RSA;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Security.Cryptography;
using Xunit;

namespace AllOverIt.Cryptography.Tests.RSA
{
    public class RsaEncryptorConfigurationFixture : FixtureBase
    {
        public class Constructor : RsaEncryptorConfigurationFixture
        {
            [Fact]
            public void Should_Default_Construct()
            {
                var actual = new RsaEncryptorConfiguration();

                actual.Padding.Should().Be(RSAEncryptionPadding.OaepSHA256);
                actual.Keys.PublicKey.Should().NotBeEmpty();
                actual.Keys.PrivateKey.Should().NotBeEmpty();
            }

            [Fact]
            public void Should_Generate_New_Keys()
            {
                var config1 = new RsaEncryptorConfiguration();
                var config2 = new RsaEncryptorConfiguration();

                config1.Keys.Should().NotBeEquivalentTo(config2.Keys);                
            }
        }

        public class Constructor_RsaKeyPair : RsaEncryptorConfigurationFixture
        {
            [Fact]
            public void Should_Create_With_Keys()
            {
                var rsaKeys = new RsaKeyPair();

                var config = new RsaEncryptorConfiguration(rsaKeys);

                config.Keys.Should().BeEquivalentTo(rsaKeys);
            }
        }
    }
}
