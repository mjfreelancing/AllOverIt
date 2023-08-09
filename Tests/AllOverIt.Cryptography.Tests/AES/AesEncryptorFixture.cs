using AllOverIt.Cryptography.AES;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Xunit;

namespace AllOverIt.Cryptography.Tests.AES
{
    public class AesEncryptorFixture : FixtureBase
    {
        public class Constructor_Key_IV : AesEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                Invoking(() =>
                {
                    _ = new AesEncryptor(null, AesUtils.GenerateIV());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Throw_When_IV_Null()
            {
                Invoking(() =>
                {
                    _ = new AesEncryptor(AesUtils.GenerateKey(256), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("iv");
            }
        }

        public class Constructor_Configuration : AesEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_Configuration_Null()
            {
                Invoking(() =>
                {
                    _ = new AesEncryptor((IAesEncryptionConfiguration) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("configuration");
            }
        }

        public class GetCipherTextLength : AesEncryptorFixture
        {
            // CTS and OFB are not valid for AES
            [Theory]
            [InlineData(CipherMode.CBC)]
            [InlineData(CipherMode.CFB)]
            [InlineData(CipherMode.ECB)]
            public void Should_Get_Expected_Length(CipherMode cipherMode)
            {
                var plainTextLength = Create<int>();
                var paddingMode = CreateExcluding(PaddingMode.None);
                var expected = -1;

                using (var aes = Aes.Create())
                {
                    aes.Mode = cipherMode;

                    expected = cipherMode switch
                    {
                        CipherMode.CBC => aes.GetCiphertextLengthCbc(plainTextLength, paddingMode),
                        CipherMode.CFB => aes.GetCiphertextLengthCfb(plainTextLength, paddingMode),
                        CipherMode.ECB => aes.GetCiphertextLengthEcb(plainTextLength, paddingMode),

                        _ => throw new InvalidOperationException()
                    };
                }

                var configuration = new AesEncryptionConfiguration
                {
                    Mode = cipherMode,
                    Padding = paddingMode
                };

                var encryptor = new AesEncryptor(configuration);

                var actual = encryptor.GetCipherTextLength(plainTextLength);

                actual.Should().Be(expected);
            }
        }

        public class Encrypt_Decrypt_Byte : AesEncryptorFixture
        {
            [Fact]
            public void Should_Encrypt_Decrypt_Default_AndKey_IV()
            {
                var plainText = CreateMany<byte>().ToArray();

                var encryptor = new AesEncryptor();
                var cipherText = encryptor.Encrypt(plainText);

                var decryptor = new AesEncryptor(encryptor.Configuration.Key, encryptor.Configuration.IV);
                var actual = decryptor.Decrypt(cipherText);

                actual.Should().BeEquivalentTo(plainText);
            }

            [Fact]
            public void Should_Encrypt_Decrypt_Configuration()
            {
                var configuration = CreateAesConfiguration();

                var plainText = CreateMany<byte>().ToArray();

                var encryptor = new AesEncryptor(configuration);
                var cipherText = encryptor.Encrypt(plainText);

                var decryptor = new AesEncryptor(configuration);
                var actual = decryptor.Decrypt(cipherText);

                actual.Should().BeEquivalentTo(plainText);
            }
        }

        public class Encrypt_Decrypt_Stream : AesEncryptorFixture
        {
            [Fact]
            public void Should_Encrypt_Decrypt_Default_AndKey_IV()
            {
                var plainText = CreateMany<byte>().ToArray();

                using (var plainTextStream = new MemoryStream(plainText))
                {
                    using (var cipherTextStream = new MemoryStream())
                    {
                        var encryptor = new AesEncryptor();
                        encryptor.Encrypt(plainTextStream, cipherTextStream);

                        using (var actualStream = new MemoryStream())
                        {
                            cipherTextStream.Position = 0;

                            var decryptor = new AesEncryptor(encryptor.Configuration.Key, encryptor.Configuration.IV);
                            decryptor.Decrypt(cipherTextStream, actualStream);

                            var actual = actualStream.ToArray();

                            actual.Should().BeEquivalentTo(plainText);
                        }
                    }
                }
            }

            [Fact]
            public void Should_Encrypt_Decrypt_Configuration()
            {
                var configuration = CreateAesConfiguration();

                var plainText = CreateMany<byte>().ToArray();

                using (var plainTextStream = new MemoryStream(plainText))
                {
                    using (var cipherTextStream = new MemoryStream())
                    {
                        var encryptor = new AesEncryptor(configuration);
                        encryptor.Encrypt(plainTextStream, cipherTextStream);

                        using (var actualStream = new MemoryStream())
                        {
                            cipherTextStream.Position = 0;

                            var decryptor = new AesEncryptor(configuration);
                            decryptor.Decrypt(cipherTextStream, actualStream);

                            var actual = actualStream.ToArray();

                            actual.Should().BeEquivalentTo(plainText);
                        }
                    }
                }
            }
        }

        private CipherMode GetCipherMode()
        {
            return CreateExcluding(CipherMode.CTS, CipherMode.OFB);
        }

        private AesEncryptionConfiguration CreateAesConfiguration()
        {
            var cipherMode = GetCipherMode();

            return new AesEncryptionConfiguration
            {
                Mode = cipherMode,

                Padding = CreateExcluding<PaddingMode>(PaddingMode.None),

                FeedbackSize = cipherMode == CipherMode.CFB
                        ? Create<bool>() ? 8 : 128
                        : 8,

                KeySize = Create<bool>()
                        ? AesUtils.GetLegalKeySizes()[0].MinSize
                        : AesUtils.GetLegalKeySizes()[0].MaxSize
            };
        }
    }
}
