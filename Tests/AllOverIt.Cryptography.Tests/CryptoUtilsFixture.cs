using AllOverIt.Fixture;
using FluentAssertions;
using System.Security.Cryptography;
using Xunit;

namespace AllOverIt.Cryptography.Tests
{
    public class CryptoUtilsFixture : FixtureBase
    {
        public class IsKeySizeValid : CryptoUtilsFixture
        {
            [Fact]
            public void Should_Determine_If_Key_Size_Is_Valid()
            {
                var minSize1 = Create<int>();
                var skipSize = GetWithinRange(4, 256);
                var maxSize1 = minSize1 + GetWithinRange(1, 4) * skipSize;

                var minSize2 = minSize1 * 2;
                var maxSize2 = minSize2 + GetWithinRange(1, 4) * skipSize;

                var keySizes1 = new KeySizes(minSize1, maxSize1, skipSize);
                var keySizes2 = new KeySizes(minSize2, maxSize2, skipSize);

                var legalKeySizes = new[] { keySizes1, keySizes2 };

                legalKeySizes.IsKeySizeValid(minSize1 - 1).Should().BeFalse();
                legalKeySizes.IsKeySizeValid(minSize1).Should().BeTrue();
                legalKeySizes.IsKeySizeValid(minSize1 + skipSize).Should().BeTrue();
                legalKeySizes.IsKeySizeValid(maxSize1).Should().BeTrue();
                legalKeySizes.IsKeySizeValid(maxSize1 - skipSize).Should().BeTrue();
                legalKeySizes.IsKeySizeValid(maxSize1 + 1).Should().BeFalse();

                legalKeySizes.IsKeySizeValid(minSize2 - 1).Should().BeFalse();
                legalKeySizes.IsKeySizeValid(minSize2).Should().BeTrue();
                legalKeySizes.IsKeySizeValid(minSize2 + skipSize).Should().BeTrue();
                legalKeySizes.IsKeySizeValid(maxSize2).Should().BeTrue();
                legalKeySizes.IsKeySizeValid(maxSize2 - skipSize).Should().BeTrue();
                legalKeySizes.IsKeySizeValid(maxSize2 + 1).Should().BeFalse();
            }
        }
    }
}
