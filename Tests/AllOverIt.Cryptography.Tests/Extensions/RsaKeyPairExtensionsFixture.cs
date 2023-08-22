using AllOverIt.Cryptography.Extensions;
using AllOverIt.Cryptography.RSA;
using AllOverIt.Fixture;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Cryptography.Tests.Extensions
{
    public class RsaKeyPairExtensionsFixture : FixtureBase
    {
        private readonly RsaKeyPair _rsaKeyPair = new RsaKeyPair(System.Security.Cryptography.RSA.Create(3072));

        public class GetPublicKeyAsBase64 : RsaKeyPairExtensionsFixture
        {
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
            public void Should_Get_PrivateKey_As_Base64()
            {
                var expected = Convert.ToBase64String(_rsaKeyPair.PrivateKey);

                var actual = RsaKeyPairExtensions.GetPrivateKeyAsBase64(_rsaKeyPair);

                actual.Should().Be(expected);
            }
        }
    }
}