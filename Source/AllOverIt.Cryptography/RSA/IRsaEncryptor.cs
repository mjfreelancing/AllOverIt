namespace AllOverIt.Cryptography.RSA
{
    /// <summary>A cryptographic implementation providing RSA encryption and decryption operations.</summary>
    public interface IRsaEncryptor : IEncryptor, IStreamEncryptor
    {
        /// <summary>Provides RSA encryption / decryption options.</summary>
        IRsaEncryptorConfiguration Configuration { get; }

        /// <summary>Only application to encryption, this returns the maximum 'plain text' input length based on the configured public key.</summary>
        /// <returns>The maximum 'plain text' encryption input length, based on the configured public key.</returns>
        int GetMaxInputLength();
    }
}
