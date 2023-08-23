using AllOverIt.Assertion;
using AllOverIt.Cryptography.RSA;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.Hybrid
{
    public sealed class RsaSigningConfiguration : IRsaSigningConfiguration
    {
        public RsaKeyPair Keys { get; init; } = RsaKeyPair.Create();
        public HashAlgorithmName HashAlgorithmName { get; init; } = HashAlgorithmName.SHA256;
        public RSASignaturePadding Padding { get; init; } = RSASignaturePadding.Pkcs1;

        public RsaSigningConfiguration()
        {
        }

        public RsaSigningConfiguration(RsaKeyPair keys)
        {
            Keys = keys.WhenNotNull(nameof(keys));
        }
    }
}
