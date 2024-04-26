using AllOverIt.Cryptography.Extensions;
using AllOverIt.Cryptography.Hybrid;
using AllOverIt.Cryptography.RSA;
using AllOverIt.Logging;
using System.Security.Cryptography;
using System.Text;

namespace RsaAesHybridEncryptionDemo
{
    internal class Program
    {
        private const string PlainText = """
            Using an RSA key with a size of 3072 bits offers a level of security roughly equivalent
            to that of a 128-bit symmetric key.
            
            The determination of RSA key size being "roughly equivalent to a 128-bit symmetric key"
            is based on the estimation of the computationaleffort required to break each encryption
            scheme. The key size in symmetric encryption algorithms (e.g., AES) and asymmetric encryption
            algorithms (e.g., RSA) is measured in bits and directly impacts the strength of the encryption.
            
            In symmetric encryption, the security primarily relies on the secrecy of the encryption key, and
            the same key is used for both encryption and decryption.The key length directly affects the number
            of possible keys, and a longer key makes exhaustive search attacks more computationally intensive.
            For example, a 128-bit symmetric key has 2^128 possible combinations, making brute-force attacks
            infeasible with current technology.

            In asymmetric encryption (like RSA), the security is based on the mathematical relationship between
            the public and private keys.The key length here influences the size of the modulus used in RSA
            calculations, and it determines the number of possible keys and the difficulty of factoring the
            modulus. Longer RSA key sizes increase the difficulty of factoring, making it more secure against
            attacks like integer factorization.

            The approximate equivalence between RSA key size and symmetric key size is derived from the current
            understanding of the best known algorithms for factoring large numbers (used in breaking RSA) and
            the best known attacks against symmetric encryption algorithms. It is also based on the assumption
            that the effort required to break RSA is about the same as the effort required to perform a brute-force
            search on a symmetric key.
            """;

        private static void Main()
        {
            // There's several extension methods that allow for encryption / decryption between bytes, plain text,
            // base64 (plain and cipher text), and streams. This demo only shows a couple of these in use.

            var logger = new ColorConsoleLogger();

            var senderRsaKeys = new RsaKeyPair();
            var recipientRsaKeys = new RsaKeyPair();

            var encryptedBase64 = Encrypt(PlainText, recipientRsaKeys.PublicKey, senderRsaKeys.PrivateKey, logger);

            Decrypt(encryptedBase64, senderRsaKeys.PublicKey, recipientRsaKeys.PrivateKey, logger);

            logger.WriteLine();
            logger.WriteLine("All Over It.");

            Console.ReadKey();
        }

        private static string Encrypt(string plainText, byte[] recipientPublicKey, byte[] senderPrivateKey, ColorConsoleLogger logger)
        {
            logger.WriteLine(ConsoleColor.White, "The phrase to be processed is:");
            logger.WriteLine(ConsoleColor.Yellow, plainText);
            logger.WriteLine();

            var encryptorConfiguration = new RsaAesHybridEncryptorConfiguration
            {
                Encryption = new RsaEncryptorConfiguration
                {
                    Keys = new RsaKeyPair(recipientPublicKey, null),    // PublicKey is used to encrypt the AES key
                    Padding = RSAEncryptionPadding.OaepSHA256
                },

                Signing = new RsaSigningConfiguration
                {
                    Keys = new RsaKeyPair(null, senderPrivateKey),      // PrivateKey is used to RSA sign the hash
                    HashAlgorithmName = HashAlgorithmName.SHA256,
                    Padding = RSASignaturePadding.Pkcs1
                }
            };

            var encryptor = new RsaAesHybridEncryptor(encryptorConfiguration);

            // This extension method uses RsaAesHybridEncryptor.Encrypt(byte[], byte[]).
            var encryptedBase64 = encryptor.EncryptPlainTextToBase64(plainText);

            logger.WriteLine(ConsoleColor.White, "Encrypted using RSA-AES (random Key and IV):");
            logger.WriteLine(ConsoleColor.Green, encryptedBase64);

            return encryptedBase64;
        }

        private static void Decrypt(string encryptedBase64, byte[] senderPublicKey, byte[] recipientPrivateKey, ColorConsoleLogger logger)
        {
            var decryptorConfiguration = new RsaAesHybridEncryptorConfiguration
            {
                Encryption = new RsaEncryptorConfiguration
                {
                    Keys = new RsaKeyPair(null, recipientPrivateKey),   // PrivateKey is used to decrypt the AES key

                    Padding = RSAEncryptionPadding.OaepSHA256
                },

                Signing = new RsaSigningConfiguration
                {
                    Keys = new RsaKeyPair(senderPublicKey, null),        // PublicKey is used to verify the RSA signature
                    HashAlgorithmName = HashAlgorithmName.SHA256,
                    Padding = RSASignaturePadding.Pkcs1
                }
            };

            var decryptor = new RsaAesHybridEncryptor(decryptorConfiguration);


            // Demonstrating this base64 string can be copied to a stream and decrypted from that
            using (var cipherStream = new MemoryStream(Convert.FromBase64String(encryptedBase64)))
            {
                using (var plainTextStream = new MemoryStream())
                {
                    decryptor.Decrypt(cipherStream, plainTextStream);

                    var decryptedFromBase64 = plainTextStream.ToArray();
                    var decryptedText = Encoding.UTF8.GetString(decryptedFromBase64);

                    logger.WriteLine();
                    logger.WriteLine(ConsoleColor.White, "Decrypted:");
                    logger.WriteLine(ConsoleColor.Yellow, decryptedText);
                }
            }
        }
    }
}