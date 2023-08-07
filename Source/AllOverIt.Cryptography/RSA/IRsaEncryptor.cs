namespace AllOverIt.Cryptography.RSA
{
    /// <summary>Represents a cryptographic implementation providing RSA encryption and decryption operations.</summary>
    public interface IRsaEncryptor : IEncryptor, IStreamEncryptor
    {
        IRsaEncryptionConfiguration Configuration { get; }

        // Derived from the public key used for encryption
        int GetMaxInputLength();
    }
}
