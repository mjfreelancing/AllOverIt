using AllOverIt.Assertion;
using AllOverIt.Cryptography.AES.Exceptions;
using AllOverIt.Extensions;
using System.IO;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.RSA
{
    /// <inheritdoc cref="IRsaEncryptor"/>
    public sealed class RsaEncryptor : IRsaEncryptor
    {
        private readonly IRsaFactory _rsaFactory;
        
        private int? _maxInputLength;

        /// <inheritdoc />
        public IRsaEncryptorConfiguration Configuration { get; }

        /// <summary>Constructor.</summary>
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

            Throw<RsaException>.WhenNull(Configuration.Keys.PublicKey, "An RSA private key has not been configured.");

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

        public static IRsaEncryptor Create(string publicKeyBase64, string privateKeyBase64)
        {
            _ = publicKeyBase64.WhenNotNull(nameof(publicKeyBase64));
            _ = privateKeyBase64.WhenNotNull(nameof(privateKeyBase64));

            var configuration = new RsaEncryptorConfiguration
            {
                Keys = new RsaKeyPair(publicKeyBase64, privateKeyBase64)
            };

            return new RsaEncryptor(configuration);
        }

        public static IRsaEncryptor Create(RsaKeyPair rsaKeyPair)
        {
            _ = rsaKeyPair.WhenNotNull(nameof(rsaKeyPair));

            var configuration = new RsaEncryptorConfiguration
            {
                Keys = rsaKeyPair
            };

            return new RsaEncryptor(configuration);
        }

        public static IRsaEncryptor Create(RSAParameters parameters)
        {
            var configuration = new RsaEncryptorConfiguration
            {
                Keys = RsaKeyPair.Create(parameters)
            };

            return new RsaEncryptor(configuration);
        }

        public static IRsaEncryptor Create(IRsaEncryptorConfiguration configuration)
        {
            return new RsaEncryptor(configuration);
        }
    }
}
