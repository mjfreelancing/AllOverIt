using AllOverIt.Cryptography.RSA;

namespace AllOverIt.Cryptography.Hybrid
{
    /// <summary>Provides configuration for performing RSA-AES hybrid encryption / decryption.</summary>
    public interface IRsaAesHybridEncryptorConfiguration
    {
        /// <summary>Provides encryption / decryption options. The sender (encryption) and the recipient
        /// (decryption) each have an RSA pair of keys.
        /// <para>
        /// When encrypting data, the <c>Encryption.Keys</c> property should be set as <c>new RsaKeyPair(recipientPublicKey, null)</c>.
        /// </para>
        /// <para>
        /// When decrypting data, the <c>Encryption.Keys</c> property should be set as <c>new RsaKeyPair(null, recipientPrivateKey)</c>.
        /// </para>
        /// </summary>
        IRsaEncryptorConfiguration Encryption { get; }

        /// <summary>Provides RSA hash signing options. The sender (encryption) and the recipient (decryption) each have an RSA pair of keys.
        /// <para>
        /// When encrypting data, the <c>Signing.Keys</c> property should be set as <c>new RsaKeyPair(null, senderPrivateKey)</c>.
        /// </para>
        /// <para>
        /// When decrypting data, the <c>Signing.Keys</c> property should be set as <c>new RsaKeyPair(senderPublicKey, null)</c>.
        /// </para>
        /// </summary>
        IRsaSigningConfiguration Signing { get; }
    }
}