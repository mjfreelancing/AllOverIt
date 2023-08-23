using AllOverIt.Cryptography.RSA;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.Hybrid
{
    public interface IRsaSigningConfiguration
    {
        // The public / private keys are used for verifying / signing the hash, respectively
        RsaKeyPair Keys { get; }
        HashAlgorithmName HashAlgorithmName { get; }
        RSASignaturePadding Padding { get; }
    }
}
