using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.Extensions
{
    /// <summary>Contains hash algorithm related extensions.</summary>
    public static class HashAlgorithmNameExtensions
    {
        private static readonly IDictionary<HashAlgorithmName, Func<HashAlgorithm>> AlgorithmRegistry
            = new Dictionary<HashAlgorithmName, Func<HashAlgorithm>>
            {
                { HashAlgorithmName.MD5, () => MD5.Create() },
                { HashAlgorithmName.SHA1, () => SHA1.Create() },
                { HashAlgorithmName.SHA256, () => SHA256.Create() },
                { HashAlgorithmName.SHA384, () => SHA384.Create() },
                { HashAlgorithmName.SHA512, () => SHA512.Create() }
            };

        private static readonly IDictionary<HashAlgorithmName, int> HashRegistry
            = new Dictionary<HashAlgorithmName, int>
            {
                { HashAlgorithmName.MD5, 128 },
                { HashAlgorithmName.SHA1, 160 },
                { HashAlgorithmName.SHA256, 256 },
                { HashAlgorithmName.SHA384, 384 },
                { HashAlgorithmName.SHA512, 512 }
            };

        /// <summary>Gets the <see cref="HashAlgorithm"/> for a specified <see cref="HashAlgorithmName"/>.</summary>
        /// <param name="algorithmName">The name of the hash algorithm.</param>
        /// <returns>The <see cref="HashAlgorithm"/> for a specified <see cref="HashAlgorithmName"/>.</returns>
        public static HashAlgorithm CreateHashAlgorithm(this HashAlgorithmName algorithmName)
        {
            // Let it throw if the algorithm isn't registered
            return AlgorithmRegistry[algorithmName].Invoke();
        }

        /// <summary>Gets the hash size, in bits, of a specified <see cref="HashAlgorithmName"/>.</summary>
        /// <param name="algorithmName">The name of the hash algorithm.</param>
        /// <returns>The hash size, in bits, of a specified <see cref="HashAlgorithmName"/>.</returns>
        public static int GetHashSize(this HashAlgorithmName algorithmName)
        {
            // Let it throw if the algorithm isn't registered
            return HashRegistry[algorithmName];
        }
    }
}
