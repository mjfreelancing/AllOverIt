using AllOverIt.Cryptography.Extensions;
using AllOverIt.Cryptography.Hybrid;
using AllOverIt.Cryptography.RSA;
using AllOverIt.Logging;
using System;
using System.IO;
using System.Text;

namespace AesRsaHybridEncryptionDemo
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
            // Creates a new public/private key pair with 128-bit security
            var rsaKeyPair = RsaKeyPair.Create();

            // This shows all options with defaults:
            //
            // var configuration = new RsaAesHybridEncryptorConfiguration
            // {
            //     Encryption = new RsaEncryptionConfiguration
            //     {
            //         Keys = rsaKeyPair,
            //         Padding = RSAEncryptionPadding.OaepSHA256
            //     },

            //     Signing = new RsaSigningConfiguration
            //     {
            //         HashAlgorithmName = HashAlgorithmName.SHA256,
            //         Padding = RSASignaturePadding.Pkcs1
            //     }
            // };

            var configuration = new RsaAesHybridEncryptorConfiguration
            {
                Encryption = new RsaEncryptionConfiguration(rsaKeyPair)
            };

            // There's several extension methods that allow for encryption / decryption between bytes, plain text,
            // base64 (plain and cipher text), and streams. This demo only shows a couple of these in use.
            var encryptor = new RsaAesHybridEncryptor(configuration);

            var logger = new ColorConsoleLogger();

            logger.WriteLine(ConsoleColor.White, "RSA Public Key:");
            logger.WriteLine(ConsoleColor.Blue, Convert.ToBase64String(rsaKeyPair.PublicKey));
            logger.WriteLine();
            logger.WriteLine(ConsoleColor.White, "RSA Private Key:");
            logger.WriteLine(ConsoleColor.Blue, Convert.ToBase64String(rsaKeyPair.PrivateKey));
            logger.WriteLine();
            //logger.WriteLine(ConsoleColor.Blue, $"AES Key: {Convert.ToBase64String()}");



            // TODO: Add a decode method that can accept encrypted bytes, base64, or stream
            //       and extract the embedded hash, signature, AES key, AES IV




            logger.WriteLine(ConsoleColor.White, "The phrase to be processed is:");

            logger.WriteLine(ConsoleColor.Yellow, PlainText);

            // This extension method uses RsaAesHybridEncryptor.Encrypt(byte[], byte[]).
            var encryptedBase64 = encryptor.EncryptPlainTextToBase64(PlainText);

            logger.WriteLine();
            logger.WriteLine(ConsoleColor.White, "Encrypted using RSA-AES (random Key and IV):");
            logger.WriteLine(ConsoleColor.Green, encryptedBase64);

            // Demonstrating this base64 string can be copied to a stream and decrypted from that
            using (var cipherStream = new MemoryStream(Convert.FromBase64String(encryptedBase64)))
            {
                using (var plainTextStream = new MemoryStream())
                {
                    encryptor.Decrypt(cipherStream, plainTextStream);

                    var decryptedFromBase64 = plainTextStream.ToArray();
                    var decryptedText = Encoding.UTF8.GetString(decryptedFromBase64);

                    logger.WriteLine();
                    logger.WriteLine(ConsoleColor.White, "Decrypted:");
                    logger.WriteLine(ConsoleColor.Yellow, decryptedText);
                }
            }



                //var e1 = new RsaAesHybridEncryptor(configuration);
                //var hybridEncrypted1 = e1.EncryptPlainTextToBytes(plainText);
                //var hybridDecrypted1 = e1.DecryptBytesToPlainText(hybridEncrypted1);



                //var e2 = new RsaAesHybridEncryptor(configuration);
                //var ms1 = new MemoryStream(e2.EncryptPlainTextToBytes(plainText));
                //var ms2 = new MemoryStream();
                //e2.Decrypt(ms1, ms2);
                //ms2.Position = 0;
                //var hybridDecrypted2 = Encoding.UTF8.GetString(ms2.ToArray());    //e2.DecryptBytesToPlainText(ms.ToArray());



                //var b = e2.EncryptStreamToBytes(new MemoryStream(Encoding.UTF8.GetBytes(plainText)));       // encrypt plain text in a stream
                //var t1 = e2.DecryptBytesToPlainText(b);

                //var ms3 = new MemoryStream();
                //e2.DecryptBytesToStream(b, ms3);
                //var t2 = Encoding.UTF8.GetString(ms3.ToArray());




            logger.WriteLine();
            logger.WriteLine("All Over It.");

            Console.ReadKey();
        }
    }
}