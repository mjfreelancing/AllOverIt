using AllOverIt.Cryptography.RSA;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.Tests.RSA
{
    public class RsaUtilsFixture : FixtureBase
    {
        public class GetMaxInputLength : RsaUtilsFixture
        {
            [Fact]
            public void Should_Calculate_Max_Input_Length()
            {
                // Based on a key size of 2048 bits
                var expected = new Dictionary<RSAEncryptionPadding, int>
                {
                    { RSAEncryptionPadding.Pkcs1, 245 },
                    { RSAEncryptionPadding.OaepSHA1, 214 },
                    { RSAEncryptionPadding.OaepSHA256, 190 },
                    { RSAEncryptionPadding.OaepSHA384, 158 },
                    { RSAEncryptionPadding.OaepSHA512, 126 }
                };

                var actual = expected.Keys.Select(key => RsaUtils.GetMaxInputLength(2048, key));

                actual.Should().BeEquivalentTo(expected.Values);
            }

        }

        public class GetLegalKeySizes : RsaUtilsFixture
        {
            [Fact]
            public void Should_Get_Legal_Key_Sizes()
            {
                var expected = new[]
                {
                    new KeySizes(512, 16384, 64)
                };

                var actual = RsaUtils.GetLegalKeySizes();

                actual.Should().BeEquivalentTo(expected);
            }
        }

        public class IsKeySizeValid : RsaUtilsFixture
        {
            [Theory]
            [InlineData(256, false)]
            [InlineData(512, true)]
            [InlineData(512 + 32, false)]
            [InlineData(16384, true)]
            [InlineData(16384 + 64, false)]
            public void Should_Get_Is_Legal_Key_Size(int keySize, bool expected)
            {
                RsaUtils.IsKeySizeValid(keySize).Should().Be(expected);
            }
        }
    }
}
