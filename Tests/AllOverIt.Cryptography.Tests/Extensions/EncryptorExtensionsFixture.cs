using AllOverIt.Cryptography.AES;
using AllOverIt.Cryptography.Extensions;
using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Buffers.Text;
using System.IO;
using System.Linq;
using Xunit;

namespace AllOverIt.Cryptography.Tests.Extensions
{
    public class EncryptorExtensionsFixture : FixtureBase
    {
        public class EncryptStreamToBytes_DecryptBytesToStream : EncryptorExtensionsFixture
        {
            public class EncryptStreamToBytes : EncryptStreamToBytes_DecryptBytesToStream
            {
                [Fact]
                public void Should_Throw_When_Encryptor_Null()
                {
                    Invoking(() =>
                    {
                        _ = EncryptorExtensions.EncryptStreamToBytes(null, Stream.Null);
                    })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("encryptor");
                }

                [Fact]
                public void Should_Throw_When_PlainTextStream_Null()
                {
                    Invoking(() =>
                    {
                        _ = EncryptorExtensions.EncryptStreamToBytes(A.Fake<IStreamEncryptor>(), null);
                    })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("plainTextStream");
                }
            }

            public class DecryptBytesToStream : EncryptStreamToBytes_DecryptBytesToStream
            {
                [Fact]
                public void Should_Throw_When_Encryptor_Null()
                {
                    Invoking(() =>
                    {
                        EncryptorExtensions.DecryptBytesToStream(null, CreateRandomBytes(), Stream.Null);
                    })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("encryptor");
                }

                [Fact]
                public void Should_Throw_When_CipherTextBytes_Null()
                {
                    Invoking(() =>
                    {
                        EncryptorExtensions.DecryptBytesToStream(A.Fake<IStreamEncryptor>(), null, Stream.Null);
                    })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("cipherTextBytes");
                }

                [Fact]
                public void Should_Throw_When_PlainTextStream_Null()
                {
                    Invoking(() =>
                    {
                        EncryptorExtensions.DecryptBytesToStream(A.Fake<IStreamEncryptor>(), CreateRandomBytes(), null);
                    })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("plainTextStream");
                }
            }

            [Fact]
            public void Should_Encrypt_Decrpyt()
            {
                var aes = new AesEncryptorFactory().Create();

                var sourceBytes = CreateRandomBytes();

                using (var sourceStream = new MemoryStream(sourceBytes))
                {
                    var cipherText = aes.EncryptStreamToBytes(sourceStream);

                    using (var plainTextStream = new MemoryStream())
                    {
                        aes.DecryptBytesToStream(cipherText, plainTextStream);

                        plainTextStream.ToArray().Should().BeEquivalentTo(sourceBytes);
                    }
                }
            }
        }

        public class EncryptStreamToBase64_DecryptBase64ToStream : EncryptorExtensionsFixture
        {
            public class EncryptStreamToBase64 : EncryptStreamToBase64_DecryptBase64ToStream
            {
                [Fact]
                public void Should_Throw_When_Encryptor_Null()
                {
                    Invoking(() =>
                    {
                        _ = EncryptorExtensions.EncryptStreamToBase64(null, Stream.Null);
                    })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("encryptor");
                }

                [Fact]
                public void Should_Throw_When_PlainTextStream_Null()
                {
                    Invoking(() =>
                    {
                        _ = EncryptorExtensions.EncryptStreamToBase64(A.Fake<IStreamEncryptor>(), null);
                    })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("plainTextStream");
                }
            }

            public class DecryptBase64ToStream : EncryptStreamToBase64_DecryptBase64ToStream
            {
                [Fact]
                public void Should_Throw_When_Encryptor_Null()
                {
                    Invoking(() =>
                    {
                        EncryptorExtensions.DecryptBase64ToStream(null, CreateRandomBase64(), Stream.Null);
                    })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("encryptor");
                }

                [Fact]
                public void Should_Throw_When_CipherTextBase64_Null()
                {
                    Invoking(() =>
                    {
                        EncryptorExtensions.DecryptBase64ToStream(A.Fake<IStreamEncryptor>(), null, Stream.Null);
                    })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("cipherTextBase64");
                }

                [Fact]
                public void Should_Throw_When_PlainTextStream_Null()
                {
                    Invoking(() =>
                    {
                        EncryptorExtensions.DecryptBase64ToStream(A.Fake<IStreamEncryptor>(), CreateRandomBase64(), null);
                    })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("plainTextStream");
                }
            }

            [Fact]
            public void Should_Encrypt_Decrpyt()
            {
                var aes = new AesEncryptorFactory().Create();

                var sourceBytes = CreateRandomBytes();

                using (var sourceStream = new MemoryStream(sourceBytes))
                {
                    var cipherText = aes.EncryptStreamToBase64(sourceStream);

                    IsValidBase64(cipherText).Should().BeTrue();

                    using (var plainTextStream = new MemoryStream())
                    {
                        aes.DecryptBase64ToStream(cipherText, plainTextStream);

                        plainTextStream.ToArray().Should().BeEquivalentTo(sourceBytes);
                    }
                }
            }
        }

        public class EncryptBytesToBase64_DecryptBase64ToBytes : EncryptorExtensionsFixture
        {
            public class EncryptBytesToBase64 : EncryptBytesToBase64_DecryptBase64ToBytes
            {
                [Fact]
                public void Should_Throw_When_Encryptor_Null()
                {
                    Invoking(() =>
                    {
                        _ = EncryptorExtensions.EncryptBytesToBase64(null, CreateRandomBytes());
                    })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("encryptor");
                }

                [Fact]
                public void Should_Throw_When_PlainTextBytes_Null()
                {
                    Invoking(() =>
                    {
                        _ = EncryptorExtensions.EncryptBytesToBase64(A.Fake<IEncryptor>(), null);
                    })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("plainTextBytes");
                }
            }

            public class DecryptBase64ToBytes : EncryptBytesToBase64_DecryptBase64ToBytes
            {
                [Fact]
                public void Should_Throw_When_Encryptor_Null()
                {
                    Invoking(() =>
                    {
                        EncryptorExtensions.DecryptBase64ToBytes(null, CreateRandomBase64());
                    })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("encryptor");
                }

                [Fact]
                public void Should_Throw_When_CipherTextBase64_Null()
                {
                    Invoking(() =>
                    {
                        EncryptorExtensions.DecryptBase64ToBytes(A.Fake<IEncryptor>(), null);
                    })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("cipherTextBase64");
                }
            }

            [Fact]
            public void Should_Encrypt_Decrpyt()
            {
                var aes = new AesEncryptorFactory().Create();

                var sourceBytes = CreateRandomBytes();

                var cipherText = aes.EncryptBytesToBase64(sourceBytes);

                IsValidBase64(cipherText).Should().BeTrue();

                var plainTextBytes = aes.DecryptBase64ToBytes(cipherText);

                plainTextBytes.Should().BeEquivalentTo(sourceBytes);
            }
        }

        public class EncryptBytesToStream_DecryptStreamToBytes : EncryptorExtensionsFixture
        {
            public class EncryptBytesToStream : EncryptBytesToStream_DecryptStreamToBytes
            {
                [Fact]
                public void Should_Throw_When_Encryptor_Null()
                {
                    Invoking(() =>
                    {
                        EncryptorExtensions.EncryptBytesToStream(null, CreateRandomBytes(), Stream.Null);
                    })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("encryptor");
                }

                [Fact]
                public void Should_Throw_When_PlainTextBytes_Null()
                {
                    Invoking(() =>
                    {
                        EncryptorExtensions.EncryptBytesToStream(A.Fake<IStreamEncryptor>(), null, Stream.Null);
                    })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("plainTextBytes");
                }

                [Fact]
                public void Should_Throw_When_CipherTextStream_Null()
                {
                    Invoking(() =>
                    {
                        EncryptorExtensions.EncryptBytesToStream(A.Fake<IStreamEncryptor>(), CreateRandomBytes(), null);
                    })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("cipherTextStream");
                }
            }

            public class DecryptStreamToBytes : EncryptBytesToStream_DecryptStreamToBytes
            {
                [Fact]
                public void Should_Throw_When_Encryptor_Null()
                {
                    Invoking(() =>
                    {
                        _ = EncryptorExtensions.DecryptStreamToBytes(null, Stream.Null);
                    })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("encryptor");
                }

                [Fact]
                public void Should_Throw_When_CipherTextBase64_Null()
                {
                    Invoking(() =>
                    {
                        _ = EncryptorExtensions.DecryptStreamToBytes(A.Fake<IStreamEncryptor>(), null);
                    })
                   .Should()
                   .Throw<ArgumentNullException>()
                   .WithNamedMessageWhenNull("cipherTextStream");
                }
            }

            [Fact]
            public void Should_Encrypt_Decrpyt()
            {
                var aes = new AesEncryptorFactory().Create();

                var sourceBytes = CreateRandomBytes();

                using (var cipherTextStream = new MemoryStream())
                {
                    aes.EncryptBytesToStream(sourceBytes, cipherTextStream);

                    cipherTextStream.Position = 0;

                    var plainTextBytes = aes.DecryptStreamToBytes(cipherTextStream);

                    plainTextBytes.ToArray().Should().BeEquivalentTo(sourceBytes);
                }
            }
        }







        private byte[] CreateRandomBytes()
        {
            return CreateMany<byte>().ToArray();
        }

        private string CreateRandomBase64()
        {
            return Create<string>().ToBase64();
        }

        private bool IsValidBase64(string base64)
        {
            var buffer = new byte[base64.Length];

            return Convert.TryFromBase64String(base64, buffer, out var _);
        }
    }
}
