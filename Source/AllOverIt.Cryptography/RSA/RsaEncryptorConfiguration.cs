using AllOverIt.Assertion;
using System.Security.Cryptography;

using RSAAlgorithm = System.Security.Cryptography.RSA;

namespace AllOverIt.Cryptography.RSA
{
    /// <inheritdoc cref="IRsaEncryptorConfiguration" />
    public sealed class RsaEncryptorConfiguration : IRsaEncryptorConfiguration
    {
        /// <inheritdoc />
        public RsaKeyPair Keys { get; init; } = new RsaKeyPair();

        /// <inheritdoc />
        /// <remarks>The default padding mode is <see cref="RSAEncryptionPadding.OaepSHA256"/>.</remarks>
        public RSAEncryptionPadding Padding { get; init; } = RSAEncryptionPadding.OaepSHA256;

        /// <summary>Constructor.</summary>
        public RsaEncryptorConfiguration()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="publicKey">The RSA public key to use. This can be <see langword="null"/> (or empty) if it is not required, such as when performing a
        /// decryption operation.</param>
        /// <param name="privateKey">>The RSA private key to use. This can be <see langword="null"/> (or empty) if it is not required, such as when performing
        /// an encryption operation.</param>
        /// <remarks>At least one of the public / private keys must be provided.</remarks>
        public RsaEncryptorConfiguration(byte[] publicKey, byte[] privateKey)
        {
            _ = publicKey.WhenNotNull(nameof(publicKey));
            _ = privateKey.WhenNotNull(nameof(privateKey));

            Keys = new RsaKeyPair(publicKey, privateKey);
        }

        /// <summary>Constructor.</summary>
        /// <param name="publicKeyBase64">The RSA public key to use, in base64 format. This can be <see langword="null"/> (or empty) if it is not required,
        /// such as when performing a decryption operation.</param>
        /// <param name="privateKeyBase64">The RSA private key to use, in base64 format. This can be <see langword="null"/> (or empty) if it is not required,
        /// such as when performing an encryption operation.</param>
        /// <remarks>At least one of the public / private keys must be provided.</remarks>
        public RsaEncryptorConfiguration(string publicKeyBase64, string privateKeyBase64)
        {
            _ = publicKeyBase64.WhenNotNull(nameof(publicKeyBase64));
            _ = privateKeyBase64.WhenNotNull(nameof(privateKeyBase64));

            Keys = new RsaKeyPair(publicKeyBase64, privateKeyBase64);
        }

        /// <summary>Constructor.</summary>
        /// <param name="parameters">The parameters for the <see cref="RSAAlgorithm"/> algorithm.</param>
        public RsaEncryptorConfiguration(RSAParameters parameters)
        {
            Keys = new RsaKeyPair(parameters);
        }

        /// <summary>Constructor.</summary>
        /// <param name="rsaKeyPair">The RSA public / private key pair to use with encryption or decryption operations.</param>
        public RsaEncryptorConfiguration(RsaKeyPair rsaKeyPair)
        {
            Keys = rsaKeyPair.WhenNotNull(nameof(rsaKeyPair));
        }
    }
}
