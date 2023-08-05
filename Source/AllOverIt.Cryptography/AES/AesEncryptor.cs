using AllOverIt.Assertion;
using System;
using System.IO;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.AES
{
    public sealed class AesEncryptor : IAesEncryptor
    {
        private readonly IAesFactory _aesFactory;

        public IAesEncryptionConfiguration Configuration { get; }

        public AesEncryptor()
            : this(AesFactory.Instance, new AesEncryptionConfiguration())
        {
        }

        public AesEncryptor(byte[] key, byte[] iv)
            : this(AesFactory.Instance, new AesEncryptionConfiguration(key, iv))
        {
        }

        public AesEncryptor(IAesEncryptionConfiguration configuration)
            : this(AesFactory.Instance, configuration)
        {
        }

        public AesEncryptor(IAesFactory aesFactory, IAesEncryptionConfiguration configuration)
        {
            _aesFactory = aesFactory.WhenNotNull(nameof(aesFactory));
            Configuration = configuration.WhenNotNull(nameof(configuration));
        }

#if !NETSTANDARD2_1
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

        public byte[] Decrypt(byte[] cipherText)
        {
            // TODO: Throw id Key / IV have not been set

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

        public void Decrypt(Stream source, Stream destination)
        {
            // TODO: Throw if Key / IV have not been set

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
