using AllOverIt.Cryptography.RSA;

namespace AllOverIt.Cryptography.Hybrid
{
    public sealed class RsaAesHybridEncryptorConfiguration : IRsaAesHybridEncryptorConfiguration
    {
        public IRsaEncryptorConfiguration Encryption { get; init; } = new RsaEncryptorConfiguration();
        public IRsaSigningConfiguration Signing { get; init; } = new RsaSigningConfiguration();
    }
}
