using System.Security.Cryptography;

using RSAAlgorithm = System.Security.Cryptography.RSA;

namespace AllOverIt.Cryptography.RSA
{
    /// <inheritdoc cref="IRsaFactory" />
    internal sealed class RsaFactory : IRsaFactory
    {
        /// <inheritdoc />
        public RSAAlgorithm Create()
        {
            return RSAAlgorithm.Create();
        }

        /// <inheritdoc />
        public RSAAlgorithm Create(int keySizeInBits)
        {
            return RSAAlgorithm.Create(keySizeInBits);
        }

        /// <inheritdoc />
        public RSAAlgorithm Create(RSAParameters parameters)
        {
            return RSAAlgorithm.Create(parameters);
        }
    }
}
