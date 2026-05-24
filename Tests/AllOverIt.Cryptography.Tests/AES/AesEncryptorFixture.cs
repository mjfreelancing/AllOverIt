using AllOverIt.Cryptography.AES;
using AllOverIt.Fixture.Extensions;
using Shouldly;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.Tests.AES
{
    public class AesEncryptorFixture : AesFixtureBase
    {
        public class Constructor_Key_IV : AesEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = new AesEncryptor(null, AesUtils.GenerateIV());
                })
                .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Throw_When_IV_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = new AesEncryptor(AesUtils.GenerateKey(256), null);
                })
                .WithNamedMessageWhenNull("iv");
            }
        }

        public class Constructor_Configuration : AesEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_Configuration_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = new AesEncryptor((IAesEncryptorConfiguration) null);
                })
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
                var paddingMode = GetPaddingMode();
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

                var configuration = new AesEncryptorConfiguration
                {
                    Mode = cipherMode,
                    Padding = paddingMode
                };

                var encryptor = new AesEncryptor(configuration);

                var actual = encryptor.GetCipherTextLength(plainTextLength);

                actual.ShouldBe(expected);
            }
        }

        public class Encrypt_Byte : AesEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_PlainText_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    var encryptor = new AesEncryptor();

                    _ = encryptor.Encrypt(null);
                })
                .WithNamedMessageWhenNull("plainText");
            }
        }

        public class Decrypt_Byte : AesEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_PlainText_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    var encryptor = new AesEncryptor();

                    _ = encryptor.Decrypt(null);
                })
                .WithNamedMessageWhenNull("cipherText");
            }
        }

        public class Encrypt_Stream : AesEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_PlainText_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    var encryptor = new AesEncryptor();

                    encryptor.Encrypt(null, Stream.Null);
                })
                .WithNamedMessageWhenNull("plainTextStream");
            }

            [Fact]
            public void Should_Throw_When_CipherText_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    var encryptor = new AesEncryptor();

                    encryptor.Encrypt(Stream.Null, null);
                })
                .WithNamedMessageWhenNull("cipherTextStream");
            }
        }

        public class Decrypt_Stream : AesEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_PlainText_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    var encryptor = new AesEncryptor();

                    encryptor.Decrypt(Stream.Null, null);
                })
                .WithNamedMessageWhenNull("plainTextStream");
            }

            [Fact]
            public void Should_Throw_When_CipherText_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    var encryptor = new AesEncryptor();

                    encryptor.Decrypt(null, Stream.Null);
                })
                .WithNamedMessageWhenNull("cipherTextStream");
            }
        }

        public class Encrypt_Decrypt_Byte : AesEncryptorFixture
        {
            [Fact]
            public void Should_Encrypt_Decrypt_Default_Key_IV()
            {
                var plainText = CreateMany<byte>().ToArray();

                var encryptor = new AesEncryptor();
                var cipherText = encryptor.Encrypt(plainText);

                var decryptor = new AesEncryptor(encryptor.Configuration.Key, encryptor.Configuration.IV);
                var actual = decryptor.Decrypt(cipherText);

                actual.ShouldBe(plainText);
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

                actual.ShouldBe(plainText);
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

                            actual.ShouldBe(plainText);
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

                            actual.ShouldBe(plainText);
                        }
                    }
                }
            }
        }
    }
}
