using AllOverIt.Cryptography.Extensions;
using AllOverIt.Cryptography.RSA;
using AllOverIt.Cryptography.RSA.Exceptions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using Shouldly;

using RSAAlgorithm = System.Security.Cryptography.RSA;

namespace AllOverIt.Cryptography.Tests.RSA
{
    public class RsaKeyPairFixture : FixtureBase
    {
        private readonly RsaKeyPair _rsaKeyPair = new RsaKeyPair();
        private readonly string _publicKeyBase64;
        private readonly string _privateKeyBase64;

        public RsaKeyPairFixture()
        {
            _publicKeyBase64 = _rsaKeyPair.GetPublicKeyAsBase64();
            _privateKeyBase64 = _rsaKeyPair.GetPrivateKeyAsBase64();
        }

        public class Constructor_KeySize : RsaKeyPairFixture
        {
            [Fact]
            public void Should_Create_Default()
            {
                var actual = new RsaKeyPair();

                actual.PublicKey.ShouldNotBeNullOrEmpty();
                actual.PrivateKey.ShouldNotBeNullOrEmpty();
                actual.KeySize.ShouldBe(3072);
            }

            [Fact]
            public void Should_Create_With_KeySize()
            {
                var keySize = RsaUtils.GetLegalKeySizes()[0].MinSize;

                var actual = new RsaKeyPair(keySize);

                actual.PublicKey.ShouldNotBeNullOrEmpty();
                actual.PrivateKey.ShouldNotBeNullOrEmpty();
                actual.KeySize.ShouldBe(keySize);
            }
        }

        public class Constructor_Byte : RsaKeyPairFixture
        {
            [Fact]
            public void Should_Throw_When_Both_Keys_Null()      // Empty is invalid as a key - the RSA will throw as 'ASN1 corrupted data'. Not adding tests for it.
            {
                Should.Throw<RsaException>(() =>
                {
                    _ = new RsaKeyPair((byte[]) null, null);
                })
                .WithMessage("At least one RSA key is required.");
            }

            [Fact]
            public void Should_Not_Throw_When_PublicKey_Null()
            {
                var actual = new RsaKeyPair(null, _rsaKeyPair.PrivateKey);

                actual.PrivateKey.ShouldBeSameAs(_rsaKeyPair.PrivateKey);
            }

            [Fact]
            public void Should_Not_Throw_When_PrivateKey_Null()
            {
                var actual = new RsaKeyPair(_rsaKeyPair.PublicKey, null);

                actual.PublicKey.ShouldBeSameAs(_rsaKeyPair.PublicKey);
            }

            [Fact]
            public void Should_Set_KeySize_From_PublicKey()
            {
                var actual = new RsaKeyPair(_rsaKeyPair.PublicKey, null);

                actual.KeySize.ShouldBe(_rsaKeyPair.KeySize);
            }

            [Fact]
            public void Should_Set_KeySize_From_PrivateKey()
            {
                var actual = new RsaKeyPair(null, _rsaKeyPair.PrivateKey);

                actual.KeySize.ShouldBe(_rsaKeyPair.KeySize);
            }
        }

        public class Constructor_Base64 : RsaKeyPairFixture
        {
            [Fact]
            public void Should_Throw_When_Both_Keys_Null()      // Empty is invalid as a key - the RSA will throw as 'ASN1 corrupted data'. Not adding tests for it.
            {
                Should.Throw<RsaException>(() =>
                {
                    _ = new RsaKeyPair((string) null, null);
                })
                .WithMessage("At least one RSA key is required.");
            }

            [Fact]
            public void Should_Not_Throw_When_PublicKey_Null()
            {
                var actual = new RsaKeyPair(null, _privateKeyBase64);

                actual.PrivateKey.ShouldBe(_rsaKeyPair.PrivateKey);
            }

            [Fact]
            public void Should_Not_Throw_When_PrivateKey_Null()
            {
                var actual = new RsaKeyPair(_publicKeyBase64, null);

                actual.PublicKey.ShouldBe(_rsaKeyPair.PublicKey);
            }

            [Fact]
            public void Should_Set_KeySize_From_PublicKey()
            {
                var actual = new RsaKeyPair(_publicKeyBase64, null);

                actual.KeySize.ShouldBe(_rsaKeyPair.KeySize);
            }

            [Fact]
            public void Should_Set_KeySize_From_PrivateKey()
            {
                var actual = new RsaKeyPair(null, _privateKeyBase64);

                actual.KeySize.ShouldBe(_rsaKeyPair.KeySize);
            }
        }

        public class Constructor_RSA : RsaKeyPairFixture
        {
            [Fact]
            public void Should_Throw_When_Both_Keys_Null()      // Empty is invalid as a key - the RSA will throw as 'ASN1 corrupted data'. Not adding tests for it.
            {
                Should.Throw<ArgumentNullException>(() =>
                {
                    _ = new RsaKeyPair((RSAAlgorithm) null);
                })
                .WithNamedMessageWhenNull("rsa");
            }

            [Fact]
            public void Should_Construct_From_RSA()
            {
                var rsa = RSAAlgorithm.Create();
                var publicKey = rsa.ExportRSAPublicKey();
                var privateKey = rsa.ExportRSAPrivateKey();

                var actual = new RsaKeyPair(rsa);

                actual.PublicKey.ShouldBe(publicKey);
                actual.PrivateKey.ShouldBe(privateKey);
                actual.KeySize.ShouldBe(rsa.KeySize);
            }
        }

        public class Constructor_RSAParameters : RsaKeyPairFixture
        {
            [Fact]
            public void Should_Construct_From_RSAParameters()
            {
                var rsa = RSAAlgorithm.Create();

                var publicKey = rsa.ExportRSAPublicKey();
                var privateKey = rsa.ExportRSAPrivateKey();

                var parameters = rsa.ExportParameters(true);
                var actual = new RsaKeyPair(parameters);

                actual.PublicKey.ShouldBe(publicKey);
                actual.PrivateKey.ShouldBe(privateKey);
                actual.KeySize.ShouldBe(rsa.KeySize);
            }
        }

        public class Constructor_XML : RsaKeyPairFixture
        {
            [Fact]
            public void Should_Construct_From_XML()
            {
                var rsa = RSAAlgorithm.Create();

                var publicKey = rsa.ExportRSAPublicKey();
                var privateKey = rsa.ExportRSAPrivateKey();

                var xml = rsa.ToXmlString(true);
                var actual = new RsaKeyPair(xml);

                actual.PublicKey.ShouldBe(publicKey);
                actual.PrivateKey.ShouldBe(privateKey);
                actual.KeySize.ShouldBe(rsa.KeySize);
            }
        }

        public class SetKeys_Byte : RsaKeyPairFixture
        {
            [Fact]
            public void Should_Throw_When_Both_Keys_Null()      // Empty is invalid as a key - the RSA will throw as 'ASN1 corrupted data'. Not adding tests for it.
            {
                Should.Throw<RsaException>(() =>
                {
                    var actual = new RsaKeyPair();

                    actual.SetKeys((byte[]) null, null);
                })
                .WithMessage("At least one RSA key is required.");
            }

            [Fact]
            public void Should_Not_Throw_When_PublicKey_Null()
            {
                var actual = new RsaKeyPair();

                actual.SetKeys(null, _rsaKeyPair.PrivateKey);

                actual.PrivateKey.ShouldBeSameAs(_rsaKeyPair.PrivateKey);
            }

            [Fact]
            public void Should_Not_Throw_When_PrivateKey_Null()
            {
                var actual = new RsaKeyPair();

                actual.SetKeys(_rsaKeyPair.PublicKey, null);

                actual.PublicKey.ShouldBeSameAs(_rsaKeyPair.PublicKey);
            }

            [Fact]
            public void Should_Set_KeySize_From_PublicKey()
            {
                var actual = new RsaKeyPair();

                actual.SetKeys(_rsaKeyPair.PublicKey, null);

                actual.KeySize.ShouldBe(_rsaKeyPair.KeySize);
            }

            [Fact]
            public void Should_Set_KeySize_From_PrivateKey()
            {
                var actual = new RsaKeyPair();

                actual.SetKeys(null, _rsaKeyPair.PrivateKey);

                actual.KeySize.ShouldBe(_rsaKeyPair.KeySize);
            }
        }

        public class SetKeys_Base64 : RsaKeyPairFixture
        {
            [Fact]
            public void Should_Throw_When_Both_Keys_Null()      // Empty is invalid as a key - the RSA will throw as 'ASN1 corrupted data'. Not adding tests for it.
            {
                Should.Throw<RsaException>(() =>
                {
                    var actual = new RsaKeyPair();

                    actual.SetKeys((string) null, null);
                })
                .WithMessage("At least one RSA key is required.");
            }

            [Fact]
            public void Should_Not_Throw_When_PublicKey_Null()
            {
                var actual = new RsaKeyPair();

                actual.SetKeys(null, _privateKeyBase64);

                actual.PrivateKey.ShouldBe(_rsaKeyPair.PrivateKey);
            }

            [Fact]
            public void Should_Not_Throw_When_PrivateKey_Null()
            {
                var actual = new RsaKeyPair();

                actual.SetKeys(_publicKeyBase64, null);

                actual.PublicKey.ShouldBe(_rsaKeyPair.PublicKey);
            }

            [Fact]
            public void Should_Set_KeySize_From_PublicKey()
            {
                var actual = new RsaKeyPair();

                actual.SetKeys(_publicKeyBase64, null);

                actual.KeySize.ShouldBe(_rsaKeyPair.KeySize);
            }

            [Fact]
            public void Should_Set_KeySize_From_PrivateKey()
            {
                var actual = new RsaKeyPair();

                actual.SetKeys(null, _privateKeyBase64);

                actual.KeySize.ShouldBe(_rsaKeyPair.KeySize);
            }
        }
    }
}
