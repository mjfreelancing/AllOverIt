using AllOverIt.Cryptography.RSA;

namespace AllOverIt.Cryptography.Hybrid
{
    /// <inheritdoc cref="IRsaAesHybridEncryptorConfiguration" />
    public sealed class RsaAesHybridEncryptorConfiguration : IRsaAesHybridEncryptorConfiguration
    {
        /// <inheritdoc />
        public IRsaEncryptorConfiguration Encryption { get; init; } = new RsaEncryptorConfiguration();

        /// <inheritdoc />
        public IRsaSigningConfiguration Signing { get; init; } = new RsaSigningConfiguration();
    }
}
