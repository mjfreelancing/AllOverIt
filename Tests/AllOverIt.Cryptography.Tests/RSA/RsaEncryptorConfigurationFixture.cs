using AllOverIt.Cryptography.Extensions;
using AllOverIt.Cryptography.RSA;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
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

        public class Constructor_Keys_Bytes : RsaEncryptorConfigurationFixture
        {
            private RsaKeyPair _rsaKeyPair = new RsaKeyPair();

            [Fact]
            public void Should_Throw_When_PublicKey_Null()
            {
                Invoking(() =>
                {
                    _ = new RsaEncryptorConfiguration(null, _rsaKeyPair.PrivateKey);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("publicKey");
            }

            [Fact]
            public void Should_Throw_When_PrivateKey_Null()
            {
                Invoking(() =>
                {
                    _ = new RsaEncryptorConfiguration(_rsaKeyPair.PublicKey, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("privateKey");
            }

            [Fact]
            public void Should_Create_With_Keys()
            {
                var encryptor = new RsaEncryptorConfiguration(_rsaKeyPair.PublicKey, _rsaKeyPair.PrivateKey);

                encryptor.Keys.PublicKey.Should().BeEquivalentTo(_rsaKeyPair.PublicKey);
                encryptor.Keys.PrivateKey.Should().BeEquivalentTo(_rsaKeyPair.PrivateKey);
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
                Invoking(() =>
                {
                    _ = new RsaEncryptorConfiguration(null, _privateKeyBase64);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("publicKeyBase64");
            }

            [Fact]
            public void Should_Throw_When_PrivateKey_Null()
            {
                Invoking(() =>
                {
                    _ = new RsaEncryptorConfiguration(_publicKeyBase64, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("privateKeyBase64");
            }

            [Fact]
            public void Should_Create_With_Keys()
            {
                var encryptor = new RsaEncryptorConfiguration(_publicKeyBase64, _privateKeyBase64);

                encryptor.Keys.PublicKey.Should().BeEquivalentTo(_rsaKeyPair.PublicKey);
                encryptor.Keys.PrivateKey.Should().BeEquivalentTo(_rsaKeyPair.PrivateKey);
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

                encryptor.Keys.PublicKey.Should().BeEquivalentTo(rsaKeyPair.PublicKey);
                encryptor.Keys.PrivateKey.Should().BeEquivalentTo(rsaKeyPair.PrivateKey);
            }
        }

        public class Constructor_RsaKeyPair : RsaEncryptorConfigurationFixture
        {
            [Fact]
            public void Should_Throw_When_RSAKeyPair_Null()
            {
                Invoking(() =>
                {
                    _ = new RsaEncryptorConfiguration(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("rsaKeyPair");
            }

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
