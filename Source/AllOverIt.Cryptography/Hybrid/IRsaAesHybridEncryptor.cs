namespace AllOverIt.Cryptography.Hybrid
{
    /// <summary>
    ///     Represents a cryptographic implementation providing an RSA-AES hybrid encryption scheme.
    /// <para>
    ///     RSA (Rivest-Shamir-Adleman) is an asymmetric encryption algorithm known for its ability to securely exchange encryption keys
    ///     and authenticate communication between two parties. It utilises a public key for encryption and a private key for decryption.
    /// </para>
    /// <para>
    ///     AES (Advanced Encryption Standard) is a symmetric encryption algorithm that excels at encrypting and decrypting large data quickly.
    ///     It utilises the same key for both encryption and decryption.
    /// </para>
    /// <para>
    ///     The RSA-AES hybrid encryption scheme combines the RSA and AES algorithms to take advantage of their individual strengths. An
    ///     additional layer of security providing data integrity and authenticity is added by including a hash of the 'plain text' data
    ///     and an RSA signature. Combined, the scheme works like so:
    /// </para>
    /// <para>
    /// </para>
    ///     1. Hash Generation: A hash value of the 'plain text' data is computed.
    /// <para>
    /// </para>
    ///     2. Signature Generation: The sender uses their private RSA key to create a digital signature of the hash.
    /// <para>
    ///     3. Key Exchange: The sender generates a random AES key, encrypts it using the recipient's RSA public key, and includes it
    ///        along with the associated AES initialization vector (IV) in the output.
    /// </para>
    /// <para>
    ///     4. Data Encryption: The actual 'plain text' data to be transmitted is encrypted using the random AES key / IV.
    /// </para>
    /// <para>
    ///     5. Data Transmission: The 'plain text' data hash, RSA signature, RSA-encrypted AES key, and the AES-encrypted 'plain text' data are sent to the recipient.
    /// </para>
    /// <para>
    ///     6. Decryption: The recipient uses their RSA private key to decrypt the encrypted AES key. This key is then used to decrypt the AES-encrypted
    ///        'plain text' data.
    /// </para>
    /// <para>
    ///     7. Data Integrity: A hash is calculated for the decrypted data and compared with the transmitted data hash.
    /// </para>
    /// <para>
    ///     8. Authentication: The sender's RSA public key is used to verify the signature.
    /// </para>
    /// </summary>
    public interface IRsaAesHybridEncryptor : IEncryptor, IStreamEncryptor
    {
    }
}
