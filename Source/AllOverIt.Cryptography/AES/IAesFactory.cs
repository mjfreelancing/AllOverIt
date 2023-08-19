using System.Security.Cryptography;

namespace AllOverIt.Cryptography.AES
{
    /// <summary>Represents a factory that creates <see cref="Aes"/> instances.</summary>
    public interface IAesFactory
    {
        /// <summary>Creates a new <see cref="Aes"/> instance using the provided configuration.</summary>
        /// <param name="configuration">The AES configuration. If null, a default instance of <see cref="AesEncryptionConfiguration"/> will be used.</param>
        /// <returns>A new <see cref="Aes"/> instance using the provided configuration.</returns>
        Aes Create(IAesEncryptionConfiguration configuration = default);
    }
}
