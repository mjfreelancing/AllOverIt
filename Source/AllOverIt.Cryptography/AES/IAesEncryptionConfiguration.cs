using System.Security.Cryptography;

namespace AllOverIt.Cryptography.AES
{
    public interface IAesEncryptionConfiguration
    {
        CipherMode Mode { get; }
        PaddingMode Padding { get; }
        int KeySize { get; }                // Bits
        int BlockSize { get; }
        int FeedbackSize { get; }
        byte[] Key { get; }
        byte[] IV { get; }

        void RegenerateKey();             // Based on KeySize
        void RegenerateIV();
        void RegenerateKeyAndIV();
    }
}
