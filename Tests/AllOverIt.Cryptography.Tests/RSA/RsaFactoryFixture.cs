using AllOverIt.Cryptography.RSA;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Security.Cryptography;
using Xunit;

namespace AllOverIt.Cryptography.Tests.RSA
{
    public class RsaFactoryFixture : FixtureBase
    {
        private readonly IRsaFactory _rsaFactory = new RsaFactory();

        public class Create : RsaFactoryFixture
        {
            [Fact]
            public void Should_Create_RSA()
            {
                var rsa1 = _rsaFactory.Create();
                var rsa2 = _rsaFactory.Create();

                rsa1.KeySize.Should().Be(2048);
                rsa2.KeySize.Should().Be(2048);

                var xml1 = rsa1.ToXmlString(true);
                var xml2 = rsa2.ToXmlString(true);

                xml1.Should().NotBe(xml2);
            }
        }

        public class Create_Key_Size : RsaFactoryFixture
        {
            [Fact]
            public void Should_Create_RSA()
            {
                var keySizeBits = GetWithinRange(1, 4) * 1024;

                var rsa1 = _rsaFactory.Create(keySizeBits);
                var rsa2 = _rsaFactory.Create(keySizeBits);

                rsa1.KeySize.Should().Be(keySizeBits);
                rsa2.KeySize.Should().Be(keySizeBits);

                var xml1 = rsa1.ToXmlString(true);
                var xml2 = rsa2.ToXmlString(true);

                xml1.Should().NotBe(xml2);
            }
        }

        public class Create_Key_Parameters : RsaFactoryFixture
        {
            [Fact]
            public void Should_Create_RSA()
            {
                var rsa1 = _rsaFactory.Create();

                var parameters = rsa1.ExportParameters(true);

                var rsa2 = _rsaFactory.Create(parameters);

                rsa1.KeySize.Should().Be(2048);
                rsa2.KeySize.Should().Be(2048);

                var xml1 = rsa1.ToXmlString(true);
                var xml2 = rsa2.ToXmlString(true);

                xml1.Should().Be(xml2);
            }
        }
    }
}
