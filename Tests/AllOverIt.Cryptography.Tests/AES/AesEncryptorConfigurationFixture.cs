using AllOverIt.Cryptography.AES;
using AllOverIt.Cryptography.AES.Exceptions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Shouldly.Extensions;
using Shouldly;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.Tests.AES
{
    public class AesEncryptorConfigurationFixture : FixtureBase
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public AesEncryptorConfigurationFixture()
        {
            _key = CreateMany<byte>(32).ToArray();
            _iv = CreateMany<byte>(16).ToArray();
        }

        public class Constructor : AesEncryptorConfigurationFixture
        {
            [Fact]
            public void Should_Default_Initialize()
            {
                var actual = new AesEncryptorConfiguration();

                var expected = new
                {
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7,
                    KeySize = 256,
                    BlockSize = 128,
                    FeedbackSize = 8,

                    // these are random - setting here to keep the test happy - see other tests for randomness
                    actual.Key,
                    actual.IV
                };

                actual.ShouldBeEquivalentTo(expected);
            }

            [Theory]
            [InlineData(128)]
            [InlineData(256)]
            public void Should_Set_Key_Length_Based_On_KeySize(int keySize)
            {
                var actual = new AesEncryptorConfiguration
                {
                    KeySize = keySize
                };

                var expected = new
                {
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7,
                    KeySize = keySize,
                    BlockSize = 128,
                    FeedbackSize = 8,

                    // these are random - setting here to keep the test happy - see other tests for randomness
                    actual.Key,
                    actual.IV
                };

                actual.ShouldBeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Have_Random_Key_And_IV()
            {
                var actual1 = new AesEncryptorConfiguration();

                actual1.Key.ShouldNotBeEmpty();
                actual1.IV.ShouldNotBeEmpty();

                var actual2 = new AesEncryptorConfiguration();

                actual1.Key.ShouldNotBe(actual2.Key);
                actual1.IV.ShouldNotBe(actual2.IV);
            }

        }

        public class Constructor_Key_IV : AesEncryptorConfigurationFixture
        {
            [Fact]
            public void Should_Throw_When_Key_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = new AesEncryptorConfiguration(null, _iv);
                })
                .WithNamedMessageWhenNull("key");
            }

            [Fact]
            public void Should_Throw_When_Key_Invalid_Size()
            {
                var keySize = AesUtils.GetLegalKeySizes().First().MaxSize / 8 - 1;

                Should.Throw<AesException>(() =>
                {
                    var key = CreateMany<byte>(keySize).ToArray();

                    _ = new AesEncryptorConfiguration(key, _iv);
                })
                .WithMessage($"AES Key size {keySize * 8} is invalid.");
            }

            [Fact]
            public void Should_Throw_When_IV_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = new AesEncryptorConfiguration(_key, null);
                })
                .WithNamedMessageWhenNull("iv");
            }

            [Fact]
            public void Should_Throw_When_IV_Invalid_Size()
            {
                Should.Throw<AesException>(() =>
                {
                    // A valid IV is 16 bytes
                    var iv = CreateMany<byte>(15).ToArray();

                    _ = new AesEncryptorConfiguration(_key, iv);
                })
                .WithMessage("The AES Initialization Vector must be 16 bytes.");
            }

            [Fact]
            public void Should_Default_Initialize_With_Provided_Key_And_IV()
            {
                var actual = new AesEncryptorConfiguration(_key, _iv);

                var expected = new
                {
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7,
                    KeySize = 256,
                    BlockSize = 128,
                    FeedbackSize = 8,
                    Key = _key,
                    IV = _iv
                };

                actual.ShouldBeEquivalentTo(expected);
            }

            [Fact]
            public void Should_Set_KeySize()
            {
                var legalKeySize = AesUtils.GetLegalKeySizes().First();
                var keySize = legalKeySize.MinSize;

                var actual = new AesEncryptorConfiguration(_key, _iv);

                actual.KeySize.ShouldNotBe(keySize);

                var key = CreateMany<byte>(keySize / 8).ToArray();

                actual = new AesEncryptorConfiguration(key, _iv);

                actual.KeySize.ShouldBe(keySize);
            }
        }

        public class RegenerateKey : AesEncryptorConfigurationFixture
        {
            [Fact]
            public void Should_Regenerate_Key()
            {
                var actual = new AesEncryptorConfiguration(_key, _iv);

                actual.RegenerateKey();

                actual.Key.ShouldNotBe(_key);
                actual.IV.ShouldBe(_iv);
            }
        }

        public class RegenerateIV : AesEncryptorConfigurationFixture
        {
            [Fact]
            public void Should_Regenerate_IV()
            {
                var actual = new AesEncryptorConfiguration(_key, _iv);

                actual.RegenerateIV();

                actual.Key.ShouldBe(_key);
                actual.IV.ShouldNotBe(_iv);
            }
        }

        public class RegenerateKeyAndIV : AesEncryptorConfigurationFixture
        {
            [Fact]
            public void Should_Regenerate_Key_And_IV()
            {
                var actual = new AesEncryptorConfiguration(_key, _iv);

                actual.RegenerateKeyAndIV();

                actual.Key.ShouldNotBe(_key);
                actual.IV.ShouldNotBe(_iv);
            }
        }
    }
}