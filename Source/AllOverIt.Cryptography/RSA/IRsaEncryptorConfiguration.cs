using System.Security.Cryptography;

namespace AllOverIt.Cryptography.RSA
{
    public interface IRsaEncryptorConfiguration
    {
        RsaKeyPair Keys { get; }
        RSAEncryptionPadding Padding { get; }
    }
}
