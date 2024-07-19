using AllOverIt.Assertion;
using AllOverIt.Cryptography.RSA.Exceptions;
using AllOverIt.Extensions;
using System.Security.Cryptography;

using RSAAlgorithm = System.Security.Cryptography.RSA;

namespace AllOverIt.Cryptography.RSA
{
    /*
        Using an RSA key with a size of 3072 bits offers a level of security roughly equivalent to that of a 128-bit symmetric key.

        The determination of RSA key size being "roughly equivalent to a 128-bit symmetric key" is based on the estimation of the computational
        effort required to break each encryption scheme.The key size in symmetric encryption algorithms(e.g., AES) and asymmetric encryption
        algorithms(e.g., RSA) is measured in bits and directly impacts the strength of the encryption.

        In symmetric encryption, the security primarily relies on the secrecy of the encryption key, and the same key is used for both encryption
        and decryption.The key length directly affects the number of possible keys, and a longer key makes exhaustive search attacks more computationally
        intensive.For example, a 128-bit symmetric key has 2^128 possible combinations, making brute-force attacks infeasible with current technology.

        In asymmetric encryption (like RSA), the security is based on the mathematical relationship between the public and private keys.The key length
        here influences the size of the modulus used in RSA calculations, and it determines the number of possible keys and the difficulty of factoring
        the modulus.Longer RSA key sizes increase the difficulty of factoring, making it more secure against attacks like integer factorization.

        The approximate equivalence between RSA key size and symmetric key size is derived from the current understanding of the best known algorithms
        for factoring large numbers (used in breaking RSA) and the best known attacks against symmetric encryption algorithms.It is also based on the
        assumption that the effort required to break RSA is about the same as the effort required to perform a brute-force search on a symmetric key.
   */

    /// <summary>A container for an RSA public and private key.</summary>
    public sealed class RsaKeyPair
    {
        /// <summary>An RSA public key. This can be <see langword="null"/> if it is not required, such as when performing a decryption operation.</summary>
        public byte[]? PublicKey { get; private set; }

        /// <summary>An RSA private key. This can be <see langword="null"/> if it is not required, such as when performing an encryption operation.</summary>
        public byte[]? PrivateKey { get; private set; }

        /// <summary>Gets the size, in bits, of the key used by the RSA algorithm.</summary>
        public int KeySize { get; private set; }

        /// <summary>Constructor.</summary>
        /// <param name="keySizeInBits">The key size, in bits.</param>
        public RsaKeyPair(int keySizeInBits = 3072)
            : this(RSAAlgorithm.Create(keySizeInBits))
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="publicKey">The RSA public key to use. This can be <see langword="null"/> (or empty) if it is not required, such as when performing a
        /// decryption operation.</param>
        /// <param name="privateKey">>The RSA private key to use. This can be <see langword="null"/> (or empty) if it is not required, such as when performing
        /// an encryption operation.</param>
        /// <remarks>At least one of the public / private keys must be provided.</remarks>
        public RsaKeyPair(byte[]? publicKey, byte[]? privateKey)
        {
            SetKeys(publicKey, privateKey);
        }

        /// <summary>Constructor.</summary>
        /// <param name="publicKeyBase64">The RSA public key to use, in base64 format. This can be <see langword="null"/> (or empty) if it is not required,
        /// such as when performing a decryption operation.</param>
        /// <param name="privateKeyBase64">The RSA private key to use, in base64 format. This can be <see langword="null"/> (or empty) if it is not required,
        /// such as when performing an encryption operation.</param>
        /// <remarks>At least one of the public / private keys must be provided.</remarks>
        public RsaKeyPair(string publicKeyBase64, string privateKeyBase64)
        {
            SetKeys(publicKeyBase64, privateKeyBase64);
        }

        /// <summary>Constructor.</summary>
        /// <param name="rsa">An instance of the <see cref="RSAAlgorithm"/> algorithm.</param>      // TEST WHAT HAPPENS WHEN THERE IS NO PUBLIC/PRIVATE KEY
        public RsaKeyPair(RSAAlgorithm rsa)
        {
            _ = rsa.WhenNotNull(nameof(rsa));

            var publicKey = rsa.ExportRSAPublicKey();
            var privateKey = rsa.ExportRSAPrivateKey();

            SetKeys(publicKey, privateKey);
        }

        /// <summary>Constructor.</summary>
        /// <param name="parameters">The parameters for the <see cref="RSAAlgorithm"/> algorithm.</param>
        public RsaKeyPair(RSAParameters parameters)
            : this(RSAAlgorithm.Create(parameters))
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="xmlKeys">The XML string containing the <see cref="RSAAlgorithm"/> algorithm key information.</param>
        public RsaKeyPair(string xmlKeys)
            : this(CreateRsaFromXml(xmlKeys))
        {
        }

        /// <summary>Sets the public and private keys to be used by the RSA algorithm.</summary>
        /// <param name="publicKey">The RSA public key to use. This can be <see langword="null"/> (or empty) if it is not required, such as when performing a
        /// decryption operation.</param>
        /// <param name="privateKey">>The RSA private key to use. This can be <see langword="null"/> (or empty) if it is not required, such as when performing
        /// an encryption operation.</param>
        /// <remarks>At least one of the public / private keys must be provided.</remarks>
        public void SetKeys(byte[]? publicKey, byte[]? privateKey)
        {
            Throw<RsaException>.When(
                publicKey.IsNullOrEmpty() && privateKey.IsNullOrEmpty(),
                "At least one RSA key is required.");

            PublicKey = publicKey;
            PrivateKey = privateKey;
            KeySize = GetKeySize();
        }

        /// <summary>Sets the public and private keys to be used by the RSA algorithm.</summary>
        /// <param name="publicKeyBase64">The RSA public key to use, in base64 format. This can be <see langword="null"/> (or empty) if it is not required,
        /// such as when performing a decryption operation.</param>
        /// <param name="privateKeyBase64">The RSA private key to use, in base64 format. This can be <see langword="null"/> (or empty) if it is not required,
        /// such as when performing an encryption operation.</param>
        /// <remarks>At least one of the public / private keys must be provided.</remarks>
        public void SetKeys(string publicKeyBase64, string privateKeyBase64)
        {
            Throw<RsaException>.When(
                publicKeyBase64.IsNullOrEmpty() && privateKeyBase64.IsNullOrEmpty(),
                "At least one RSA key is required.");

            var publicKey = GetAsBytes(publicKeyBase64);
            var privateKey = GetAsBytes(privateKeyBase64);

            SetKeys(publicKey, privateKey);
        }

        private static byte[]? GetAsBytes(string key)
        {
            if (key.IsNullOrEmpty())
            {
                return null;
            }

            return Convert.FromBase64String(key);
        }

        private int GetKeySize()
        {
            using var rsa = RSAAlgorithm.Create();

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

        private static RSAAlgorithm CreateRsaFromXml(string xmlKeys)
        {
            var rsa = RSAAlgorithm.Create();

            rsa.FromXmlString(xmlKeys);

            return rsa;
        }
    }
}
