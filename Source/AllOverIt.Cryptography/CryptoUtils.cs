using System.Linq;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography
{
    /// <summary>Contains cryptography related utilities.</summary>
    public static class CryptoUtils
    {
        /// <summary>Determines if a specified <paramref name="keySize"/> is valid based on the provided <paramref name="legalKeySizes"/>.</summary>
        /// <param name="legalKeySizes">The valid key size definitions.</param>
        /// <param name="keySize">The key size to validate against the provided legal size definitions.</param>
        /// <returns><see langword="True"/> if the key size is valid, otherwise <see langword="False"/>.</returns>
        public static bool IsKeySizeValid(this KeySizes[] legalKeySizes, int keySize)
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
