namespace AllOverIt.Cryptography.AES
{
    /// <summary>Represents a factory that creates <see cref="IAesEncryptor"/> instances.</summary>
    public interface IAesEncryptorFactory
    {
        /// <summary>Creates an <see cref="IAesEncryptor"/> using a default configuration and new random secret key and initialization vector.</summary>
        /// <returns>A new <see cref="IAesEncryptor"/> instance.</returns>
        IAesEncryptor Create();

        /// <summary>Creates an <see cref="IAesEncryptor"/> using a default configuration and the provided secret key and initialization vector.</summary>
        /// <param name="key">The secret key.</param>
        /// <param name="iv">The initialization vector.</param>
        /// <returns>A new <see cref="IAesEncryptor"/> instance.</returns>
        IAesEncryptor Create(byte[] key, byte[] iv);
    }
}
