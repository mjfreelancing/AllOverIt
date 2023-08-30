using AllOverIt.Assertion;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.RSA
{
    /// <inheritdoc cref="IRsaEncryptorConfiguration" />
    public sealed class RsaEncryptorConfiguration : IRsaEncryptorConfiguration
    {
        /// <inheritdoc />
        public RsaKeyPair Keys { get; init; } = RsaKeyPair.Create();

        /// <inheritdoc />
        /// <remarks>The default padding mode is <see cref="RSAEncryptionPadding.OaepSHA256"/>.</remarks>
        public RSAEncryptionPadding Padding { get; init; } = RSAEncryptionPadding.OaepSHA256;

        /// <summary>Constructor.</summary>
        public RsaEncryptorConfiguration()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="keys">The RSA public / private key pair to use with encryption or decryption operations.</param>
        public RsaEncryptorConfiguration(RsaKeyPair keys)
        {
            Keys = keys.WhenNotNull(nameof(keys));
        }
    }
}
