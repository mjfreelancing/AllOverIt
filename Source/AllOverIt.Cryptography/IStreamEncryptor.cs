using System.IO;

namespace AllOverIt.Cryptography
{
    /// <summary>Represents the ability to encrypt and decrypt from one stream to another.</summary>
    public interface IStreamEncryptor
    {
        /// <summary>Encrypts a stream containing the 'plain text' to another stream that will be populated with the 'cipher text'.</summary>
        /// <param name="plainTextStream">The stream containing the 'plain text'.</param>
        /// <param name="cipherTextStream">The stream to be populated with the resulting 'cipher text'.</param>
        void Encrypt(Stream plainTextStream, Stream cipherTextStream);

        /// <summary>Decrypts a stream containing the 'cipher text' to another stream that will be populated with the 'plain text'.</summary>
        /// <param name="plainTextStream">The stream containing the 'cipher text'.</param>
        /// <param name="cipherTextStream">The stream to be populated with the resulting 'plain text'.</param>
        void Decrypt(Stream cipherTextStream, Stream plainTextStream);
    }
}
