namespace AllOverIt.Cryptography.AES
{
    public interface IAesEncryptor : IEncryptor, IStreamEncryptor
    {
        IAesEncryptionConfiguration Configuration { get; }

#if !NETSTANDARD2_1
        int GetCipherTextLength(int plainTextLength);
#endif        
    }
}
