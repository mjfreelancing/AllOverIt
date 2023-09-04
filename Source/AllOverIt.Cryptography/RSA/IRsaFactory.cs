using System.Security.Cryptography;

using RSAAlgorithm = System.Security.Cryptography.RSA;

namespace AllOverIt.Cryptography.RSA
{
    /// <summary>Provides a factory for creating <see cref="RSAAlgorithm"/> instances.</summary>
    public interface IRsaFactory
    {
        /// <summary>Creates a default implementation of the <see cref="RSAAlgorithm"/> algorithm.</summary>
        /// <returns>A new instance of the default implementation of the <see cref="RSAAlgorithm"/> algorithm.</returns>
        RSAAlgorithm Create();

        /// <summary>Creates a new instance of the <see cref="RSAAlgorithm"/> algorithm with an ephemeral key with
        /// the specified key size, in bits.</summary>
        /// <param name="keySizeInBits">The RSA key size, in bits.</param>
        /// <returns>A new instance of the <see cref="RSAAlgorithm"/> algorithm with an ephemeral key with
        /// the specified key size, in bits.</returns>
        RSAAlgorithm Create(int keySizeInBits);

        /// <summary>Creates a new instance of the <see cref="RSAAlgorithm"/> algorithm with an ephemeral key with
        /// the specified parameters.</summary>
        /// <param name="parameters">The parameters for the <see cref="RSAAlgorithm"/> algorithm.</param>
        /// <returns>A new instance of the <see cref="RSAAlgorithm"/> algorithm with an ephemeral key with
        /// the specified parameters.</returns>
        RSAAlgorithm Create(RSAParameters parameters);
    }
}
