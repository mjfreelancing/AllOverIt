using AllOverIt.Assertion;
using AllOverIt.Extensions;
using System;
using System.Security.Cryptography;

using RSAAlgorithm = System.Security.Cryptography.RSA;

namespace AllOverIt.Cryptography.RSA
{
    // Using an RSA key with a size of 3072 bits offers a level of security roughly equivalent to that of a 128-bit symmetric key.
    //
    // The determination of RSA key size being "roughly equivalent to a 128-bit symmetric key" is based on the estimation of the computational
    // effort required to break each encryption scheme. The key size in symmetric encryption algorithms (e.g., AES) and asymmetric encryption
    // algorithms (e.g., RSA) is measured in bits and directly impacts the strength of the encryption.

    // In symmetric encryption, the security primarily relies on the secrecy of the encryption key, and the same key is used for both encryption
    // and decryption.The key length directly affects the number of possible keys, and a longer key makes exhaustive search attacks more computationally
    // intensive.For example, a 128-bit symmetric key has 2^128 possible combinations, making brute-force attacks infeasible with current technology.


    // In asymmetric encryption (like RSA), the security is based on the mathematical relationship between the public and private keys.The key length
    // here influences the size of the modulus used in RSA calculations, and it determines the number of possible keys and the difficulty of factoring
    // the modulus.Longer RSA key sizes increase the difficulty of factoring, making it more secure against attacks like integer factorization.

    // The approximate equivalence between RSA key size and symmetric key size is derived from the current understanding of the best known algorithms
    // for factoring large numbers (used in breaking RSA) and the best known attacks against symmetric encryption algorithms. It is also based on the
    // assumption that the effort required to break RSA is about the same as the effort required to perform a brute-force search on a symmetric key.

    public sealed class RsaKeyPair
    {
        public byte[] PublicKey { get; private set; }
        public byte[] PrivateKey { get; private set; }

        /// <summary>Gets the size, in bits, of the key modulus use by the RSA algorithm.</summary>
        public int KeySize { get; private set; }

        public RsaKeyPair(RSAAlgorithm rsa)
        {
            var publicKey = rsa.ExportRSAPublicKey();
            var privateKey = rsa.ExportRSAPrivateKey();

            SetKeys(publicKey, privateKey);
        }

        // one of these values can be null / empty
        public RsaKeyPair(string publicKeyBase64, string privateKeyBase64)
        {
            SetKeys(publicKeyBase64, privateKeyBase64);
        }

        // one of these values can be null / empty
        public RsaKeyPair(byte[] publicKey, byte[] privateKey)
        {
            SetKeys(publicKey, privateKey);
        }

        public void SetKeys(string publicKeyBase64, string privateKeyBase64)
        {
            // TODO: Custom exception
            Throw<InvalidOperationException>.When(
                publicKeyBase64.IsNullOrEmpty() && privateKeyBase64.IsNullOrEmpty(),
                "At least one RSA key is required.");

            var publicKey = GetAsBytes(publicKeyBase64);
            var privateKey = GetAsBytes(privateKeyBase64);

            SetKeys(publicKey, privateKey);
        }

        public void SetKeys(byte[] publicKeyBase64, byte[] privateKeyBase64)
        {
            // TODO: Custom exception
            Throw<InvalidOperationException>.When(
                publicKeyBase64.IsNullOrEmpty() && privateKeyBase64.IsNullOrEmpty(),
                "At least one RSA key is required.");

            PublicKey = publicKeyBase64;
            PrivateKey = privateKeyBase64;
            KeySize = GetKeySize();
        }

        public static RsaKeyPair Create(RSAAlgorithm rsa)
        {
            return new RsaKeyPair(rsa);
        }

        public static RsaKeyPair Create(int keySizeInBits = 3072)
        {
            using (var rsa = RSAAlgorithm.Create(keySizeInBits))
            {
                return new RsaKeyPair(rsa);
            }
        }

        public static RsaKeyPair Create(RSAParameters parameters)
        {
            using (var rsa = RSAAlgorithm.Create(parameters))
            {
                return new RsaKeyPair(rsa);
            }
        }

        public static RsaKeyPair Create(string xmlKeys)
        {
            using (var rsa = RSAAlgorithm.Create())
            {
                rsa.FromXmlString(xmlKeys);

                return new RsaKeyPair(rsa);
            }
        }

        private static byte[] GetAsBytes(string key)
        {
            if (key.IsNullOrEmpty())
            {
                return null;
            }

            return Convert.FromBase64String(key);
        }

        private int GetKeySize()
        {
            using (var rsa = RSAAlgorithm.Create())
            {
                if (PublicKey is not null)
                {
                    rsa.ImportRSAPublicKey(PublicKey, out _);
                }
                else
                {
                    rsa.ImportRSAPrivateKey(PrivateKey, out _);
                }

                return rsa.KeySize;
            }
        }
    }
}
