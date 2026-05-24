using AllOverIt.Cryptography.Hybrid;
using AllOverIt.Cryptography.RSA;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using Shouldly;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.Tests.Hybrid
{
    public class RsaAesHybridEncryptorFixture : FixtureBase
    {
        private readonly IRsaAesHybridEncryptor _encryptor;
        private readonly IRsaAesHybridEncryptor _decryptor;

        public RsaAesHybridEncryptorFixture()
        {
            var senderRsaKeys = new RsaKeyPair();        // encrypter
            var recipientRsaKeys = new RsaKeyPair();     // decrypter

            var encryptionConfiguration = new RsaAesHybridEncryptorConfiguration
            {
                Encryption = new RsaEncryptorConfiguration
                {
                    Keys = new RsaKeyPair(recipientRsaKeys.PublicKey, null),    // PublicKey is used to encrypt the AES key
                    Padding = RSAEncryptionPadding.OaepSHA256
                },

                Signing = new RsaSigningConfiguration
                {
                    Keys = new RsaKeyPair(null, senderRsaKeys.PrivateKey),      // PrivateKey is used to RSA sign the hash
                    HashAlgorithmName = HashAlgorithmName.SHA256,
                    Padding = RSASignaturePadding.Pkcs1
                }
            };

            _encryptor = new RsaAesHybridEncryptor(encryptionConfiguration);

            var decryptionConfiguration = new RsaAesHybridEncryptorConfiguration
            {
                Encryption = new RsaEncryptorConfiguration
                {
                    Keys = new RsaKeyPair(null, recipientRsaKeys.PrivateKey),   // PrivateKey is used to decrypt the AES key

                    Padding = RSAEncryptionPadding.OaepSHA256
                },

                Signing = new RsaSigningConfiguration
                {
                    Keys = new RsaKeyPair(senderRsaKeys.PublicKey, null),       // PublicKey is used to verify the RSA signature
                    HashAlgorithmName = HashAlgorithmName.SHA256,
                    Padding = RSASignaturePadding.Pkcs1
                }
            };

            _decryptor = new RsaAesHybridEncryptor(decryptionConfiguration);
        }

        public class Constructor : RsaAesHybridEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_Configuration_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = new RsaAesHybridEncryptor(null);
                })
                .WithNamedMessageWhenNull("configuration");
            }
        }

        public class Encrypt_Bytes : RsaAesHybridEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_PlainText_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = _encryptor.Encrypt(null);
                })
                .WithNamedMessageWhenNull("plainText");
            }
        }

        public class Encrypt_Stream : RsaAesHybridEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_PlainText_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _encryptor.Encrypt(null, Stream.Null);
                })
                .WithNamedMessageWhenNull("plainTextStream");
            }

            [Fact]
            public void Should_Throw_When_CipherText_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _encryptor.Encrypt(Stream.Null, null);
                })
                .WithNamedMessageWhenNull("cipherTextStream");
            }
        }

        public class Decrypt_Bytes : RsaAesHybridEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_PlainText_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = _decryptor.Decrypt(null);
                })
                .WithNamedMessageWhenNull("cipherText");
            }
        }

        public class Decrypt_Stream : RsaAesHybridEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_PlainText_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _decryptor.Decrypt(null, Stream.Null);
                })
                .WithNamedMessageWhenNull("cipherTextStream");
            }

            [Fact]
            public void Should_Throw_When_CipherText_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _decryptor.Decrypt(Stream.Null, null);
                })
                .WithNamedMessageWhenNull("plainTextStream");
            }
        }

        public class Encrypt_Decrypt_Bytes : RsaAesHybridEncryptorFixture
        {
            [Fact]
            public void Should_Encrypt_Decrypt()
            {
                var byteCount = GetWithinRange(1, 2048);
                var plainText = CreateMany<byte>(byteCount).ToArray();

                var cipherText = _encryptor.Encrypt(plainText);

                var actual = _decryptor.Decrypt(cipherText);

                actual.ShouldBe(plainText);
            }
        }

        public class Encrypt_Decrypt_Stream : RsaAesHybridEncryptorFixture
        {
            [Fact]
            public void Should_Encrypt_Decrypt()
            {
                var byteCount = GetWithinRange(1, 2048);
                var plainText = CreateMany<byte>(byteCount).ToArray();

                var plainTextStream = new MemoryStream(plainText);
                var cipherTextStream = new MemoryStream();

                _encryptor.Encrypt(plainTextStream, cipherTextStream);

                plainTextStream = new MemoryStream();

                cipherTextStream.Position = 0;
                _decryptor.Decrypt(cipherTextStream, plainTextStream);

                var actual = plainTextStream.ToArray();

                actual.ShouldBe(plainText);
            }
        }
    }
}