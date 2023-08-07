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

        #region Encrypt from stream to ...

        public static byte[] EncryptStreamToBytes(this IStreamEncryptor encrypter, Stream plainTextStream)
        {
            using (var cipherTextStream = new MemoryStream())
            {
                encrypter.Encrypt(plainTextStream, cipherTextStream);

                return cipherTextStream.ToArray();
            }
        }

        public static string EncryptStreamToBase64(this IStreamEncryptor encrypter, Stream plainTextStream)
        {
            var cipherTextBytes = EncryptStreamToBytes(encrypter, plainTextStream);

            return Convert.ToBase64String(cipherTextBytes);
        }

        #endregion

        #region Encrypt from bytes to ...

        public static string EncryptBytesToBase64(this IEncryptor encrypter, byte[] plainTextBytes)
        {
            var cipherTextBytes = encrypter.Encrypt(plainTextBytes);

            return Convert.ToBase64String(cipherTextBytes);
        }

        public static void EncryptBytesToStream(this IStreamEncryptor encrypter, byte[] plainTextBytes, Stream cipherTextStream)
        {
            using (var plainTextStream = new MemoryStream(plainTextBytes))
            {
                encrypter.Encrypt(plainTextStream, cipherTextStream);
            }
        }

        #endregion

        #region Encrypt from plain text to ...

        public static byte[] EncryptPlainTextToBytes(this IEncryptor encrypter, string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            return encrypter.Encrypt(plainTextBytes);
        }

        public static string EncryptPlainTextToBase64(this IEncryptor encrypter, string plainText)
        {
            var cipherTextBytes = EncryptPlainTextToBytes(encrypter, plainText);

            return Convert.ToBase64String(cipherTextBytes);
        }

        public static void EncryptPlainTextToStream(this IStreamEncryptor encrypter, string plainText, Stream cipherTextStream)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            EncryptBytesToStream(encrypter, plainTextBytes, cipherTextStream);
        }

        #endregion

        #region Encrypt from base64 to ...

        public static byte[] EncryptBase64ToBytes(this IEncryptor encrypter, string plainTextBase64)
        {
            var plainTextBytes = Convert.FromBase64String(plainTextBase64);

            return encrypter.Encrypt(plainTextBytes);
        }

        public static void EncryptBase64ToStream(this IStreamEncryptor encrypter, string plainTextBase64, Stream cipherTextStream)
        {
            var plainTextBytes = Convert.FromBase64String(plainTextBase64);

            DecryptBytesToStream(encrypter, plainTextBytes, cipherTextStream);
        }

        #endregion

        #region Decrypt from stream to ...

        public static byte[] DecryptStreamToBytes(this IStreamEncryptor encrypter, Stream cipherTextStream)
        {
            using (var plainTextStream = new MemoryStream())
            {
                encrypter.Decrypt(cipherTextStream, plainTextStream);

                return plainTextStream.ToArray();
            }
        }

        public static string DecryptStreamToBase64(this IStreamEncryptor encrypter, Stream cipherTextStream)
        {
            var plainTextBytes = DecryptStreamToBytes(encrypter, cipherTextStream);

            return Convert.ToBase64String(plainTextBytes);
        }

        public static string DecryptStreamToPlainText(this IStreamEncryptor encrypter, Stream cipherTextStream)
        {
            var plainTextBytes = DecryptStreamToBytes(encrypter, cipherTextStream);

            return Encoding.UTF8.GetString(plainTextBytes);
        }

        #endregion

        #region Decrypt from bytes to ...

        public static string DecryptBytesToBase64(this IEncryptor encrypter, byte[] cipherTextBytes)
        {
            var plainTextBytes = encrypter.Decrypt(cipherTextBytes);

            return Convert.ToBase64String(plainTextBytes);
        }

        public static string DecryptBytesToPlainText(this IEncryptor encrypter, byte[] cipherTextBytes)
        {
            var plainTextBytes = encrypter.Decrypt(cipherTextBytes);

            return Encoding.UTF8.GetString(plainTextBytes);
        }

        public static void DecryptBytesToStream(this IStreamEncryptor encrypter, byte[] cipherTextBytes, Stream plainTextStream)
        {
            using (var cipherTextStream = new MemoryStream(cipherTextBytes))
            {
                encrypter.Decrypt(cipherTextStream, plainTextStream);
            }
        }

        #endregion

        #region Decrypt from base64 to ...

        public static byte[] DecryptBase64ToBytes(this IEncryptor encrypter, string cipherTextBase64)
        {
            var cipherTextBytes = Convert.FromBase64String(cipherTextBase64);

            return encrypter.Decrypt(cipherTextBytes);
        }

        public static string DecryptBase64ToPlainText(this IEncryptor encrypter, string cipherTextBase64)
        {
            var cipherTextBytes = Convert.FromBase64String(cipherTextBase64);

            return DecryptBytesToPlainText(encrypter, cipherTextBytes);
        }

        public static void DecryptBase64ToStream(this IStreamEncryptor encrypter, string cipherTextBase64, Stream plainTextStream)
        {
            var cipherTextBytes = Convert.FromBase64String(cipherTextBase64);

            DecryptBytesToStream(encrypter, cipherTextBytes, plainTextStream);
        }

        #endregion
    }
}
