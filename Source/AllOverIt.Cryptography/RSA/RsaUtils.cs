﻿using AllOverIt.Cryptography.Extensions;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.RSA
{
    public static class RsaUtils
    {
        // Valid key sizes are dependent on the cryptographic service provider (CSP) that is used by the RSACryptoServiceProvider instance.
        // Windows CSPs enable keys sizes of 384 to 16384 bits for Windows versions prior to Windows 8.1, and key sizes of 512 to 16384 bits
        // for Windows 8.1. For more information, see CryptGenKey function in the Windows documentation.
        // https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsacryptoserviceprovider

        /// <summary>Calculates the maximum input size, in bytes, the RSA algorithm can encrypt based on its key size, in bits,
        /// and the encryption padding mode used.</summary>
        /// <param name="keySize">The RSA key size, in bits.</param>
        /// <param name="padding">The RSA encryption padding mode.</param>
        /// <returns>The maximum input size, in bytes, the RSA algorithm can encrypt based on its key size, in bits,
        /// and the encryption padding mode used.</returns>
        public static int GetMaxInputLength(int keySize, RSAEncryptionPadding padding)
        {
            var keySizeBytes = keySize / 8;

            if (padding == RSAEncryptionPadding.Pkcs1)
            {
                /*
                   https://www.rfc-editor.org/rfc/rfc3447#section-7.2.1

                   RSAES-PKCS1-V1_5-ENCRYPT ((n, e), M)

                   Input:
                   (n, e)   recipient's RSA public key (k denotes the length in octets
                            of the modulus n)
                   M        message to be encrypted, an octet string of length mLen,
                            where mLen <= k - 11
                 */
                return keySizeBytes - 11;
            }

            // https://www.rfc-editor.org/rfc/rfc3447#section-7.1
            // RSAES-OAEP can operate on messages of length up to k -2hLen - 2 octets, where hLen is the length of the output
            // from the underlying hash function and k is the length in octets of the recipient's RSA modulus.

            var hashLength = padding.OaepHashAlgorithm.GetHashSize() / 8;

            return keySizeBytes - (2 * hashLength) - 2;
        }
    }
}
