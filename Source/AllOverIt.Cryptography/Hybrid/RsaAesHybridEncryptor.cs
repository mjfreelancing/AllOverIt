using AllOverIt.Assertion;
using AllOverIt.Cryptography.AES;
using AllOverIt.Cryptography.Extensions;
using AllOverIt.Cryptography.RSA;
using System;
using System.IO;
using System.Linq;

namespace AllOverIt.Cryptography.Hybrid
{
    /// <summary>A cryptographic implementation providing RSA-AES hybrid encryption and decryption operations.</summary>
    public sealed class RsaAesHybridEncryptor : IRsaAesHybridEncryptor
    {
        private readonly IRsaFactory _rsaFactory;
        private readonly IRsaEncryptor _rsaEncryptor;
        private readonly IAesEncryptorFactory _aesEncryptorFactory;


        private readonly IRsaSigningConfiguration _signingConfiguration;
        
        public RsaAesHybridEncryptor(IRsaAesHybridEncryptorConfiguration configuration)
            : this(
                  new RsaFactory(),
                  RsaEncryptor.Create(configuration.Encryption),
                  new AesEncryptorFactory(),
                  configuration.Signing)
        {
        }

        internal RsaAesHybridEncryptor(IRsaFactory rsaFactory, IRsaEncryptor rsaEncryptor,
            IAesEncryptorFactory aesEncryptorFactory, IRsaSigningConfiguration signingConfiguration)
        {
            _rsaFactory = rsaFactory.WhenNotNull(nameof(rsaFactory));
            _rsaEncryptor = rsaEncryptor.WhenNotNull(nameof(rsaEncryptor));
            _aesEncryptorFactory = aesEncryptorFactory.WhenNotNull(nameof(aesEncryptorFactory));
            _signingConfiguration = signingConfiguration.WhenNotNull(nameof(signingConfiguration));
        }

        public byte[] Encrypt(byte[] plainText)
        {
            // TODO: Throw if there's no RSA private key
            // TODO: Add an option to choose public/private or private/public keys for encryption/decryption

            using (var cipherTextStream = new MemoryStream())
            {
                using (var plainTextStream = new MemoryStream(plainText))
                {
                    Encrypt(plainTextStream, cipherTextStream);

                    return cipherTextStream.ToArray();
                }
            }
        }

        public byte[] Decrypt(byte[] cipherText)
        {
            using (var cipherTextStream = new MemoryStream(cipherText))
            {
                using (var plainTextStream = new MemoryStream())
                {
                    Decrypt(cipherTextStream, plainTextStream);

                    return plainTextStream.ToArray();
                }
            }
        }

        // The plainTextStream must be random access and the entire stream will be processed
        public void Encrypt(Stream plainTextStream, Stream cipherTextStream)
        {
            // Calculate the hash for the plain text
            plainTextStream.Position = 0;
            var hash = CalculateHash(plainTextStream);

            // Sign the plain text hash
            var signature = SignHash(hash);

            // Prepare AES encryptor with a random Key and IV
            var aesEncryptor = _aesEncryptorFactory.Create();

            // RSA encrypt the AES key
            var rsaEncryptedAesKey = _rsaEncryptor.Encrypt(aesEncryptor.Configuration.Key);

            // Write the data to the stream
            cipherTextStream.Write(signature);
            cipherTextStream.Write(hash);
            cipherTextStream.Write(aesEncryptor.Configuration.IV);
            cipherTextStream.Write(rsaEncryptedAesKey);

            plainTextStream.Position = 0;
            aesEncryptor.Encrypt(plainTextStream, cipherTextStream);
        }

        public void Decrypt(Stream cipherTextStream, Stream plainTextStream)
        {
            // Read the signature
            var signature = ReadFromStream(cipherTextStream, _rsaEncryptor.Configuration.Keys.KeySize / 8);

            // Read the expected hash of the plain text
            var expectedHash = ReadFromStream(cipherTextStream, _signingConfiguration.HashAlgorithmName.GetHashSize() / 8);

            // Read the AES IV
            var iv = ReadFromStream(cipherTextStream, 16);

            // Determine the AES key
            var rsaEncryptedAesKey = ReadFromStream(cipherTextStream, _rsaEncryptor.Configuration.Keys.KeySize / 8);
            var aesKey = _rsaEncryptor.Decrypt(rsaEncryptedAesKey);

            // Read the cipher text
            var remaining = cipherTextStream.Length - cipherTextStream.Position;
            var encryptedPlainText = ReadFromStream(cipherTextStream, (int) remaining);

            // Decrypt the cipher text (in the stream)
            var aesEncryptor = _aesEncryptorFactory.Create(aesKey, iv);
            var plainText = aesEncryptor.Decrypt(encryptedPlainText);

            // Calculate the hash of the plain text
            var plainTextHash = CalculateHash(plainText);

            // Including the raw hash adds another level of security on top of the signed hash validated below
            if (!expectedHash.SequenceEqual(plainTextHash))
            {
                // TODO: Custom exception
                throw new InvalidOperationException("Hash mismatch.");
            }

            // Verify the signature
            using (var rsa = _rsaFactory.Create())
            {
                rsa.ImportRSAPublicKey(_rsaEncryptor.Configuration.Keys.PublicKey, out _);

                var isValid = rsa.VerifyHash(plainTextHash, signature, _signingConfiguration.HashAlgorithmName, _signingConfiguration.Padding);

                // TODO: Custom exception
                Throw<InvalidOperationException>.WhenNot(isValid, "The digital signature is invalid.");
            }

            plainTextStream.Write(plainText);
        }

        private byte[] CalculateHash(byte[] plainText)
        {
            using (var hashAlgorithm = _signingConfiguration.HashAlgorithmName.CreateHashAlgorithm())
            {
                return hashAlgorithm.ComputeHash(plainText);
            }
        }

        private byte[] CalculateHash(Stream plainText)
        {
            using (var hashAlgorithm = _signingConfiguration.HashAlgorithmName.CreateHashAlgorithm())
            {
                return hashAlgorithm.ComputeHash(plainText);
            }
        }

        private byte[] SignHash(byte[] hash)
        {
            using (var rsa = _rsaFactory.Create())
            {
                var rsaPrivateKey = _rsaEncryptor.Configuration.Keys.PrivateKey;

                rsa.ImportRSAPrivateKey(rsaPrivateKey, out _);

                return rsa.SignHash(hash, _signingConfiguration.HashAlgorithmName, _signingConfiguration.Padding);
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
