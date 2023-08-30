using AllOverIt.Cryptography.Hybrid;
using AllOverIt.Cryptography.RSA;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Security.Cryptography;
using Xunit;

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

                actual.Keys.Should().NotBeNull();
                actual.HashAlgorithmName.Should().Be(HashAlgorithmName.SHA256);
                actual.Padding.Should().Be(RSASignaturePadding.Pkcs1);
            }
        }

        public class Constructor_Keys : RsaSigningConfigurationFixture
        {
            [Fact]
            public void Should_Have_Defaults_With_Keys()
            {
                var keys = RsaKeyPair.Create();
                var actual = new RsaSigningConfiguration(keys);

                actual.Keys.Should().BeSameAs(keys);
                actual.HashAlgorithmName.Should().Be(HashAlgorithmName.SHA256);
                actual.Padding.Should().Be(RSASignaturePadding.Pkcs1);
            }
        }
    }
}