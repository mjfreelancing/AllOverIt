using AllOverIt.Cryptography.Extensions;
using AllOverIt.Cryptography.RSA;
using AllOverIt.Cryptography.RSA.Exceptions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Shouldly.Extensions;
using FakeItEasy;
using Shouldly;

using RSAAlgorithm = System.Security.Cryptography.RSA;

namespace AllOverIt.Cryptography.Tests.RSA
{
    public class RsaEncryptorFixture : FixtureBase
    {
        public class Constructor : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Use_Default_Configuration()
            {
                var encryptor1 = new RsaEncryptor();
                var encryptor2 = new RsaEncryptor();

                encryptor1.Configuration.ShouldBeOfType<RsaEncryptorConfiguration>();

                encryptor1.Configuration.Keys.ShouldNotBeEquivalentTo(encryptor2.Configuration.Keys);
            }
        }

        public class Constructor_Configuration : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_Configuration_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = new RsaEncryptor((IRsaEncryptorConfiguration)null);
                })
                .WithNamedMessageWhenNull("configuration");
            }

            [Fact]
            public void Should_Use_Specified_Configuration()
            {
                var config = new RsaEncryptorConfiguration();
                var encryptor = new RsaEncryptor(config);

                encryptor.Configuration.ShouldBeSameAs(config);
            }
        }

        public class Constructor_Keys_Bytes : RsaEncryptorFixture
        {
            private RsaKeyPair _rsaKeyPair = new RsaKeyPair();

            [Fact]
            public void Should_Throw_When_PublicKey_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = new RsaEncryptor(null, _rsaKeyPair.PrivateKey);
                })
                .WithNamedMessageWhenNull("publicKey");
            }

            [Fact]
            public void Should_Throw_When_PrivateKey_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = new RsaEncryptor(_rsaKeyPair.PublicKey, null);
                })
                .WithNamedMessageWhenNull("privateKey");
            }

            [Fact]
            public void Should_Return_Configured_Encryptor()
            {
                var encryptor = new RsaEncryptor(_rsaKeyPair.PublicKey, _rsaKeyPair.PrivateKey);

                encryptor.ShouldBeOfType<RsaEncryptor>();

                encryptor.Configuration.Keys.PublicKey.ShouldBe(_rsaKeyPair.PublicKey);
                encryptor.Configuration.Keys.PrivateKey.ShouldBe(_rsaKeyPair.PrivateKey);
            }
        }

        public class Constructor_Keys_Base64 : RsaEncryptorFixture
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
                    _ = new RsaEncryptor(null, _privateKeyBase64);
                })
                .WithNamedMessageWhenNull("publicKeyBase64");
            }

            [Fact]
            public void Should_Throw_When_PrivateKey_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = new RsaEncryptor(_publicKeyBase64, null);
                })
                .WithNamedMessageWhenNull("privateKeyBase64");
            }

            [Fact]
            public void Should_Return_Configured_Encryptor()
            {
                var encryptor = new RsaEncryptor(_publicKeyBase64, _privateKeyBase64);

                encryptor.ShouldBeOfType<RsaEncryptor>();

                encryptor.Configuration.Keys.PublicKey.ShouldBe(_rsaKeyPair.PublicKey);
                encryptor.Configuration.Keys.PrivateKey.ShouldBe(_rsaKeyPair.PrivateKey);
            }
        }

        public class Constructor_RSAParameters : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Return_Configured_Encryptor()
            {
                var rsa = RSAAlgorithm.Create();
                var parameters = rsa.ExportParameters(true);

                var encryptor = new RsaEncryptor(parameters);

                encryptor.ShouldBeOfType<RsaEncryptor>();

                var rsaKeyPair = new RsaKeyPair(rsa);

                encryptor.Configuration.Keys.PublicKey.ShouldBe(rsaKeyPair.PublicKey);
                encryptor.Configuration.Keys.PrivateKey.ShouldBe(rsaKeyPair.PrivateKey);
            }
        }

        public class Constructor_RsaKeyPair : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_RsaKeyPair_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = new RsaEncryptor((RsaKeyPair)null);
                })
                .WithNamedMessageWhenNull("rsaKeyPair");
            }

            [Fact]
            public void Should_Return_Configured_Encryptor()
            {
                var rsaKeyPair = new RsaKeyPair();

                var encryptor = new RsaEncryptor(rsaKeyPair);

                encryptor.ShouldBeOfType<RsaEncryptor>();

                encryptor.Configuration.Keys.PublicKey.ShouldBe(rsaKeyPair.PublicKey);
                encryptor.Configuration.Keys.PrivateKey.ShouldBe(rsaKeyPair.PrivateKey);
            }
        }

        public class GetMaxInputLength : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_Public_Key_Null()
            {
                Should.Throw<RsaException>(() =>
                {
                    var rsaKeyPair = new RsaKeyPair();

                    var config = new RsaEncryptorConfiguration
                    {
                        Keys = new RsaKeyPair(null, rsaKeyPair.PrivateKey)
                    };

                    var encryptor = new RsaEncryptor(config);

                    _ = encryptor.GetMaxInputLength();
                })
                .WithMessage("An RSA public key has not been configured.");
            }

            [Fact]
            public void Should_Not_Throw_When_No_Private_Key()
            {
                Should.NotThrow(() =>
                {
                    var rsaKeyPair = new RsaKeyPair();

                    var config = new RsaEncryptorConfiguration
                    {
                        Keys = new RsaKeyPair(rsaKeyPair.PublicKey, null)
                    };

                    var encryptor = new RsaEncryptor(config);

                    _ = encryptor.GetMaxInputLength();
                });
            }

            [Fact]
            public void Should_Evaluate_MaxLength_Once()
            {
                var rsaFactoryFake = this.CreateFake<IRsaFactory>();

                rsaFactoryFake
                    .CallsTo(fake => fake.Create())
                    .Returns(RSAAlgorithm.Create());

                var encryptor = new RsaEncryptor(rsaFactoryFake.FakedObject, new RsaEncryptorConfiguration());

                encryptor.GetMaxInputLength();
                encryptor.GetMaxInputLength();

                rsaFactoryFake
                    .CallsTo(fake => fake.Create())
                    .MustHaveHappenedOnceExactly();
            }

            [Fact]
            public void Should_Get_MaxLength()
            {
                var encryptor = new RsaEncryptor();

                var expected = RsaUtils.GetMaxInputLength(encryptor.Configuration.Keys.KeySize, encryptor.Configuration.Padding);

                var actual = encryptor.GetMaxInputLength();

                actual.ShouldBe(expected);
            }
        }

        public class Encrypt_Bytes : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_PlainText_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    var encryptor = new RsaEncryptor();

                    _ = encryptor.Encrypt(null);
                })
                .WithNamedMessageWhenNull("plainText");
            }

            [Fact]
            public void Should_Throw_When_PublicKey_Null()
            {
                Should.Throw<RsaException>(() =>
                {
                    var rsaKeyPair = new RsaKeyPair();

                    var config = new RsaEncryptorConfiguration
                    {
                        Keys = new RsaKeyPair(null, rsaKeyPair.PrivateKey)
                    };

                    var encryptor = new RsaEncryptor(config);

                    _ = encryptor.Encrypt(CreateMany<byte>().ToArray());
                })
                .WithMessage("An RSA public key has not been configured.");
            }
        }

        public class Decrypt_Bytes : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_CipherText_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    var encryptor = new RsaEncryptor();

                    _ = encryptor.Decrypt(null);
                })
                .WithNamedMessageWhenNull("cipherText");
            }

            [Fact]
            public void Should_Throw_When_PrivateKey_Null()
            {
                Should.Throw<RsaException>(() =>
                {
                    var rsaKeyPair = new RsaKeyPair();

                    var config = new RsaEncryptorConfiguration
                    {
                        Keys = new RsaKeyPair(rsaKeyPair.PublicKey, null)
                    };

                    var encryptor = new RsaEncryptor(config);

                    _ = encryptor.Decrypt(CreateMany<byte>().ToArray());
                })
                .WithMessage("An RSA private key has not been configured.");
            }
        }

        public class Encrypt_Stream : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_PlainTextStream_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    var encryptor = new RsaEncryptor();

                    encryptor.Encrypt(null, Stream.Null);
                })
                .WithNamedMessageWhenNull("plainTextStream");
            }

            [Fact]
            public void Should_Throw_When_CipherTextStream_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    var encryptor = new RsaEncryptor();

                    encryptor.Encrypt(Stream.Null, null);
                })
                .WithNamedMessageWhenNull("cipherTextStream");
            }

            [Fact]
            public void Should_Throw_When_PublicKey_Null()
            {
                Should.Throw<RsaException>(() =>
                {
                    var rsaKeyPair = new RsaKeyPair();

                    var config = new RsaEncryptorConfiguration
                    {
                        Keys = new RsaKeyPair(null, rsaKeyPair.PrivateKey)
                    };

                    var encryptor = new RsaEncryptor(config);

                    encryptor.Encrypt(new MemoryStream(CreateMany<byte>().ToArray()), new MemoryStream());
                })
                .WithMessage("An RSA public key has not been configured.");
            }
        }

        public class Decrypt_Stream : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_CipherTextStream_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    var encryptor = new RsaEncryptor();

                    encryptor.Decrypt(null, Stream.Null);
                })
                .WithNamedMessageWhenNull("cipherTextStream");
            }

            [Fact]
            public void Should_Throw_When_PlainTextStream_Null()
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    var encryptor = new RsaEncryptor();

                    encryptor.Decrypt(Stream.Null, null);
                })
                .WithNamedMessageWhenNull("plainTextStream");
            }

            [Fact]
            public void Should_Throw_When_PrivateKey_Null()
            {
                Should.Throw<RsaException>(() =>
                {
                    var rsaKeyPair = new RsaKeyPair();

                    var config = new RsaEncryptorConfiguration
                    {
                        Keys = new RsaKeyPair(rsaKeyPair.PublicKey, null)
                    };

                    var encryptor = new RsaEncryptor(config);

                    encryptor.Decrypt(new MemoryStream(CreateMany<byte>().ToArray()), new MemoryStream());
                })
                .WithMessage("An RSA private key has not been configured.");
            }
        }

        public class Encrypt_Decrypt_Bytes : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Encrypt_Decrypt_Less_Than_Max_Input_Length()
            {
                var encryptor = new RsaEncryptor();
                var maxInputLength = encryptor.GetMaxInputLength();
                var inputLength = maxInputLength - GetWithinRange(1, maxInputLength / 2);

                var plainText = CreateMany<byte>(inputLength).ToArray();

                var cipherText = encryptor.Encrypt(plainText);

                var actual = encryptor.Decrypt(cipherText);

                actual.ShouldBe(plainText);
            }

            [Fact]
            public void Should_Encrypt_Decrypt_With_Max_Input_Length()
            {
                var encryptor = new RsaEncryptor();
                var maxInputLength = encryptor.GetMaxInputLength();

                var plainText = CreateMany<byte>(maxInputLength).ToArray();

                var cipherText = encryptor.Encrypt(plainText);

                var actual = encryptor.Decrypt(cipherText);

                actual.ShouldBe(plainText);
            }

            [Fact]
            public void Should_Encrypt_Decrypt_More_Than_Max_Input_Length()
            {
                var encryptor = new RsaEncryptor();
                var maxInputLength = encryptor.GetMaxInputLength();
                var inputLength = maxInputLength + GetWithinRange(1, maxInputLength * 4);

                var plainText = CreateMany<byte>(inputLength).ToArray();

                var cipherText = encryptor.Encrypt(plainText);

                var actual = encryptor.Decrypt(cipherText);

                actual.ShouldBe(plainText);
            }
        }

        public class Encrypt_Decrypt_Stream : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Encrypt_Decrypt_Less_Than_Max_Input_Length()
            {
                var encryptor = new RsaEncryptor();
                var maxInputLength = encryptor.GetMaxInputLength();
                var inputLength = maxInputLength - GetWithinRange(1, maxInputLength / 2);

                var plainText = CreateMany<byte>(inputLength).ToArray();
                var plainTextStream = new MemoryStream(plainText);
                var cipherTextStream = new MemoryStream();

                encryptor.Encrypt(plainTextStream, cipherTextStream);

                plainTextStream = new MemoryStream();

                cipherTextStream.Position = 0;

                encryptor.Decrypt(cipherTextStream, plainTextStream);

                var actual = plainTextStream.ToArray();

                actual.ShouldBe(plainText);
            }

            [Fact]
            public void Should_Encrypt_Decrypt_With_Max_Input_Length()
            {
                var encryptor = new RsaEncryptor();
                var maxInputLength = encryptor.GetMaxInputLength();

                var plainText = CreateMany<byte>(maxInputLength).ToArray();
                var plainTextStream = new MemoryStream(plainText);
                var cipherTextStream = new MemoryStream();

                encryptor.Encrypt(plainTextStream, cipherTextStream);

                plainTextStream = new MemoryStream();

                cipherTextStream.Position = 0;

                encryptor.Decrypt(cipherTextStream, plainTextStream);

                var actual = plainTextStream.ToArray();

                actual.ShouldBe(plainText);
            }

            [Fact]
            public void Should_Encrypt_Decrypt_More_Than_Max_Input_Length()
            {
                var encryptor = new RsaEncryptor();
                var maxInputLength = encryptor.GetMaxInputLength();
                var inputLength = maxInputLength + GetWithinRange(1, maxInputLength * 4);

                var plainText = CreateMany<byte>(inputLength).ToArray();
                var plainTextStream = new MemoryStream(plainText);
                var cipherTextStream = new MemoryStream();

                encryptor.Encrypt(plainTextStream, cipherTextStream);

                plainTextStream = new MemoryStream();

                cipherTextStream.Position = 0;

                encryptor.Decrypt(cipherTextStream, plainTextStream);

                var actual = plainTextStream.ToArray();

                actual.ShouldBe(plainText);
            }
        }
    }
}
