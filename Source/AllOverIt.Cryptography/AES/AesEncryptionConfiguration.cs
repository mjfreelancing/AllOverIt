using AllOverIt.Assertion;
using AllOverIt.Cryptography.AES.Exceptions;
using System;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.AES
{
    /// <summary>Provides configuration options for AES encryption and decryption.</summary>
    public sealed class AesEncryptionConfiguration : IAesEncryptionConfiguration
    {
        /// <inheritdoc />
        public CipherMode Mode { get; init; } = CipherMode.CBC;

        /// <inheritdoc />
        public PaddingMode Padding { get; init; } = PaddingMode.PKCS7;

        /// <inheritdoc />
        public int KeySize { get; init; } = 256;

        /// <inheritdoc />
        public int BlockSize { get; init; } = 128;

        /// <inheritdoc />
        public int FeedbackSize { get; init; } = 8;

        /// <inheritdoc />
        public byte[] Key { get; private set; }

        /// <inheritdoc />
        public byte[] IV { get; private set; }

        /// <summary>Constructor. Uses a default configuration (see <see cref="IAesEncryptionConfiguration"/>) with
        /// a random secrey key and initialization vector.</summary>
        public AesEncryptionConfiguration()
        {
            RegenerateKeyAndIV();
        }

        /// <summary>Constructor. Uses a default configuration (see <see cref="IAesEncryptionConfiguration"/>) with
        /// the provided secrey key and initialization vector. The key size will be updated to match the length of
        /// the secret key.</summary>
        public AesEncryptionConfiguration(byte[] key, byte[] iv)
        {
            _ = iv.WhenNotNull(nameof(iv));
            _ = key.WhenNotNull(nameof(key));

            var keySizeBits = key.Length * 8;

            Throw<AesException>.When(!AesUtils.IsKeySizeValid(keySizeBits), $"AES Key size {keySizeBits} is invalid.");
            Throw<AesException>.When(iv.Length != 16, "The AES Initialization Vector must be 16 bytes.");

            Key = key;                      // The length will be validated
            KeySize = keySizeBits;

            IV = iv;
        }

        /// <inheritdoc />
        public void RegenerateKey()
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = KeySize;          // The aes.Key will be updated if this is not the default
                Key = aes.Key;
            }
        }

        /// <inheritdoc />
        public void RegenerateIV()
        {
            using (var aes = Aes.Create())
            {
                IV = aes.IV;
            }
        }

        /// <inheritdoc />
        public void RegenerateKeyAndIV()
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = KeySize;          // The aes.Key will be updated if this is not the default
                Key = aes.Key;
                IV = aes.IV;
            }
        }
    }
}
