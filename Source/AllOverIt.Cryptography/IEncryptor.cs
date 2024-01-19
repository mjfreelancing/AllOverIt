namespace AllOverIt.Cryptography
{
    /// <summary>Represents the ability to encrypt and decrypt an array of bytes.</summary>
    public interface IEncryptor
    {
        /// <summary>Encrypts a byte array containing the 'plain text' to another byte array that will be populated with the 'cipher text'.</summary>
        /// <param name="plainText">The byte array containing the 'plain text'.</param>
        /// <returns>The byte array populated with the resulting 'cipher text'.</returns>
        byte[] Encrypt(byte[] plainText);

        /// <summary>Decrypts a byte array containing the 'cipher text' to another byte array that will be populated with the 'plain text'.</summary>
        /// <param name="cipherText">The byte array containing the 'cipher text'.</param>
        /// <returns>The byte array populated with the resulting 'plain text'.</returns>
        byte[] Decrypt(byte[] cipherText);
    }
}
