using AllOverIt.Cryptography.RSA;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.Hybrid
{
    /// <summary>Provides options used for RSA signature creation and verification operations.</summary>
    public interface IRsaSigningConfiguration
    {
        /// <summary>The sender (encryption) and the recipient (decryption) each have an RSA pair of keys.
        /// <para>
        /// When encrypting data, this property should be set as <c>new RsaKeyPair(null, senderPrivateKey)</c>.
        /// </para>
        /// <para>
        /// When decrypting data, this property should be set as <c>new RsaKeyPair(senderPublicKey, null)</c>.
        /// </para>
        /// </summary>
        RsaKeyPair Keys { get; }

        /// <summary>Specifies the name of the hash algorithm to use with RSA signature creation and verification operations.</summary>
        HashAlgorithmName HashAlgorithmName { get; }

        /// <summary>Specifies the padding mode to use with RSA signature creation and verification operations.</summary>
        RSASignaturePadding Padding { get; }
    }
}
