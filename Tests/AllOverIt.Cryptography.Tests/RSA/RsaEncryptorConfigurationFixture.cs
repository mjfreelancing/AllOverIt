using AllOverIt.Cryptography.Extensions;
using AllOverIt.Cryptography.RSA;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Shouldly.Extensions;
using Shouldly;
using System.Security.Cryptography;

using RSAAlgorithm = System.Security.Cryptography.RSA;

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

                actual.Padding.ShouldBe(RSAEncryptionPadding.OaepSHA256);
                actual.Keys.PublicKey.ShouldNotBeEmpty();
                actual.Keys.PrivateKey.ShouldNotBeEmpty();
            }

            [Fact]
            public void Should_Generate_New_Keys()
            {
                var config1 = new RsaEncryptorConfiguration();
                var config2 = new RsaEncryptorConfiguration();

                config1.Keys.PublicKey.ShouldNotBe(config2.Keys.PublicKey);
            }
        }

        public class Constructor_Keys_Bytes : RsaEncryptorConfigurationFixture
        {
            private RsaKeyPair _rsaKeyPair = new RsaKeyPair();

            [Fact]
            public void Should_Throw_When_PublicKey_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = new RsaEncryptorConfiguration(null, _rsaKeyPair.PrivateKey);
                })
                .WithNamedMessageWhenNull("publicKey");
            }

            [Fact]
            public void Should_Throw_When_PrivateKey_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = new RsaEncryptorConfiguration(_rsaKeyPair.PublicKey, null);
                })
                .WithNamedMessageWhenNull("privateKey");
            }

            [Fact]
            public void Should_Create_With_Keys()
            {
                var encryptor = new RsaEncryptorConfiguration(_rsaKeyPair.PublicKey, _rsaKeyPair.PrivateKey);

                encryptor.Keys.PublicKey.ShouldBe(_rsaKeyPair.PublicKey);
                encryptor.Keys.PrivateKey.ShouldBe(_rsaKeyPair.PrivateKey);
            }
        }

        public class Constructor_Keys_Base64 : RsaEncryptorConfigurationFixture
        {
            private RsaKeyPair _rsaKeyPair = new RsaKeyPair();

            private readonly string _publicKeyBase64;
            private readonly string _privateKeyBase64;

            public Constructor_Keys_Base64()
            {
                _publicKeyBase64 = _rsaKeyPair.GetPublicKeyAsBase64();
                _privateKeyBase64 = _rsaKeyPair.GetPrivateKeyAsBase64();
            }

            [Fact]
            public void Should_Throw_When_PublicKey_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = new RsaEncryptorConfiguration(null, _privateKeyBase64);
                })
                .WithNamedMessageWhenNull("publicKeyBase64");
            }

            [Fact]
            public void Should_Throw_When_PrivateKey_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = new RsaEncryptorConfiguration(_publicKeyBase64, null);
                })
                .WithNamedMessageWhenNull("privateKeyBase64");
            }

            [Fact]
            public void Should_Create_With_Keys()
            {
                var encryptor = new RsaEncryptorConfiguration(_publicKeyBase64, _privateKeyBase64);

                encryptor.Keys.PublicKey.ShouldBe(_rsaKeyPair.PublicKey);
                encryptor.Keys.PrivateKey.ShouldBe(_rsaKeyPair.PrivateKey);
            }
        }

        public class Constructor_RSAParameters : RsaEncryptorConfigurationFixture
        {
            [Fact]
            public void Should_Create_With_Keys()
            {
                var rsa = RSAAlgorithm.Create();
                var parameters = rsa.ExportParameters(true);

                var encryptor = new RsaEncryptorConfiguration(parameters);

                var rsaKeyPair = new RsaKeyPair(rsa);

                encryptor.Keys.PublicKey.ShouldBe(rsaKeyPair.PublicKey);
                encryptor.Keys.PrivateKey.ShouldBe(rsaKeyPair.PrivateKey);
            }
        }

        public class Constructor_RsaKeyPair : RsaEncryptorConfigurationFixture
        {
            [Fact]
            public void Should_Throw_When_RSAKeyPair_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = new RsaEncryptorConfiguration(null);
                })
                .WithNamedMessageWhenNull("rsaKeyPair");
            }

            [Fact]
            public void Should_Create_With_Keys()
            {
                var rsaKeys = new RsaKeyPair();

                var config = new RsaEncryptorConfiguration(rsaKeys);

                config.Keys.ShouldBeEquivalentTo(rsaKeys);
            }
        }
    }
}
