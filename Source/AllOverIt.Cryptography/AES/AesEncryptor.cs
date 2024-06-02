using AllOverIt.Assertion;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.AES
{
    /// <summary>A cryptographic implementation providing AES encryption and decryption operations.</summary>
    public sealed class AesEncryptor : IAesEncryptor
    {
        private readonly IAesFactory _aesFactory;

        /// <summary>Contains the AES encryption and decryption configuration options.</summary>
        public IAesEncryptorConfiguration Configuration { get; }

        /// <summary>Constructor. Uses a default configuration and a new random secret key and IV.</summary>
        public AesEncryptor()
            : this(AesFactory.Instance, new AesEncryptorConfiguration())
        {
        }

        /// <summary>Constructor. Uses a default configuration and the provided secret key and IV.</summary>
        /// <param name="key">The secret key to use.</param>
        /// <param name="iv">The initialization vector to use.</param>
        public AesEncryptor(byte[] key, byte[] iv)
            : this(AesFactory.Instance, new AesEncryptorConfiguration(key, iv))
        {
        }

        /// <summary>Constructor. Uses the provided configuration.</summary>
        /// <param name="configuration">The AES configuration to use.</param>
        public AesEncryptor(IAesEncryptorConfiguration configuration)
            : this(AesFactory.Instance, configuration)
        {
        }

        private AesEncryptor(IAesFactory aesFactory, IAesEncryptorConfiguration configuration)
        {
            _aesFactory = aesFactory.WhenNotNull(nameof(aesFactory));
            Configuration = configuration.WhenNotNull(nameof(configuration));
        }

#if !NETSTANDARD2_1
        /// <inheritdoc />
        public int GetCipherTextLength(int plainTextLength)
        {
            using var aes = _aesFactory.Create(Configuration);

            // AES cannot be created for modes other than those listed below
            return Configuration.Mode switch
            {
                CipherMode.CBC => aes.GetCiphertextLengthCbc(plainTextLength, Configuration.Padding),
                CipherMode.CFB => aes.GetCiphertextLengthCfb(plainTextLength, Configuration.Padding),
                CipherMode.ECB => aes.GetCiphertextLengthEcb(plainTextLength, Configuration.Padding),

                _ => throw new InvalidOperationException($"Unexpected cipher mode '{Configuration.Mode}' for the AES algorithm."),
            };
        }
#endif

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0079:Remove unnecessary suppression", Justification = "Silence Code Analysis")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "Code readability")]
        public byte[] Encrypt(byte[] plainText)
        {
            _ = plainText.WhenNotNull(nameof(plainText));

            using (var memoryStream = new MemoryStream())
            {
                // Aes will throw if not appropriately configured
                using (var aes = _aesFactory.Create(Configuration))
                {
                    var encryptor = aes.CreateEncryptor();

                    using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

                    cryptoStream.Write(plainText, 0, plainText.Length);
                }

                return memoryStream.ToArray();
            }
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0079:Remove unnecessary suppression", Justification = "Silence Code Analysis")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "Code readability")]
        public byte[] Decrypt(byte[] cipherText)
        {
            _ = cipherText.WhenNotNullOrEmpty(nameof(cipherText));

            using (var memoryStream = new MemoryStream())
            {
                // Aes will throw if not appropriately configured
                using (var aes = _aesFactory.Create(Configuration))
                {
                    var decryptor = aes.CreateDecryptor();

                    using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write);

                    cryptoStream.Write(cipherText, 0, cipherText.Length);
                }

                return memoryStream.ToArray();
            }
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0079:Remove unnecessary suppression", Justification = "Silence Code Analysis")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "Code readability")]
        public void Encrypt(Stream plainTextStream, Stream cipherTextStream)
        {
            _ = plainTextStream.WhenNotNull(nameof(plainTextStream));
            _ = cipherTextStream.WhenNotNull(nameof(cipherTextStream));

            // Aes will throw if not appropriately configured
            using (var aes = _aesFactory.Create(Configuration))
            {
                var encryptor = aes.CreateEncryptor();

                using var cryptoStream = new CryptoStream(cipherTextStream, encryptor, CryptoStreamMode.Write, true);

                plainTextStream.CopyTo(cryptoStream);
            }
        }

        /// <inheritdoc />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0079:Remove unnecessary suppression", Justification = "Silence Code Analysis")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "Code readability")]
        public void Decrypt(Stream cipherTextStream, Stream plainTextStream)
        {
            _ = cipherTextStream.WhenNotNull(nameof(cipherTextStream));
            _ = plainTextStream.WhenNotNull(nameof(plainTextStream));

            // Aes will throw if not appropriately configured
            using (var aes = _aesFactory.Create(Configuration))
            {
                var decryptor = aes.CreateDecryptor();

                using var cryptoStream = new CryptoStream(cipherTextStream, decryptor, CryptoStreamMode.Read, true);

                cryptoStream.CopyTo(plainTextStream);
            }
        }
    }
}
