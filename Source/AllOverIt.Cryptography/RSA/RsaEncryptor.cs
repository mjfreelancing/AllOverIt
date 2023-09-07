using AllOverIt.Assertion;
using AllOverIt.Cryptography.RSA.Exceptions;
using AllOverIt.Extensions;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace AllOverIt.Cryptography.RSA
{
    /// <inheritdoc cref="IRsaEncryptor"/>
    public sealed class RsaEncryptor : IRsaEncryptor
    {
        private readonly IRsaFactory _rsaFactory;
        
        private int? _maxInputLength;

        /// <inheritdoc />
        public IRsaEncryptorConfiguration Configuration { get; }

        /// <summary>Constructor. Applies a default <see cref="RsaEncryptorConfiguration"/> with ephemeral RSA public and private keys.</summary>
        public RsaEncryptor()
            : this(new RsaFactory(), new RsaEncryptorConfiguration())
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="configuration">Provides the RSA encryption / decryption options.</param>
        public RsaEncryptor(IRsaEncryptorConfiguration configuration)
            : this(new RsaFactory(), configuration)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="publicKey">The RSA public key to use. This can be <see langword="null"/> (or empty) if it is not required, such as when performing a
        /// decryption operation.</param>
        /// <param name="privateKey">>The RSA private key to use. This can be <see langword="null"/> (or empty) if it is not required, such as when performing
        /// an encryption operation.</param>
        /// <remarks>At least one of the public / private keys must be provided.</remarks>
        public RsaEncryptor(byte[] publicKey, byte[] privateKey)
            : this(new RsaEncryptorConfiguration(publicKey, privateKey))
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="publicKeyBase64">The RSA public key to use, in base64 format. This can be <see langword="null"/> (or empty) if it is not required,
        /// such as when performing a decryption operation.</param>
        /// <param name="privateKeyBase64">The RSA private key to use, in base64 format. This can be <see langword="null"/> (or empty) if it is not required,
        /// such as when performing an encryption operation.</param>
        /// <remarks>At least one of the public / private keys must be provided.</remarks>
        public RsaEncryptor(string publicKeyBase64, string privateKeyBase64)
            : this(new RsaEncryptorConfiguration(publicKeyBase64, privateKeyBase64))
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="rsaKeyPair">The RSA public / private key pair to use with encryption or decryption operations.</param>
        public RsaEncryptor(RsaKeyPair rsaKeyPair)
            : this(new RsaEncryptorConfiguration(rsaKeyPair))
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="parameters">The parameters for the <see cref="RSAAlgorithm"/> algorithm.</param>
        public RsaEncryptor(RSAParameters parameters)
            : this(new RsaEncryptorConfiguration(parameters))
        {
        }

        internal RsaEncryptor(IRsaFactory rsaFactory, IRsaEncryptorConfiguration configuration)
        {
            _rsaFactory = rsaFactory.WhenNotNull(nameof(rsaFactory));
            Configuration = configuration.WhenNotNull(nameof(configuration));
        }

        /// <inheritdoc />
        public int GetMaxInputLength()
        {
            Throw<RsaException>.WhenNull(Configuration.Keys.PublicKey, "An RSA public key has not been configured.");

            if (!_maxInputLength.HasValue)
            {
                using (var rsa = _rsaFactory.Create())
                {
                    rsa.ImportRSAPublicKey(Configuration.Keys.PublicKey, out _);

                    _maxInputLength = RsaUtils.GetMaxInputLength(rsa.KeySize, Configuration.Padding);
                }
            }

            return _maxInputLength.Value;
        }

        /// <inheritdoc />
        public byte[] Encrypt(byte[] plainText)
        {
            _ = plainText.WhenNotNullOrEmpty(nameof(plainText));

            Throw<RsaException>.WhenNull(Configuration.Keys.PublicKey, "An RSA public key has not been configured.");

            using (var rsa = _rsaFactory.Create())
            {
                rsa.ImportRSAPublicKey(Configuration.Keys.PublicKey, out _);

                return rsa.Encrypt(plainText, Configuration.Padding);
            }
        }

        /// <inheritdoc />
        public byte[] Decrypt(byte[] cipherText)
        {
            _ = cipherText.WhenNotNullOrEmpty(nameof(cipherText));

            Throw<RsaException>.WhenNull(Configuration.Keys.PrivateKey, "An RSA private key has not been configured.");

            using (var rsa = _rsaFactory.Create())
            {
                rsa.ImportRSAPrivateKey(Configuration.Keys.PrivateKey, out _);

                return rsa.Decrypt(cipherText, Configuration.Padding);
            }
        }

        /// <inheritdoc />
        public void Encrypt(Stream plainTextStream, Stream cipherTextStream)
        {
            _ = plainTextStream.WhenNotNull(nameof(plainTextStream));
            _ = cipherTextStream.WhenNotNull(nameof(cipherTextStream));

            var plainTextBytes = plainTextStream.ToByteArray();

            var cipherTextBytes = Encrypt(plainTextBytes);              // May throw RsaException

            cipherTextStream.FromByteArray(cipherTextBytes);
        }

        /// <inheritdoc />
        public void Decrypt(Stream cipherTextStream, Stream plainTextStream)
        {
            _ = cipherTextStream.WhenNotNull(nameof(cipherTextStream));
            _ = plainTextStream.WhenNotNull(nameof(plainTextStream));

            var cipherTextBytes = cipherTextStream.ToByteArray();

            var plainTextBytes = Decrypt(cipherTextBytes);              // May throw RsaException

            plainTextStream.FromByteArray(plainTextBytes);
        }
    }
}
