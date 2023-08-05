using System.Security.Cryptography;

namespace AllOverIt.Cryptography.AES
{
    public interface IAesFactory
    {
        Aes Create(IAesEncryptionConfiguration configuration);
    }
}
