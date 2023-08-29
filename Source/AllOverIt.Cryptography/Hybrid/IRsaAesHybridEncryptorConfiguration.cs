using AllOverIt.Cryptography.RSA;

namespace AllOverIt.Cryptography.Hybrid
{
    public interface IRsaAesHybridEncryptorConfiguration
    {
        // Encryption: Use recipient's public key to encrypt the AES session key
        // Decryption: Use recipient's private key to decrypt the AES session key
        IRsaEncryptorConfiguration Encryption { get; }

        // Encryption: Use sender's private key to create the hash signature
        // Decryption: Use sender's public key to verify the RSA hash signature
        IRsaSigningConfiguration Signing { get; }
    }
}