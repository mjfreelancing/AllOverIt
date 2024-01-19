using System.Security.Cryptography;

namespace AllOverIt.Cryptography.RSA
{
    /// <summary>Provides RSA encryption / decryption options.</summary>
    public interface IRsaEncryptorConfiguration
    {
        /// <summary>Contains the asymetric public and private RSA keys.</summary>
        RsaKeyPair Keys { get; }

        /// <summary>Specifies the padding mode to use with RSA encryption or decryption operations.</summary>
        RSAEncryptionPadding Padding { get; }
    }
}
