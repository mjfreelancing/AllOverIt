using AllOverIt.Assertion;
using AllOverIt.Cryptography.RSA;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.Hybrid
{
    /// <inheritdoc cref="IRsaSigningConfiguration" />
    public sealed class RsaSigningConfiguration : IRsaSigningConfiguration
    {
        /// <inheritdoc />
        public RsaKeyPair Keys { get; init; } = new RsaKeyPair();

        /// <inheritdoc />
        /// <remarks>The default is <see cref="HashAlgorithmName.SHA256"/>.</remarks>
        public HashAlgorithmName HashAlgorithmName { get; init; } = HashAlgorithmName.SHA256;

        /// <inheritdoc />
        /// <remarks>The default is <see cref="RSASignaturePadding.Pkcs1"/>.</remarks>
        public RSASignaturePadding Padding { get; init; } = RSASignaturePadding.Pkcs1;

        /// <summary>Constructor.</summary>
        public RsaSigningConfiguration()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="keys">The keys used for RSA signature creation and verification.</param>
        public RsaSigningConfiguration(RsaKeyPair keys)
        {
            Keys = keys.WhenNotNull();
        }
    }
}
