using AllOverIt.Assertion;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.AES
{
    /// <summary>Implements a factory that creates <see cref="Aes"/> instances.</summary>
    public sealed class AesFactory : IAesFactory
    {
        /// <summary>A static instance of the <see cref="AesFactory"/>.</summary>
        public static readonly AesFactory Instance = new();

        /// <inheritdoc />
        public Aes Create(IAesEncryptorConfiguration configuration = default)
        {
            configuration ??= new AesEncryptorConfiguration();

            var aes = Aes.Create();

            aes.Mode = configuration.Mode;                  // For reference, CTS and OFB are not valid for AES
            aes.Padding = configuration.Padding;
            aes.KeySize = configuration.KeySize;
            aes.BlockSize = configuration.BlockSize;
            aes.FeedbackSize = configuration.FeedbackSize;
            aes.Key = configuration.Key;
            aes.IV = configuration.IV;

            return aes;
        }
    }
}
