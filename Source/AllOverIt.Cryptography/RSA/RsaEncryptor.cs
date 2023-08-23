using AllOverIt.Assertion;
using AllOverIt.Extensions;
using System.IO;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.RSA
{
    /// <summary>A cryptographic implementation providing RSA encryption and decryption operations.</summary>
    public sealed class RsaEncryptor : IRsaEncryptor
    {
        private readonly IRsaFactory _rsaFactory;
        
        private int? _maxInputLength;

        public IRsaEncryptorConfiguration Configuration { get; }

        public RsaEncryptor()
            : this(new RsaFactory(), new RsaEncryptorConfiguration())
        {
        }

        public RsaEncryptor(IRsaEncryptorConfiguration configuration)
            : this(new RsaFactory(), configuration)
        {
        }

        internal RsaEncryptor(IRsaFactory rsaFactory, IRsaEncryptorConfiguration configuration)
        {
            _rsaFactory = rsaFactory.WhenNotNull(nameof(rsaFactory));
            Configuration = configuration.WhenNotNull(nameof(configuration));
        }

        public int GetMaxInputLength()
        {
            // TODO: Throw if _rsaKeyPair.PublicKey is null

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

        public byte[] Encrypt(byte[] plainText)
        {
            _ = plainText.WhenNotNullOrEmpty(nameof(plainText));

            // TODO: Throw if _rsaKeyPair.PublicKey is null

            using (var rsa = _rsaFactory.Create())
            {
                rsa.ImportRSAPublicKey(Configuration.Keys.PublicKey, out _);

                return rsa.Encrypt(plainText, Configuration.Padding);
            }
        }

        public byte[] Decrypt(byte[] cipherText)
        {
            _ = cipherText.WhenNotNullOrEmpty(nameof(cipherText));

            using (var rsa = _rsaFactory.Create())
            {
                // TODO: Throw if _rsaKeyPair.PrivateKey is null

                rsa.ImportRSAPrivateKey(Configuration.Keys.PrivateKey, out _);

                return rsa.Decrypt(cipherText, Configuration.Padding);
            }
        }

        public void Encrypt(Stream plainTextStream, Stream cipherTextStream)
        {
            var plainTextBytes = plainTextStream.ToByteArray();

            var cipherTextBytes = Encrypt(plainTextBytes);

            cipherTextStream.FromByteArray(cipherTextBytes);
        }

        public void Decrypt(Stream cipherTextStream, Stream plainTextStream)
        {
            var cipherTextBytes = cipherTextStream.ToByteArray();

            var plainTextBytes = Decrypt(cipherTextBytes);

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
