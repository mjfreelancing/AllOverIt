using AllOverIt.Assertion;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.RSA
{
    public sealed class RsaEncryptorConfiguration : IRsaEncryptorConfiguration
    {
        public RsaKeyPair Keys { get; init; } = RsaKeyPair.Create();

        public RSAEncryptionPadding Padding { get; init; } = RSAEncryptionPadding.OaepSHA256;

        public RsaEncryptorConfiguration()
        {
        }

        public RsaEncryptorConfiguration(RsaKeyPair keys)
        {
            Keys = keys.WhenNotNull(nameof(keys));
        }
    }
}
