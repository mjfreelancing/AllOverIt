using AllOverIt.Cryptography.Extensions;
using AllOverIt.Cryptography.RSA;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;

namespace AllOverIt.Cryptography.Tests.Extensions
{
    public class RsaKeyPairExtensionsFixture : FixtureBase
    {
        private readonly RsaKeyPair _rsaKeyPair = new RsaKeyPair(System.Security.Cryptography.RSA.Create(3072));

        public class GetPublicKeyAsBase64 : RsaKeyPairExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                {
                    _ = RsaKeyPairExtensions.GetPublicKeyAsBase64(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("rsaKeyPair");
            }

            [Fact]
            public void Should_Throw_When_PublicKey_Null()
            {
                Invoking(() =>
                {
                    var rsaKeyPair = new RsaKeyPair(null, _rsaKeyPair.PrivateKey);

                    _ = rsaKeyPair.GetPublicKeyAsBase64();
                })
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("The public key cannot be null.");
            }

            [Fact]
            public void Should_Get_PublicKey_As_Base64()
            {
                var expected = Convert.ToBase64String(_rsaKeyPair.PublicKey);

                var actual = RsaKeyPairExtensions.GetPublicKeyAsBase64(_rsaKeyPair);

                actual.Should().Be(expected);
            }
        }

        public class GetPrivateKeyAsBase64 : RsaKeyPairExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Null()
            {
                Invoking(() =>
                {
                    _ = RsaKeyPairExtensions.GetPrivateKeyAsBase64(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("rsaKeyPair");
            }

            [Fact]
            public void Should_Throw_When_PrivateKey_Null()
            {
                Invoking(() =>
                {
                    var rsaKeyPair = new RsaKeyPair(_rsaKeyPair.PublicKey, null);

                    _ = rsaKeyPair.GetPrivateKeyAsBase64();
                })
                .Should()
                .Throw<ArgumentException>()
                .WithMessage("The private key cannot be null.");
            }

            [Fact]
            public void Should_Get_PrivateKey_As_Base64()
            {
                var expected = Convert.ToBase64String(_rsaKeyPair.PrivateKey);

                var actual = RsaKeyPairExtensions.GetPrivateKeyAsBase64(_rsaKeyPair);

                actual.Should().Be(expected);
            }
        }
    }
}