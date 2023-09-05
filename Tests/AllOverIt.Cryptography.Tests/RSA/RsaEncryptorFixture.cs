using AllOverIt.Cryptography.RSA;
using AllOverIt.Cryptography.RSA.Exceptions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using FakeItEasy;
using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using Xunit;

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
                    _ = new RsaEncryptor(null);
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

        public class GetMaxInputLength : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_Public_Key_Null()
            {
                Invoking(() =>
                {
                    var rsaKeyPair = RsaKeyPair.Create();

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
                    var rsaKeyPair = RsaKeyPair.Create();

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
                var rsaFactory = this.CreateFake<IRsaFactory>();

                rsaFactory
                    .CallsTo(fake => fake.Create())
                    .Returns(RSAAlgorithm.Create());

                var encryptor = new RsaEncryptor(rsaFactory.FakedObject, new RsaEncryptorConfiguration());

                encryptor.GetMaxInputLength();
                encryptor.GetMaxInputLength();

                rsaFactory
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
                    var rsaKeyPair = RsaKeyPair.Create();

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
                    var rsaKeyPair = RsaKeyPair.Create();

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
                    var rsaKeyPair = RsaKeyPair.Create();

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
                    var rsaKeyPair = RsaKeyPair.Create();

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

        public class Create_Keys_Bytes : RsaEncryptorFixture
        {
            private RsaKeyPair _rsaKeyPair = RsaKeyPair.Create();

            [Fact]
            public void Should_Throw_When_PublicKey_Null()
            {
                Invoking(() =>
                {
                    _ = RsaEncryptor.Create(null, _rsaKeyPair.PrivateKey);
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
                    _ = RsaEncryptor.Create(_rsaKeyPair.PublicKey, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("privateKey");
            }

            [Fact]
            public void Should_Return_Configured_Encryptor()
            {
                var encryptor = RsaEncryptor.Create(_rsaKeyPair.PublicKey, _rsaKeyPair.PrivateKey);

                encryptor.Should().BeOfType<RsaEncryptor>();

                encryptor.Configuration.Keys.PublicKey.Should().BeEquivalentTo(_rsaKeyPair.PublicKey);
                encryptor.Configuration.Keys.PrivateKey.Should().BeEquivalentTo(_rsaKeyPair.PrivateKey);
            }
        }

        public class Create_Keys_Base64 : RsaEncryptorFixture
        {
            private RsaKeyPair _rsaKeyPair = RsaKeyPair.Create();

            private readonly string _publicKeyBase64;

            private readonly string _privateKeyBase64;

            public Create_Keys_Base64()
            {
                _publicKeyBase64 = Convert.ToBase64String(_rsaKeyPair.PublicKey);
                _privateKeyBase64 = Convert.ToBase64String(_rsaKeyPair.PrivateKey);
            }

            [Fact]
            public void Should_Throw_When_PublicKey_Null()
            {
                Invoking(() =>
                {
                    _ = RsaEncryptor.Create(null, _privateKeyBase64);
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
                    _ = RsaEncryptor.Create(_publicKeyBase64, null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("privateKeyBase64");
            }

            [Fact]
            public void Should_Return_Configured_Encryptor()
            {
                var encryptor = RsaEncryptor.Create(_publicKeyBase64, _privateKeyBase64);

                encryptor.Should().BeOfType<RsaEncryptor>();

                encryptor.Configuration.Keys.PublicKey.Should().BeEquivalentTo(_rsaKeyPair.PublicKey);
                encryptor.Configuration.Keys.PrivateKey.Should().BeEquivalentTo(_rsaKeyPair.PrivateKey);
            }
        }

        public class Create_RsaKeyPair : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_RsaKeyPair_Null()
            {
                Invoking(() =>
                {
                    _ = RsaEncryptor.Create((RsaKeyPair) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("rsaKeyPair");
            }

            [Fact]
            public void Should_Return_Configured_Encryptor()
            {
                var rsaKeyPair = RsaKeyPair.Create();

                var encryptor = RsaEncryptor.Create(rsaKeyPair);

                encryptor.Should().BeOfType<RsaEncryptor>();

                encryptor.Configuration.Keys.PublicKey.Should().BeEquivalentTo(rsaKeyPair.PublicKey);
                encryptor.Configuration.Keys.PrivateKey.Should().BeEquivalentTo(rsaKeyPair.PrivateKey);
            }
        }

        public class Create_RSAParameters : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Return_Configured_Encryptor()
            {
                var rsa = RSAAlgorithm.Create();
                var parameters = rsa.ExportParameters(true);

                var encryptor = RsaEncryptor.Create(parameters);

                encryptor.Should().BeOfType<RsaEncryptor>();

                var rsaKeyPair = new RsaKeyPair(rsa);

                encryptor.Configuration.Keys.PublicKey.Should().BeEquivalentTo(rsaKeyPair.PublicKey);
                encryptor.Configuration.Keys.PrivateKey.Should().BeEquivalentTo(rsaKeyPair.PrivateKey);
            }
        }

        public class Create_Configuration : RsaEncryptorFixture
        {
            [Fact]
            public void Should_Throw_When_Configuration_Null()
            {
                Invoking(() =>
                {
                    _ = RsaEncryptor.Create((IRsaEncryptorConfiguration) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("configuration");
            }

            [Fact]
            public void Should_Return_Configured_Encryptor()
            {
                var configuration = new RsaEncryptorConfiguration();

                var encryptor = RsaEncryptor.Create(configuration);

                encryptor.Should().BeOfType<RsaEncryptor>();

                encryptor.Configuration.Keys.PublicKey.Should().BeEquivalentTo(configuration.Keys.PublicKey);
                encryptor.Configuration.Keys.PrivateKey.Should().BeEquivalentTo(configuration.Keys.PrivateKey);
            }
        }
    }
}
