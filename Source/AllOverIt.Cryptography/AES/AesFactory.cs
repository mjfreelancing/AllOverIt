using AllOverIt.Assertion;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.AES
{
    public sealed class AesFactory : IAesFactory
    {
        public static readonly AesFactory Instance = new();

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
