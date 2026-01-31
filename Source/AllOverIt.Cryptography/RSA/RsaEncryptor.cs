using AllOverIt.Assertion;
using AllOverIt.Cryptography.RSA.Exceptions;
using AllOverIt.Extensions;
using System.Security.Cryptography;

using RSAAlgorithm = System.Security.Cryptography.RSA;

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
            _rsaFactory = rsaFactory.WhenNotNull();
            Configuration = configuration.WhenNotNull();
        }

        /// <inheritdoc />
        public int GetMaxInputLength()
        {
            Throw<RsaException>.WhenNull(Configuration.Keys.PublicKey, "An RSA public key has not been configured.");

            if (!_maxInputLength.HasValue)
            {
                using var rsa = _rsaFactory.Create();

                rsa.ImportRSAPublicKey(Configuration.Keys.PublicKey, out _);

                _maxInputLength = RsaUtils.GetMaxInputLength(rsa.KeySize, Configuration.Padding);
            }

            return _maxInputLength.Value;
        }

        /// <inheritdoc />
        public byte[] Encrypt(byte[] plainText)
        {
            _ = plainText.WhenNotNullOrEmpty();

            Throw<RsaException>.WhenNull(Configuration.Keys.PublicKey, "An RSA public key has not been configured.");

            using var rsa = _rsaFactory.Create();

            rsa.ImportRSAPublicKey(Configuration.Keys.PublicKey, out _);

            var maxInputLength = GetMaxInputLength();

            if (plainText.Length <= maxInputLength)
            {
                return rsa.Encrypt(plainText, Configuration.Padding);
            }
            else
            {
                using var output = new MemoryStream();

                for (int offset = 0; offset < plainText.Length; offset += maxInputLength)
                {
                    int chunkSize = Math.Min(maxInputLength, plainText.Length - offset);
                    var chunk = new byte[chunkSize];
                    Array.Copy(plainText, offset, chunk, 0, chunkSize);

                    var encryptedChunk = rsa.Encrypt(chunk, Configuration.Padding);
                    output.Write(encryptedChunk, 0, encryptedChunk.Length);
                }

                return output.ToArray();
            }
        }

        /// <inheritdoc />
        public byte[] Decrypt(byte[] cipherText)
        {
            _ = cipherText.WhenNotNullOrEmpty();

            Throw<RsaException>.WhenNull(Configuration.Keys.PrivateKey, "An RSA private key has not been configured.");

            using var rsa = _rsaFactory.Create();

            rsa.ImportRSAPrivateKey(Configuration.Keys.PrivateKey, out _);

            var blockSize = rsa.KeySize / 8;

            if (cipherText.Length <= blockSize)
            {
                return rsa.Decrypt(cipherText, Configuration.Padding);
            }
            else
            {
                using var output = new MemoryStream();

                for (int offset = 0; offset < cipherText.Length; offset += blockSize)
                {
                    int chunkSize = Math.Min(blockSize, cipherText.Length - offset);

                    if (chunkSize != blockSize)
                    {
                        throw new RsaException("Failed to decrypt. Data may be corrupted.");
                    }

                    var chunk = new byte[chunkSize];
                    Array.Copy(cipherText, offset, chunk, 0, chunkSize);

                    var decryptedChunk = rsa.Decrypt(chunk, Configuration.Padding);
                    output.Write(decryptedChunk, 0, decryptedChunk.Length);
                }

                return output.ToArray();
            }
        }

        /// <inheritdoc />
        public void Encrypt(Stream plainTextStream, Stream cipherTextStream)
        {
            _ = plainTextStream.WhenNotNull();
            _ = cipherTextStream.WhenNotNull();

            Throw<RsaException>.WhenNull(Configuration.Keys.PublicKey, "An RSA public key has not been configured.");

            using var rsa = _rsaFactory.Create();

            rsa.ImportRSAPublicKey(Configuration.Keys.PublicKey, out _);

            var maxInputLength = GetMaxInputLength();
            var buffer = new byte[maxInputLength];
            int bytesRead;

            // Not using stream.Read(buffer, 0, maxInputLength) because some streams (network, compressed, ZipArchiveEntry, etc.) may
            // not return the full requested amount in a single read. This could result in encrypting smaller chunks than necessary,
            // which is inefficient. We want to maximize chunk sizes up to MaxInputLength for optimal performance.
            while ((bytesRead = plainTextStream.ReadFullBlock(buffer, maxInputLength)) > 0)
            {
                var chunk = bytesRead == maxInputLength
                    ? buffer
                    : buffer[..bytesRead];

                var encryptedChunk = rsa.Encrypt(chunk, Configuration.Padding);
                cipherTextStream.Write(encryptedChunk, 0, encryptedChunk.Length);
            }
        }

        /// <inheritdoc />
        public void Decrypt(Stream cipherTextStream, Stream plainTextStream)
        {
            _ = cipherTextStream.WhenNotNull();
            _ = plainTextStream.WhenNotNull();

            Throw<RsaException>.WhenNull(Configuration.Keys.PrivateKey, "An RSA private key has not been configured.");

            using var rsa = _rsaFactory.Create();

            rsa.ImportRSAPrivateKey(Configuration.Keys.PrivateKey, out _);

            var blockSize = rsa.KeySize / 8;
            var buffer = new byte[blockSize];
            int bytesRead;

            // Using ReadFullBlock() is essential because:
            // - Encrypted data MUST be in exact blocks of BlockSize
            // - If we don't read a complete block, the data is corrupted/incomplete
            // - Some streams may not return the full requested amount in a single Read() call
            while ((bytesRead = cipherTextStream.ReadFullBlock(buffer, blockSize)) > 0)
            {
                if (bytesRead != blockSize)
                {
                    throw new RsaException("Failed to decrypt stream. Data may be corrupted.");
                }

                var decryptedChunk = rsa.Decrypt(buffer, Configuration.Padding);
                plainTextStream.Write(decryptedChunk, 0, decryptedChunk.Length);
            }
        }
    }
}
