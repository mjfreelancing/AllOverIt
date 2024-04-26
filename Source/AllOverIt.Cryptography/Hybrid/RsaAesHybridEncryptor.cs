using AllOverIt.Assertion;
using AllOverIt.Cryptography.AES;
using AllOverIt.Cryptography.Extensions;
using AllOverIt.Cryptography.Hybrid.Exceptions;
using AllOverIt.Cryptography.RSA;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.Hybrid
{
    /*
         Sender                                                                         Receiver
           |                                                                                |
           | 1. Generate hash of data                                                       |
           |------------------------------------------------------------------------------->|
           |                                                                                |
           | 2. Create RSA signature of hash using sender's private key                     |
           |------------------------------------------------------------------------------->|
           |                                                                                |
           | 3. Generate AES key                                                            |
           |------------------------------------------------------------------------------->|
           |                                                                                |
           | 4. Encrypt AES key with recipient's RSA public key                             |
           |------------------------------------------------------------------------------->|
           |                                                                                |
           | 5. Encrypt data with AES key                                                   |
           |------------------------------------------------------------------------------->|
           |                                                                                |
           | 6. Send encrypted AES key, encrypted data, hash, and RSA signature to receiver |
           |------------------------------------------------------------------------------->|
           |                                                                                |

  
         Sender                                                                         Receiver
           |                                                                                |
           | 7. Decrypt AES key using recipient's RSA private key                           |
           |<-------------------------------------------------------------------------------|
           |                                                                                |
           | 8. Decrypt data using AES key                                                  |
           |<-------------------------------------------------------------------------------|
           |                                                                                |
           | 9. Verify RSA signature of hash using sender's RSA public key                  |
           |<-------------------------------------------------------------------------------|
           |                                                                                |     
     */

    /// <inheritdoc cref="IRsaAesHybridEncryptor" />
    public sealed class RsaAesHybridEncryptor : IRsaAesHybridEncryptor
    {
        private readonly RsaFactory _rsaFactory;
        private readonly RsaEncryptor _rsaEncryptor;
        private readonly AesEncryptorFactory _aesEncryptorFactory;
        private readonly IRsaSigningConfiguration _signingConfiguration;

        /// <summary>Constructor.</summary>
        /// <param name="configuration">The configuration providing the required encryption and signing options.</param>
        public RsaAesHybridEncryptor(IRsaAesHybridEncryptorConfiguration configuration)
        {
            _ = configuration.WhenNotNull(nameof(configuration));

            // The current tests are functional and there's no current need to make these
            // injectable but a new constructor is easy enough to add if this is required.
            _rsaFactory = new RsaFactory();
            _rsaEncryptor = new RsaEncryptor(configuration.Encryption);
            _aesEncryptorFactory = new AesEncryptorFactory();
            _signingConfiguration = configuration.Signing;
        }

        /// <summary>
        /// The encryption process includes:
        /// 
        /// <para>1. Calculate a hash of the provided 'plain text' using the hash algorithm specified on the
        ///          <see cref="IRsaAesHybridEncryptorConfiguration.Signing"/> configuration provided to the constructor.</para>
        /// <para>2. Compute the RSA signature of the hash using the sender's private key from the <see cref="IRsaAesHybridEncryptorConfiguration.Signing"/>
        ///          configuration provided to the constructor.</para>
        /// <para>3. Create a random AES session key and initialization vector (IV).</para>
        /// <para>4. RSA encrypt the AES session key using the recipient's public key on the <see cref="IRsaAesHybridEncryptorConfiguration.Encryption"/>
        ///          configuration provided to the constructor.</para>
        /// <para>5. Encrypt the 'plain text' using the random AES session key and (IV).</para>
        /// 
        /// The resultant ciphertext includes:
        /// 
        /// <para>* The hash from step 1;</para>
        /// <para>* The RSA signature of the hash from step 2;</para>
        /// <para>* The RSA encrypted AES Key from steps 3 and 4;</para>
        /// <para>* The AES initialization vector from step 3;</para>
        /// <para>* The AES encrypted 'plain text' from step 5.</para>
        /// </summary>
        /// <param name="plainText">The byte array containing the 'plain text'.</param>
        /// <returns>The byte array populated with the resulting 'cipher text'.</returns>
        public byte[] Encrypt(byte[] plainText)
        {
            _ = plainText.WhenNotNull(nameof(plainText));

            using (var cipherTextStream = new MemoryStream())
            {
                using (var plainTextStream = new MemoryStream(plainText))
                {
                    Encrypt(plainTextStream, cipherTextStream);

                    return cipherTextStream.ToArray();
                }
            }
        }

        /// <summary>
        /// The decryption process reads the content written to the 'cipher text' and performs the following:
        /// 
        /// <para>* Decrypt the embedded AES session key using the recipient's private RSA key on the <see cref="IRsaAesHybridEncryptorConfiguration.Encryption"/>
        ///         configuration provided to the constructor.</para>
        /// <para>* Decrypt the previously AES encrypted 'plain text' data.</para>
        /// <para>* Calculates the hash of the determined 'plain text' data and compares it to the hash embedded in the 'cipher text'.</para>
        /// <para>* Verifies the signature using the sender's public RSA key from the <see cref="IRsaAesHybridEncryptorConfiguration.Signing"/>
        ///         configuration provided to the constructor.</para>
        /// </summary>
        /// <param name="cipherText">The byte array containing the 'cipher text'.</param>
        /// <returns>The byte array populated with the resulting 'plain text'.</returns>
        /// <exception cref="RsaAesHybridException">The hash or its signature are invalid.</exception>
        public byte[] Decrypt(byte[] cipherText)
        {
            _ = cipherText.WhenNotNull(nameof(cipherText));

            using (var cipherTextStream = new MemoryStream(cipherText))
            {
                using (var plainTextStream = new MemoryStream())
                {
                    Decrypt(cipherTextStream, plainTextStream);

                    return plainTextStream.ToArray();
                }
            }
        }

        /// <inheritdoc cref="Encrypt(byte[])"/>
        /// <remarks>The <paramref name="plainTextStream"/> must be random access and the entire stream will be processed.</remarks>
        public void Encrypt(Stream plainTextStream, Stream cipherTextStream)
        {
            _ = plainTextStream.WhenNotNull(nameof(plainTextStream));
            _ = cipherTextStream.WhenNotNull(nameof(cipherTextStream));

            // Calculate the hash for the plain text
            plainTextStream.Position = 0;
            var hash = CalculateHash(plainTextStream);

            // Sign the plain text hash (using the sender's private RSA key)
            var signature = SignHash(hash);

            // Prepare AES encryptor with a random session Key and IV
            var aesEncryptor = _aesEncryptorFactory.Create();

            // RSA encrypt the AES key (using the recipient's public RSA key)
            var rsaEncryptedAesKey = _rsaEncryptor.Encrypt(aesEncryptor.Configuration.Key);

            // Write the data to the stream
            cipherTextStream.Write(hash);
            cipherTextStream.Write(signature);
            cipherTextStream.Write(rsaEncryptedAesKey);
            cipherTextStream.Write(aesEncryptor.Configuration.IV);

            plainTextStream.Position = 0;
            aesEncryptor.Encrypt(plainTextStream, cipherTextStream);
        }

        /// <inheritdoc cref="Decrypt(byte[])" />
        public void Decrypt(Stream cipherTextStream, Stream plainTextStream)
        {
            _ = cipherTextStream.WhenNotNull(nameof(cipherTextStream));
            _ = plainTextStream.WhenNotNull(nameof(plainTextStream));

            // Read the expected hash of the plain text
            var expectedHash = ReadFromStream(cipherTextStream, _signingConfiguration.HashAlgorithmName.GetHashSize() / 8);

            // Read the signature
            var signature = ReadFromStream(cipherTextStream, _rsaEncryptor.Configuration.Keys.KeySize / 8);

            // Determine the AES key
            var rsaEncryptedAesKey = ReadFromStream(cipherTextStream, _rsaEncryptor.Configuration.Keys.KeySize / 8);
            var aesKey = _rsaEncryptor.Decrypt(rsaEncryptedAesKey);     // using the recipient's private key

            // Read the AES IV
            var iv = ReadFromStream(cipherTextStream, 16);

            // Read the cipher text
            var remaining = cipherTextStream.Length - cipherTextStream.Position;
            var encryptedPlainText = ReadFromStream(cipherTextStream, (int) remaining);

            // Decrypt the cipher text (in the stream)
            var aesEncryptor = _aesEncryptorFactory.Create(aesKey, iv);
            var plainText = aesEncryptor.Decrypt(encryptedPlainText);

            // Calculate the hash of the plain text
            var plainTextHash = CalculateHash(plainText);

            // Including the raw hash adds another level of security on top of the signed hash validated below
            var isValid = expectedHash.SequenceEqual(plainTextHash);

            Throw<RsaAesHybridException>.WhenNot(isValid, "Hash mismatch.");

            // Verify the signature (using the sender's public key)
            VerifyHashSignature(plainTextHash, signature);

            plainTextStream.Write(plainText);
        }

        private byte[] CalculateHash(byte[] plainText)
        {
#if NET9_0_OR_GREATER
            return CryptographicOperations.HashData(_signingConfiguration.HashAlgorithmName, plainText);
#else
            using (var hashAlgorithm = _signingConfiguration.HashAlgorithmName.CreateHashAlgorithm())
            {
                return hashAlgorithm.ComputeHash(plainText);
            }
#endif
        }

        private byte[] CalculateHash(Stream plainText)
        {
#if NET9_0_OR_GREATER
            return CryptographicOperations.HashData(_signingConfiguration.HashAlgorithmName, plainText);
#else
            using (var hashAlgorithm = _signingConfiguration.HashAlgorithmName.CreateHashAlgorithm())
            {
                return hashAlgorithm.ComputeHash(plainText);
            }
#endif
        }

        private byte[] SignHash(byte[] hash)
        {
            using (var rsa = _rsaFactory.Create())
            {
                var rsaPrivateKey = _signingConfiguration.Keys.PrivateKey;

                rsa.ImportRSAPrivateKey(rsaPrivateKey, out _);

                return rsa.SignHash(hash, _signingConfiguration.HashAlgorithmName, _signingConfiguration.Padding);
            }
        }

        private void VerifyHashSignature(byte[] plainTextHash, byte[] signature)
        {
            using (var rsa = _rsaFactory.Create())
            {
                rsa.ImportRSAPublicKey(_signingConfiguration.Keys.PublicKey, out _);

                var isValid = rsa.VerifyHash(plainTextHash, signature, _signingConfiguration.HashAlgorithmName, _signingConfiguration.Padding);

                Throw<RsaAesHybridException>.WhenNot(isValid, "The digital signature is invalid.");
            }
        }

        private static byte[] ReadFromStream(Stream stream, int length)
        {
            var data = new byte[length];
            stream.Read(data);

            return data;
        }
    }
}
