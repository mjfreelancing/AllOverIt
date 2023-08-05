using System.Linq;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography
{
    public static class CryptoUtils
    {
        public static bool IsKeySizeValid(KeySizes[] legalKeySizes, int keySize)
        {
            static bool IsValid(int min, int max, int skip, int value)
            {
                if (value < min || value > max)
                {
                    return false;
                }

                if ((value - min) % skip != 0)
                {
                    return false;
                }

                return true;
            }

            var isValid = legalKeySizes.FirstOrDefault(legalKeySize => IsValid(legalKeySize.MinSize, legalKeySize.MaxSize, legalKeySize.SkipSize, keySize));

            return isValid is not null;
        }
    }
}
