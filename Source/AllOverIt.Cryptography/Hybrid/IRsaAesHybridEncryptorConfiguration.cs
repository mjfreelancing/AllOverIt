using AllOverIt.Cryptography.RSA;

namespace AllOverIt.Cryptography.Hybrid
{
    public interface IRsaAesHybridEncryptorConfiguration
    {
        // The public / private keys are used for encrypting / decrypting the AES session key, respectively
        IRsaEncryptorConfiguration Encryption { get; }
        IRsaSigningConfiguration Signing { get; }
    }
}