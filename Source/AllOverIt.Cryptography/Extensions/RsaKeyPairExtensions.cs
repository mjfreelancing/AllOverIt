using AllOverIt.Assertion;
using AllOverIt.Cryptography.RSA;

namespace AllOverIt.Cryptography.Extensions
{
    /// <summary>Provides extensions methods for <see cref="RsaKeyPair"/>.</summary>
    public static class RsaKeyPairExtensions
    {
        /// <summary>Converts the public key within a <see cref="RsaKeyPair"/> to a base64 encoded string.</summary>
        /// <param name="rsaKeyPair">The <see cref="RsaKeyPair"/> instance.</param>
        /// <returns>The public key within a <see cref="RsaKeyPair"/> to a base64 encoded string.</returns>
        public static string GetPublicKeyAsBase64(this RsaKeyPair rsaKeyPair)
        {
            _ = rsaKeyPair.WhenNotNull();

            Throw<ArgumentException>.WhenNull(rsaKeyPair.PublicKey, "The public key cannot be null.");

            return Convert.ToBase64String(rsaKeyPair.PublicKey);
        }

        /// <summary>Converts the private key within a <see cref="RsaKeyPair"/> to a base64 encoded string.</summary>
        /// <param name="rsaKeyPair">The <see cref="RsaKeyPair"/> instance.</param>
        /// <returns>The private key within a <see cref="RsaKeyPair"/> to a base64 encoded string.</returns>
        public static string GetPrivateKeyAsBase64(this RsaKeyPair rsaKeyPair)
        {
            _ = rsaKeyPair.WhenNotNull();

            Throw<ArgumentException>.WhenNull(rsaKeyPair.PrivateKey, "The private key cannot be null.");

            return Convert.ToBase64String(rsaKeyPair.PrivateKey);
        }
    }
}
