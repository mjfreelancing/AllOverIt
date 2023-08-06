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
        public Aes Create(IAesEncryptionConfiguration configuration)
        {
            _ = configuration.WhenNotNull(nameof(configuration));

            var aes = Aes.Create();

            aes.Mode = configuration.Mode;
            aes.Padding = configuration.Padding;
            aes.KeySize = configuration.KeySize;          // The aes.Key will be updated if this is not the default
            aes.BlockSize = configuration.BlockSize;
            aes.FeedbackSize = configuration.FeedbackSize;
            aes.Key = configuration.Key;
            aes.IV = configuration.IV;

            return aes;
        }
    }
}
