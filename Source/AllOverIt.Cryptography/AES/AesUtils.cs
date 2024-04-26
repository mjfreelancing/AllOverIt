using System.Security.Cryptography;

namespace AllOverIt.Cryptography.AES
{
    /// <summary>Provides a number of AES related utilities.</summary>
    public static class AesUtils
    {
        /// <summary>Gets the key size, in bits, that are supported by the AES algorithm.</summary>
        /// <returns>An array that contains the supported key sizes.</returns>
        public static KeySizes[] GetLegalKeySizes()
        {
            using (var aes = Aes.Create())
            {
                return aes.LegalKeySizes;
            }
        }

        /// <summary>Indicates if the provided key size, in bits, is valid for the AES algorithm.</summary>
        /// <param name="keySize">The key size, in bits.</param>
        /// <returns><see langword="True"/> if the key size is valid, otherwise false.</returns>
        public static bool IsKeySizeValid(int keySize)
        {
            return GetLegalKeySizes().IsKeySizeValid(keySize);
        }

        /// <summary>Generates a random key using the provided key size, in bits, for use with the AES algorithm.</summary>
        /// <param name="keySize">The key size, in bits.</param>
        /// <returns>An array of bytes containing a new secret key.</returns>
        public static byte[] GenerateKey(int keySize)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = keySize;
                aes.GenerateKey();

                return aes.Key;
            }
        }

        /// <summary>Generates a random initialization vector for use with the AES algorithm.</summary>
        /// <returns>An array of bytes containing a new initialization vector.</returns>
        public static byte[] GenerateIV()
        {
            using (var aes = Aes.Create())
            {
                aes.GenerateIV();

                return aes.IV;
            }
        }
    }
}
