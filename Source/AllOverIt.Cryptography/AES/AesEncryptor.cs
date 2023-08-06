﻿using AllOverIt.Assertion;
using System;
using System.IO;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.AES
{
    /// <summary>A cryptographic implementation providing AES encryption and decryption operations.</summary>
    public sealed class AesEncryptor : IAesEncryptor
    {
        private readonly IAesFactory _aesFactory;

        /// <summary>Contains the AES encryption and decryption configuration options.</summary>
        public IAesEncryptionConfiguration Configuration { get; }

        /// <summary>Constructor. Uses a default configuration and a new random secret key and IV.</summary>
        public AesEncryptor()
            : this(AesFactory.Instance, new AesEncryptionConfiguration())
        {
        }

        /// <summary>Constructor. Uses a default configuration and the provided secret key and IV.</summary>
        /// <param name="key">The secret key to use.</param>
        /// <param name="iv">The initialization vector to use.</param>
        public AesEncryptor(byte[] key, byte[] iv)
            : this(AesFactory.Instance, new AesEncryptionConfiguration(key, iv))
        {
        }

        /// <summary>Constructor. Uses the provided configuration.</summary>
        /// <param name="configuration">The AES configuration to use.</param>
        public AesEncryptor(IAesEncryptionConfiguration configuration)
            : this(AesFactory.Instance, configuration)
        {
        }

        internal AesEncryptor(IAesFactory aesFactory, IAesEncryptionConfiguration configuration)
        {
            _aesFactory = aesFactory.WhenNotNull(nameof(aesFactory));
            Configuration = configuration.WhenNotNull(nameof(configuration));
        }

#if !NETSTANDARD2_1
        /// <inheritdoc />
        public int GetCipherTextLength(int plainTextLength)
        {
            using (var aes = _aesFactory.Create(Configuration))
            {
                return Configuration.Mode switch
                {
                    CipherMode.CBC => aes.GetCiphertextLengthCbc(plainTextLength, Configuration.Padding),
                    CipherMode.CFB => aes.GetCiphertextLengthCfb(plainTextLength, Configuration.Padding),
                    CipherMode.ECB => aes.GetCiphertextLengthEcb(plainTextLength, Configuration.Padding),

                    // TODO: custom exception
                    _ => throw new InvalidOperationException($"The {Configuration.Mode} cipher mode is not valid for the AES algorithm."),
                };
            }
        }
#endif

        /// <inheritdoc />
        public byte[] Encrypt(byte[] plainText)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var aes = _aesFactory.Create(Configuration))
                {
                    var encryptor = aes.CreateEncryptor();

                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainText, 0, plainText.Length);
                    }
                }

                return memoryStream.ToArray();
            }
        }

        /// <inheritdoc />
        public byte[] Decrypt(byte[] cipherText)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var aes = _aesFactory.Create(Configuration))
                {
                    var decryptor = aes.CreateDecryptor();

                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(cipherText, 0, cipherText.Length);
                    }
                }

                return memoryStream.ToArray();
            }
        }

        /// <inheritdoc />
        public void Encrypt(Stream source, Stream destination)
        {
            using (var aes = _aesFactory.Create(Configuration))
            {
                var encryptor = aes.CreateEncryptor();

                using (var cryptoStream = new CryptoStream(destination, encryptor, CryptoStreamMode.Write))
                {
                    source.CopyTo(cryptoStream);
                }
            }
        }

        /// <inheritdoc />
        public void Decrypt(Stream source, Stream destination)
        {
            using (var aes = _aesFactory.Create(Configuration))
            {
                var decryptor = aes.CreateDecryptor();

                using (var cryptoStream = new CryptoStream(destination, decryptor, CryptoStreamMode.Write))
                {
                    source.CopyTo(cryptoStream);
                }
            }
        }       
    }
}
