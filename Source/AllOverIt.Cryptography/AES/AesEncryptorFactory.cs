﻿using AllOverIt.Assertion;

namespace AllOverIt.Cryptography.AES
{
    /// <summary>Implements a factory that creates <see cref="IAesEncryptor"/> instances.</summary>
    public sealed class AesEncryptorFactory : IAesEncryptorFactory
    {
        /// <inheritdoc />
        public IAesEncryptor Create()
        {
            return new AesEncryptor();
        }

        /// <inheritdoc />
        public IAesEncryptor Create(byte[] key, byte[] iv)
        {
            _ = key.WhenNotNullOrEmpty();
            _ = iv.WhenNotNullOrEmpty();

            return new AesEncryptor(key, iv);
        }
    }
}
