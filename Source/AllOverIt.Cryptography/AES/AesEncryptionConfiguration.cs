using AllOverIt.Assertion;
using System;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.AES
{
    public sealed class AesEncryptionConfiguration : IAesEncryptionConfiguration
    {
        public CipherMode Mode { get; init; } = CipherMode.CBC;
        public PaddingMode Padding { get; init; } = PaddingMode.PKCS7;
        public int KeySize { get; init; } = 256;
        public int BlockSize { get; init; } = 128;
        public int FeedbackSize { get; init; } = 8;
        public byte[] Key { get; private set; }
        public byte[] IV { get; private set; }

        public AesEncryptionConfiguration()
        {
            RegenerateKeyAndIV();
        }

        public AesEncryptionConfiguration(byte[] key, byte[] iv)
        {
            SetKey(key);
            SetIV(iv);
        }

        public void RegenerateKey()
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = KeySize;          // The aes.Key will be updated if this is not the default
                Key = aes.Key;
            }
        }

        public void RegenerateIV()
        {
            using (var aes = Aes.Create())
            {
                IV = aes.IV;
            }
        }

        public void RegenerateKeyAndIV()
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = KeySize;          // The aes.Key will be updated if this is not the default
                Key = aes.Key;
                IV = aes.IV;
            }
        }

        private void SetKey(byte[] key)
        {
            _ = key.WhenNotNull(nameof(key));

            // TODO: Custom exceptions
            Throw<InvalidOperationException>.When(AesUtils.IsKeySizeValid(key.Length), $"AES Key size {key.Length} is invalid.");

            Key = key;
        }

        private void SetIV(byte[] iv)
        {
            _ = iv.WhenNotNull(nameof(iv));

            // TODO: Custom exceptions
            Throw<InvalidOperationException>.When(iv.Length != 16, "The AES Initialization Vector must be 16 bytes.");

            IV = iv;
        }
    }
}
