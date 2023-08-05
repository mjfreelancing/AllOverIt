using System;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.AES
{
    public static class AesUtils
    {
        public static int GetCipherTextLength(int plainTextLength)
        {
            var numBlocks = (int)Math.Ceiling(plainTextLength / 16.0d);

            return numBlocks * 16;
        }

        public static KeySizes[] GetLegalKeySizes()
        {
            using (var aes = Aes.Create())
            {
                return aes.LegalKeySizes;
            }
        }

        public static bool IsKeySizeValid(int keySize)
        {
            return CryptoUtils.IsKeySizeValid(GetLegalKeySizes(), keySize);
        }


        public static byte[] GenerateIV()
        {
            using (var aes = Aes.Create())
            {
                aes.GenerateIV();

                return aes.IV;
            }
        }

        public static byte[] GenerateKey(int keySize)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = keySize;
                aes.GenerateKey();

                return aes.Key;
            }
        }
    }
}
