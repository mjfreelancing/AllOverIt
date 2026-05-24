using AllOverIt.Fixture;
using Shouldly;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.Tests
{
    public class CryptoUtilsFixture : FixtureBase
    {
        public class IsKeySizeValid : CryptoUtilsFixture
        {
            [Fact]
            public void Should_Determine_If_Key_Size_Is_Valid()
            {
                var minSize1 = GetWithinRange(1, 16) * 8;
                var skipSize = GetWithinRange(1, 4) * 4;
                var maxSize1 = minSize1 + GetWithinRange(1, 4) * skipSize;

                var minSize2 = minSize1 * 2;
                var maxSize2 = minSize2 + GetWithinRange(1, 4) * skipSize;

                var keySizes1 = new KeySizes(minSize1, maxSize1, skipSize);
                var keySizes2 = new KeySizes(minSize2, maxSize2, skipSize);

                var legalKeySizes = new[] { keySizes1, keySizes2 };

                legalKeySizes.IsKeySizeValid(minSize1 - 1).ShouldBeFalse();
                legalKeySizes.IsKeySizeValid(minSize1).ShouldBeTrue();
                legalKeySizes.IsKeySizeValid(minSize1 + skipSize).ShouldBeTrue();
                legalKeySizes.IsKeySizeValid(maxSize1).ShouldBeTrue();
                legalKeySizes.IsKeySizeValid(maxSize1 - skipSize).ShouldBeTrue();
                legalKeySizes.IsKeySizeValid(maxSize1 + 1).ShouldBeFalse();

                legalKeySizes.IsKeySizeValid(minSize2 - 1).ShouldBeFalse();
                legalKeySizes.IsKeySizeValid(minSize2).ShouldBeTrue();
                legalKeySizes.IsKeySizeValid(minSize2 + skipSize).ShouldBeTrue();
                legalKeySizes.IsKeySizeValid(maxSize2).ShouldBeTrue();
                legalKeySizes.IsKeySizeValid(maxSize2 - skipSize).ShouldBeTrue();
                legalKeySizes.IsKeySizeValid(maxSize2 + 1).ShouldBeFalse();
            }
        }
    }
}
