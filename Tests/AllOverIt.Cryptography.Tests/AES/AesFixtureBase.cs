using AllOverIt.Cryptography.AES;
using AllOverIt.Fixture;
using System.Security.Cryptography;

namespace AllOverIt.Cryptography.Tests.AES
{
    public class AesFixtureBase : FixtureBase
    {
        public CipherMode GetCipherMode()
        {
            return CreateExcluding(CipherMode.CTS, CipherMode.OFB);
        }

        public PaddingMode GetPaddingMode()
        {
            // None is no good since the input needs to be a multiple of the block size
            // Zeros is not good since this will be included in the decrypted output, causing the tests to fail
            return CreateExcluding(PaddingMode.None, PaddingMode.Zeros);
        }

        public AesEncryptionConfiguration CreateAesConfiguration()
        {
            var cipherMode = GetCipherMode();

            return new AesEncryptionConfiguration
            {
                Mode = cipherMode,

                Padding = GetPaddingMode(),

                FeedbackSize = cipherMode == CipherMode.CFB
                        ? Create<bool>() ? 8 : 128
                        : 8,

                KeySize = Create<bool>()
                    ? AesUtils.GetLegalKeySizes()[0].MinSize
                    : AesUtils.GetLegalKeySizes()[0].MaxSize
            };
        }
    }
}
