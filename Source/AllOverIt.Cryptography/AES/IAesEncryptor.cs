namespace AllOverIt.Cryptography.AES
{
    /// <summary>Represents a cryptographic implementation providing AES encryption and decryption operations.</summary>
    public interface IAesEncryptor : IEncryptor, IStreamEncryptor
    {
        /// <summary>The AES configuration.</summary>
        IAesEncryptorConfiguration Configuration { get; }

        /// <summary>Gets the cipher text length for the specified plain text length and the current <see cref="Configuration"/>.</summary>
        /// <param name="plainTextLength">The length of the plain text content.</param>
        /// <returns>The cipher text length for the specified plain text length and the current <see cref="Configuration"/>.</returns>
        int GetCipherTextLength(int plainTextLength);
    }
}
