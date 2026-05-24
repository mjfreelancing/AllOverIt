using AllOverIt.Cryptography.Hybrid;
using AllOverIt.Cryptography.RSA;
using AllOverIt.Fixture;
using Shouldly;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.Tests.Hybrid
{
    public class RsaSigningConfigurationFixture : FixtureBase
    {
        public class Constructor : RsaSigningConfigurationFixture
        {
            [Fact]
            public void Should_Have_Defaults()
            {
                var actual = new RsaSigningConfiguration();

                actual.Keys.ShouldNotBeNull();
                actual.HashAlgorithmName.ShouldBe(HashAlgorithmName.SHA256);
                actual.Padding.ShouldBe(RSASignaturePadding.Pkcs1);
            }
        }

        public class Constructor_Keys : RsaSigningConfigurationFixture
        {
            [Fact]
            public void Should_Have_Defaults_With_Keys()
            {
                var keys = new RsaKeyPair();
                var actual = new RsaSigningConfiguration(keys);

                actual.Keys.ShouldBeSameAs(keys);
                actual.HashAlgorithmName.ShouldBe(HashAlgorithmName.SHA256);
                actual.Padding.ShouldBe(RSASignaturePadding.Pkcs1);
            }
        }
    }
}