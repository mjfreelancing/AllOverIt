using AllOverIt.Assertion;
using System;
using System.IO;
using System.Text;

namespace AllOverIt.Cryptography.Extensions
{
    /// <summary>The <see cref="IEncryptor"/> and <see cref="IStreamEncryptor"/> provides support for encrypting and decrypting
    /// between byte arrays and streams respectively. This class contains extension methods that simplify the encryption and decryption
    /// of higher level constructs, such as plain text (strings) and base64 encoded versions of plain text and cipher text.</summary>
    public static class EncryptorExtensions
    {
        // Encrypt                                          Decrypt
        // From             To                  Done        From            To                  Done
        // =========================================        =========================================
        // Bytes            Bytes               Yes         Bytes           Bytes               Yes
        // Stream           Stream              Yes         Stream          Stream              Yes
        //
        // Stream           Bytes               Yes         Stream          Bytes               Yes
        // Stream           Base64              Yes         Stream          Base64              Yes
        // Stream           CipherText          N/A         Stream          PlainText           Yes
        //
        // Bytes            Base64              Yes         Bytes           Base64              Yes
        // Bytes            CipherText          N/A         Bytes           PlainText           Yes
        // Bytes            Stream              Yes         Bytes           Stream              Yes
        //
        // PlainText        Bytes               Yes         CipherText      Bytes               N/A
        // PlainText        Base64              Yes         CipherText      Base64              N/A
        // PlainText        Stream              Yes         CipherText      Stream              N/A
        //
        // Base64           Bytes               Yes         Base64          Bytes               Yes
        // Base64           CipherText          N/A         Base64          PlainText           Yes
        // Base64           Stream              Yes         Base64          Stream              Yes

        // NOTE: The extensions do not assert empty strings / array - letting it fail just like invalid data would

        #region Encrypt from stream to ...

        // done
        public static byte[] EncryptStreamToBytes(this IStreamEncryptor encryptor, Stream plainTextStream)
        {
            _ = encryptor.WhenNotNull(nameof(encryptor));
            _ = plainTextStream.WhenNotNull(nameof(plainTextStream));

            using (var cipherTextStream = new MemoryStream())
            {
                encryptor.Encrypt(plainTextStream, cipherTextStream);

                return cipherTextStream.ToArray();
            }
        }

        // done
        public static string EncryptStreamToBase64(this IStreamEncryptor encryptor, Stream plainTextStream)
        {
            _ = encryptor.WhenNotNull(nameof(encryptor));
            _ = plainTextStream.WhenNotNull(nameof(plainTextStream));

            var cipherTextBytes = EncryptStreamToBytes(encryptor, plainTextStream);

            return Convert.ToBase64String(cipherTextBytes);
        }

        #endregion

        #region Encrypt from bytes to ...

        // done
        public static string EncryptBytesToBase64(this IEncryptor encryptor, byte[] plainTextBytes)
        {
            _ = encryptor.WhenNotNull(nameof(encryptor));
            _ = plainTextBytes.WhenNotNull(nameof(plainTextBytes));

            var cipherTextBytes = encryptor.Encrypt(plainTextBytes);

            return Convert.ToBase64String(cipherTextBytes);
        }

        // done
        public static void EncryptBytesToStream(this IStreamEncryptor encryptor, byte[] plainTextBytes, Stream cipherTextStream)
        {
            _ = encryptor.WhenNotNull(nameof(encryptor));
            _ = plainTextBytes.WhenNotNull(nameof(plainTextBytes));
            _ = cipherTextStream.WhenNotNull(nameof(cipherTextStream));

            using (var plainTextStream = new MemoryStream(plainTextBytes))
            {
                encryptor.Encrypt(plainTextStream, cipherTextStream);
            }
        }

        #endregion

        #region Encrypt from plain text to ...

        // done
        public static byte[] EncryptPlainTextToBytes(this IEncryptor encryptor, string plainText)
        {
            _ = encryptor.WhenNotNull(nameof(encryptor));
            _ = plainText.WhenNotNull(nameof(plainText));

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            return encryptor.Encrypt(plainTextBytes);
        }

        // done
        public static string EncryptPlainTextToBase64(this IEncryptor encryptor, string plainText)
        {
            _ = encryptor.WhenNotNull(nameof(encryptor));
            _ = plainText.WhenNotNull(nameof(plainText));

            var cipherTextBytes = EncryptPlainTextToBytes(encryptor, plainText);

            return Convert.ToBase64String(cipherTextBytes);
        }

        public static void EncryptPlainTextToStream(this IStreamEncryptor encryptor, string plainText, Stream cipherTextStream)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            EncryptBytesToStream(encryptor, plainTextBytes, cipherTextStream);
        }

        #endregion

        #region Encrypt from base64 to ...

        public static byte[] EncryptBase64ToBytes(this IEncryptor encryptor, string plainTextBase64)
        {
            var plainTextBytes = Convert.FromBase64String(plainTextBase64);

            return encryptor.Encrypt(plainTextBytes);
        }

        public static void EncryptBase64ToStream(this IStreamEncryptor encryptor, string plainTextBase64, Stream cipherTextStream)
        {
            var plainTextBytes = Convert.FromBase64String(plainTextBase64);

            DecryptBytesToStream(encryptor, plainTextBytes, cipherTextStream);
        }

        #endregion

        #region Decrypt from stream to ...

        // done
        public static byte[] DecryptStreamToBytes(this IStreamEncryptor encryptor, Stream cipherTextStream)
        {
            _ = encryptor.WhenNotNull(nameof(encryptor));
            _ = cipherTextStream.WhenNotNull(nameof(cipherTextStream));

            using (var plainTextStream = new MemoryStream())
            {
                encryptor.Decrypt(cipherTextStream, plainTextStream);

                return plainTextStream.ToArray();
            }
        }

        public static string DecryptStreamToBase64(this IStreamEncryptor encryptor, Stream cipherTextStream)
        {
            var plainTextBytes = DecryptStreamToBytes(encryptor, cipherTextStream);

            return Convert.ToBase64String(plainTextBytes);
        }

        public static string DecryptStreamToPlainText(this IStreamEncryptor encryptor, Stream cipherTextStream)
        {
            var plainTextBytes = DecryptStreamToBytes(encryptor, cipherTextStream);

            return Encoding.UTF8.GetString(plainTextBytes);
        }

        #endregion

        #region Decrypt from bytes to ...

        public static string DecryptBytesToBase64(this IEncryptor encryptor, byte[] cipherTextBytes)
        {
            var plainTextBytes = encryptor.Decrypt(cipherTextBytes);

            return Convert.ToBase64String(plainTextBytes);
        }

        // done
        public static string DecryptBytesToPlainText(this IEncryptor encryptor, byte[] cipherTextBytes)
        {
            _ = encryptor.WhenNotNull(nameof(encryptor));
            _ = cipherTextBytes.WhenNotNull(nameof(cipherTextBytes));

            var plainTextBytes = encryptor.Decrypt(cipherTextBytes);

            return Encoding.UTF8.GetString(plainTextBytes);
        }

        // done
        public static void DecryptBytesToStream(this IStreamEncryptor encryptor, byte[] cipherTextBytes, Stream plainTextStream)
        {
            _ = encryptor.WhenNotNull(nameof(encryptor));
            _ = cipherTextBytes.WhenNotNull(nameof(cipherTextBytes));
            _ = plainTextStream.WhenNotNull(nameof(plainTextStream));

            using (var cipherTextStream = new MemoryStream(cipherTextBytes))
            {
                encryptor.Decrypt(cipherTextStream, plainTextStream);
            }
        }

        #endregion

        #region Decrypt from base64 to ...

        // done
        public static byte[] DecryptBase64ToBytes(this IEncryptor encryptor, string cipherTextBase64)
        {
            _ = encryptor.WhenNotNull(nameof(encryptor));
            _ = cipherTextBase64.WhenNotNull(nameof(cipherTextBase64));

            var cipherTextBytes = Convert.FromBase64String(cipherTextBase64);

            return encryptor.Decrypt(cipherTextBytes);
        }

        // done
        public static string DecryptBase64ToPlainText(this IEncryptor encryptor, string cipherTextBase64)
        {
            _ = encryptor.WhenNotNull(nameof(encryptor));
            _ = cipherTextBase64.WhenNotNull(nameof(cipherTextBase64));

            var cipherTextBytes = Convert.FromBase64String(cipherTextBase64);

            return DecryptBytesToPlainText(encryptor, cipherTextBytes);
        }

        // done
        public static void DecryptBase64ToStream(this IStreamEncryptor encryptor, string cipherTextBase64, Stream plainTextStream)
        {
            _ = encryptor.WhenNotNull(nameof(encryptor));
            _ = cipherTextBase64.WhenNotNull(nameof(cipherTextBase64));
            _ = plainTextStream.WhenNotNull(nameof(plainTextStream));

            var cipherTextBytes = Convert.FromBase64String(cipherTextBase64);

            DecryptBytesToStream(encryptor, cipherTextBytes, plainTextStream);
        }

        #endregion
    }
}
