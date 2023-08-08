using System.Security.Cryptography;

namespace AllOverIt.Cryptography.AES
{
    /// <summary>Provides configuration options for AES encryption and decryption.</summary>
    public interface IAesEncryptionConfiguration
    {
        /// <summary>Specifies the block cipher mode to use. The default is <see cref="CipherMode.CBC"/>.</summary>
        CipherMode Mode { get; }

        /// <summary>Specifies the type of padding to apply when the message data block is shorter than the full number
        /// of bytes needed for a cryptographic operation. The default is <see cref="PaddingMode.PKCS7"/>.</summary>
        PaddingMode Padding { get; }

        /// <summary>The size, in bits, of the secret key used by the AES algorithm. Valid key sizes can be obtained from
        /// <see cref="AesUtils.GetLegalKeySizes"/>. The default is 256.</summary>
        int KeySize { get; }

        /// <summary>The block size, in bits, of the cryptographic operation. For AES, the only valid block size is 128 bits
        /// so this value cannot be changed.</summary>
        int BlockSize { get; }

        /// <summary>The feedback size, in bits, is associated with cipher block modes such as Cipher Feedback (CFB) and
        /// Output Feedback (OFB) (not supported by AES) where it determines how many bits of the output are fed back into
        /// the encryption process in each iteration. The current implementation allows for a feedback size of 8 or 128.
        /// The feedback size used offers a trade-off between security and efficiency. Larger feedback sizes can provide
        /// better security, but may introduce more overhead in terms of data processing and may require additional padding
        /// for inputs that are not a multiple of the feedback size. The default is 8.</summary>
        int FeedbackSize { get; }

        /// <summary>The secret key for the AES algorithm.</summary>
        byte[] Key { get; }

        /// <summary>The initialization vector for the AES algorithm.</summary>
        byte[] IV { get; }

        /// <summary>Replaces the current secret key with a new key based on the current <see cref="KeySize"/>.</summary>
        void RegenerateKey();

        /// <summary>Replaces the current initialization vector (IV) with a new 16-byte IV.</summary>
        void RegenerateIV();

        /// <summary>Replaces the current secret key, based on the current <see cref="KeySize"/>, and 16-byte initialization vector.</summary>
        void RegenerateKeyAndIV();
    }
}
