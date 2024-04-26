﻿using AllOverIt.Cryptography.Extensions;
using AllOverIt.Cryptography.RSA;
using AllOverIt.Cryptography.RSA.Exceptions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using FluentAssertions;

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

                encryptor1.Configuration.Should().BeOfType<RsaEncryptorConfiguration>();

                encryptor1.Configuration.Keys.Should().NotBeEquivalentTo(encryptor2.Configuration.Keys);
            }
        }

        public class Constructor_Configuration : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_Configuration_Null()
            {
                Invoking(() =>
                {
                    _ = new RsaEncryptor((IRsaEncryptorConfiguration) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("configuration");
            }

            [Fact]
            public void Should_Use_Specified_Configuration()
            {
                var config = new RsaEncryptorConfiguration();
                var encryptor = new RsaEncryptor(config);

                encryptor.Configuration.Should().BeSameAs(config);
            }
        }

        public class Constructor_Keys_Bytes : RsaEncryptorFixture
        {
            private RsaKeyPair _rsaKeyPair = new RsaKeyPair();

            [Fact]
            public void Should_Throw_When_PublicKey_Null()
            {
                Invoking(() =>
                {
                    _ = new RsaEncryptor(null, _rsaKeyPair.PrivateKey);
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
                    _ = new RsaEncryptor(_rsaKeyPair.PublicKey, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("privateKey");
            }

            [Fact]
            public void Should_Return_Configured_Encryptor()
            {
                var encryptor = new RsaEncryptor(_rsaKeyPair.PublicKey, _rsaKeyPair.PrivateKey);

                encryptor.Should().BeOfType<RsaEncryptor>();

                encryptor.Configuration.Keys.PublicKey.Should().BeEquivalentTo(_rsaKeyPair.PublicKey);
                encryptor.Configuration.Keys.PrivateKey.Should().BeEquivalentTo(_rsaKeyPair.PrivateKey);
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
                Invoking(() =>
                {
                    _ = new RsaEncryptor(null, _privateKeyBase64);
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
                    _ = new RsaEncryptor(_publicKeyBase64, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("privateKeyBase64");
            }

            [Fact]
            public void Should_Return_Configured_Encryptor()
            {
                var encryptor = new RsaEncryptor(_publicKeyBase64, _privateKeyBase64);

                encryptor.Should().BeOfType<RsaEncryptor>();

                encryptor.Configuration.Keys.PublicKey.Should().BeEquivalentTo(_rsaKeyPair.PublicKey);
                encryptor.Configuration.Keys.PrivateKey.Should().BeEquivalentTo(_rsaKeyPair.PrivateKey);
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

                encryptor.Should().BeOfType<RsaEncryptor>();

                var rsaKeyPair = new RsaKeyPair(rsa);

                encryptor.Configuration.Keys.PublicKey.Should().BeEquivalentTo(rsaKeyPair.PublicKey);
                encryptor.Configuration.Keys.PrivateKey.Should().BeEquivalentTo(rsaKeyPair.PrivateKey);
            }
        }

        public class Constructor_RsaKeyPair : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_RsaKeyPair_Null()
            {
                Invoking(() =>
                {
                    _ = new RsaEncryptor((RsaKeyPair) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("rsaKeyPair");
            }

            [Fact]
            public void Should_Return_Configured_Encryptor()
            {
                var rsaKeyPair = new RsaKeyPair();

                var encryptor = new RsaEncryptor(rsaKeyPair);

                encryptor.Should().BeOfType<RsaEncryptor>();

                encryptor.Configuration.Keys.PublicKey.Should().BeEquivalentTo(rsaKeyPair.PublicKey);
                encryptor.Configuration.Keys.PrivateKey.Should().BeEquivalentTo(rsaKeyPair.PrivateKey);
            }
        }

        public class GetMaxInputLength : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_Public_Key_Null()
            {
                Invoking(() =>
                {
                    var rsaKeyPair = new RsaKeyPair();

                    var config = new RsaEncryptorConfiguration
                    {
                        Keys = new RsaKeyPair(null, rsaKeyPair.PrivateKey)
                    };

                    var encryptor = new RsaEncryptor(config);

                    _ = encryptor.GetMaxInputLength();
                })
                .Should()
                .Throw<RsaException>()
                .WithMessage("An RSA public key has not been configured.");
            }

            [Fact]
            public void Should_Not_Throw_When_No_Private_Key()
            {
                Invoking(() =>
                {
                    var rsaKeyPair = new RsaKeyPair();

                    var config = new RsaEncryptorConfiguration
                    {
                        Keys = new RsaKeyPair(rsaKeyPair.PublicKey, null)
                    };

                    var encryptor = new RsaEncryptor(config);

                    _ = encryptor.GetMaxInputLength();
                })
                .Should()
                .NotThrow();
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

                actual.Should().Be(expected);
            }
        }

        public class Encrypt_Bytes : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_PlainText_Null()
            {
                Invoking(() =>
                {
                    var encryptor = new RsaEncryptor();

                    _ = encryptor.Encrypt(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("plainText");
            }

            [Fact]
            public void Should_Throw_When_PublicKey_Null()
            {
                Invoking(() =>
                {
                    var rsaKeyPair = new RsaKeyPair();

                    var config = new RsaEncryptorConfiguration
                    {
                        Keys = new RsaKeyPair(null, rsaKeyPair.PrivateKey)
                    };

                    var encryptor = new RsaEncryptor(config);

                    _ = encryptor.Encrypt(CreateMany<byte>().ToArray());
                })
                .Should()
                .Throw<RsaException>()
                .WithMessage("An RSA public key has not been configured.");
            }
        }

        public class Decrypt_Bytes : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_CipherText_Null()
            {
                Invoking(() =>
                {
                    var encryptor = new RsaEncryptor();

                    _ = encryptor.Decrypt(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("cipherText");
            }

            [Fact]
            public void Should_Throw_When_PrivateKey_Null()
            {
                Invoking(() =>
                {
                    var rsaKeyPair = new RsaKeyPair();

                    var config = new RsaEncryptorConfiguration
                    {
                        Keys = new RsaKeyPair(rsaKeyPair.PublicKey, null)
                    };

                    var encryptor = new RsaEncryptor(config);

                    _ = encryptor.Decrypt(CreateMany<byte>().ToArray());
                })
                .Should()
                .Throw<RsaException>()
                .WithMessage("An RSA private key has not been configured.");
            }
        }

        public class Encrypt_Stream : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_PlainTextStream_Null()
            {
                Invoking(() =>
                {
                    var encryptor = new RsaEncryptor();

                    encryptor.Encrypt(null, Stream.Null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("plainTextStream");
            }

            [Fact]
            public void Should_Throw_When_CipherTextStream_Null()
            {
                Invoking(() =>
                {
                    var encryptor = new RsaEncryptor();

                    encryptor.Encrypt(Stream.Null, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("cipherTextStream");
            }

            [Fact]
            public void Should_Throw_When_PublicKey_Null()
            {
                Invoking(() =>
                {
                    var rsaKeyPair = new RsaKeyPair();

                    var config = new RsaEncryptorConfiguration
                    {
                        Keys = new RsaKeyPair(null, rsaKeyPair.PrivateKey)
                    };

                    var encryptor = new RsaEncryptor(config);

                    encryptor.Encrypt(new MemoryStream(CreateMany<byte>().ToArray()), new MemoryStream());
                })
                .Should()
                .Throw<RsaException>()
                .WithMessage("An RSA public key has not been configured.");
            }
        }

        public class Decrypt_Stream : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_CipherTextStream_Null()
            {
                Invoking(() =>
                {
                    var encryptor = new RsaEncryptor();

                    encryptor.Decrypt(null, Stream.Null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("cipherTextStream");
            }

            [Fact]
            public void Should_Throw_When_PlainTextStream_Null()
            {
                Invoking(() =>
                {
                    var encryptor = new RsaEncryptor();

                    encryptor.Decrypt(Stream.Null, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("plainTextStream");
            }

            [Fact]
            public void Should_Throw_When_PrivateKey_Null()
            {
                Invoking(() =>
                {
                    var rsaKeyPair = new RsaKeyPair();

                    var config = new RsaEncryptorConfiguration
                    {
                        Keys = new RsaKeyPair(rsaKeyPair.PublicKey, null)
                    };

                    var encryptor = new RsaEncryptor(config);

                    encryptor.Decrypt(new MemoryStream(CreateMany<byte>().ToArray()), new MemoryStream());
                })
                .Should()
                .Throw<RsaException>()
                .WithMessage("An RSA private key has not been configured.");
            }
        }

        public class Encrypt_Decrypt_Bytes : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Encrypt_Decrypt()
            {
                var encryptor = new RsaEncryptor();

                var plainText = CreateMany<byte>().ToArray();

                var cipherText = encryptor.Encrypt(plainText);

                var actual = encryptor.Decrypt(cipherText);

                actual.Should().BeEquivalentTo(plainText);
            }
        }

        public class Encrypt_Decrypt_Stream : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Encrypt_Decrypt()
            {
                var encryptor = new RsaEncryptor();

                var plainText = CreateMany<byte>().ToArray();
                var plainTextStream = new MemoryStream(plainText);
                var cipherTextStream = new MemoryStream();

                encryptor.Encrypt(plainTextStream, cipherTextStream);

                plainTextStream = new MemoryStream();

                cipherTextStream.Position = 0;

                encryptor.Decrypt(cipherTextStream, plainTextStream);

                var actual = plainTextStream.ToArray();

                actual.Should().BeEquivalentTo(plainText);
            }
        }
    }
}
