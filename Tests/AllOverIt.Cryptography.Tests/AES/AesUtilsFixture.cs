using AllOverIt.Cryptography.AES;
using AllOverIt.Fixture;
using FluentAssertions;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.Tests.AES
{
    public class AesUtilsFixture : FixtureBase
    {
        public class GetLegalKeySizes : AesUtilsFixture
        {
            [Fact]
            public void Should_Get_Legal_Key_Sizes()
            {
                var actual = AesUtils.GetLegalKeySizes();

                actual.Should().BeEquivalentTo(new[]
                {
                    new KeySizes(128, 256, 64)
                });
            }
        }

        public class IsKeySizeValid : AesUtilsFixture
        {
            [Theory]
            [InlineData(64, false)]
            [InlineData(96, false)]
            [InlineData(128, true)]
            [InlineData(192, true)]
            [InlineData(256, true)]
            [InlineData(320, false)]
            public void Should_Determine_If_Key_Is_Valid(int keySize, bool expected)
            {
                var actual = AesUtils.IsKeySizeValid(keySize);

                actual.Should().Be(expected);
            }
        }

        public class GenerateKey : AesUtilsFixture
        {
            [Theory]
            [InlineData(128)]
            [InlineData(192)]
            [InlineData(256)]
            public void Should_Generate_Key(int keySize)
            {
                var actual = AesUtils.GenerateKey(keySize);

                actual.Length.Should().Be(keySize / 8);
            }

            [Fact]
            public void Should_Generate_Different_Key()
            {
                var actual1 = AesUtils.GenerateKey(256);
                var actual2 = AesUtils.GenerateKey(256);

                actual1.Should().NotBeEquivalentTo(actual2);
            }
        }

        public class GenerateIV : AesUtilsFixture
        {
            [Fact]
            public void Should_Generate_IV()
            {
                var actual = AesUtils.GenerateIV();

                actual.Length.Should().Be(16);
            }

            [Fact]
            public void Should_Generate_Different_IV()
            {
                var actual1 = AesUtils.GenerateIV();
                var actual2 = AesUtils.GenerateIV();

                actual1.Should().NotBeEquivalentTo(actual2);
            }
        }
    }
}
