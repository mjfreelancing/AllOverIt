using AllOverIt.Assertion;
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

        /// <summary>Encrypts a source stream containing the 'plain text' data to a byte array.</summary>
        /// <param name="encryptor">The encryptor instance.</param>
        /// <param name="plainTextStream">The source stream containing the 'plain text' data to encrypt.</param>
        /// <returns>A byte array containing the 'cipher text' content.</returns>
        public static byte[] EncryptStreamToBytes(this IStreamEncryptor encryptor, Stream plainTextStream)
        {
            _ = encryptor.WhenNotNull();
            _ = plainTextStream.WhenNotNull();

            using var cipherTextStream = new MemoryStream();

            encryptor.Encrypt(plainTextStream, cipherTextStream);

            return cipherTextStream.ToArray();

        }

        /// <summary>Encrypts a source stream containing the 'plain text' data to a base64 encoded string.</summary>
        /// <param name="encryptor">The encryptor instance.</param>
        /// <param name="plainTextStream">The source stream containing the 'plain text' data to encrypt.</param>
        /// <returns>A base64 encoded string containing the 'cipher text' content.</returns>
        public static string EncryptStreamToBase64(this IStreamEncryptor encryptor, Stream plainTextStream)
        {
            _ = encryptor.WhenNotNull();
            _ = plainTextStream.WhenNotNull();

            var cipherTextBytes = EncryptStreamToBytes(encryptor, plainTextStream);

            return Convert.ToBase64String(cipherTextBytes);
        }

        #endregion

        #region Encrypt from bytes to ...

        /// <summary>Encrypts a byte array containing the 'plain text' data to a base64 encoded string.</summary>
        /// <param name="encryptor">The encryptor instance.</param>
        /// <param name="plainTextBytes">The byte array containing the 'plain text' data to encrypt.</param>
        /// <returns>A base64 encoded string containing the 'cipher text' content.</returns>
        public static string EncryptBytesToBase64(this IEncryptor encryptor, byte[] plainTextBytes)
        {
            _ = encryptor.WhenNotNull();
            _ = plainTextBytes.WhenNotNull();

            var cipherTextBytes = encryptor.Encrypt(plainTextBytes);

            return Convert.ToBase64String(cipherTextBytes);
        }

        /// <summary>Encrypts a byte array containing the 'plain text' data to a destination stream.</summary>
        /// <param name="encryptor">The encryptor instance.</param>
        /// <param name="plainTextBytes">The byte array containing the 'plain text' data to encrypt.</param>
        /// <param name="cipherTextStream">The destination stream containing the 'cipher text' content.</param>
        public static void EncryptBytesToStream(this IStreamEncryptor encryptor, byte[] plainTextBytes, Stream cipherTextStream)
        {
            _ = encryptor.WhenNotNull();
            _ = plainTextBytes.WhenNotNull();
            _ = cipherTextStream.WhenNotNull();

            using var plainTextStream = new MemoryStream(plainTextBytes);

            encryptor.Encrypt(plainTextStream, cipherTextStream);
        }

        #endregion

        #region Encrypt from plain text to ...

        /// <summary>Encrypts a string containing the 'plain text' data to a byte array.</summary>
        /// <param name="encryptor">The encryptor instance.</param>
        /// <param name="plainText">The source string containing the 'plain text' data to encrypt.</param>
        /// <returns>A byte array containing the 'cipher text' content.</returns>
        public static byte[] EncryptPlainTextToBytes(this IEncryptor encryptor, string plainText)
        {
            _ = encryptor.WhenNotNull();
            _ = plainText.WhenNotNull();

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            return encryptor.Encrypt(plainTextBytes);
        }

        /// <summary>Encrypts a string containing the 'plain text' data to a base64 encoded string.</summary>
        /// <param name="encryptor">The encryptor instance.</param>
        /// <param name="plainText">The string containing the 'plain text' data to encrypt.</param>
        /// <returns>A base64 encoded string containing the 'cipher text' content.</returns>
        public static string EncryptPlainTextToBase64(this IEncryptor encryptor, string plainText)
        {
            _ = encryptor.WhenNotNull();
            _ = plainText.WhenNotNull();

            var cipherTextBytes = EncryptPlainTextToBytes(encryptor, plainText);

            return Convert.ToBase64String(cipherTextBytes);
        }

        /// <summary>Encrypts a string containing the 'plain text' data to a destination stream.</summary>
        /// <param name="encryptor">The encryptor instance.</param>
        /// <param name="plainText">The string containing the 'plain text' data to encrypt.</param>
        /// <param name="cipherTextStream">The destination stream containing the 'cipher text' content.</param>
        public static void EncryptPlainTextToStream(this IStreamEncryptor encryptor, string plainText, Stream cipherTextStream)
        {
            _ = encryptor.WhenNotNull();
            _ = plainText.WhenNotNull();
            _ = cipherTextStream.WhenNotNull();

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            EncryptBytesToStream(encryptor, plainTextBytes, cipherTextStream);
        }

        #endregion

        #region Encrypt from base64 to ...

        /// <summary>Encrypts a base64 encoded string containing the 'plain text' data to a byte array.</summary>
        /// <param name="encryptor">The encryptor instance.</param>
        /// <param name="plainTextBase64">The base64 encoded string containing the 'plain text' data to encrypt.</param>
        /// <returns>A byte array containing the 'cipher text' content.</returns>
        public static byte[] EncryptBase64ToBytes(this IEncryptor encryptor, string plainTextBase64)
        {
            _ = encryptor.WhenNotNull();
            _ = plainTextBase64.WhenNotNull();

            var plainTextBytes = Convert.FromBase64String(plainTextBase64);

            return encryptor.Encrypt(plainTextBytes);
        }

        /// <summary>Encrypts a base64 encoded string containing the 'plain text' data to a destination stream.</summary>
        /// <param name="encryptor">The encryptor instance.</param>
        /// <param name="plainTextBase64">The base64 encoded string containing the 'plain text' data to encrypt.</param>
        /// <param name="cipherTextStream">The destination stream containing the 'cipher text' content.</param>
        public static void EncryptBase64ToStream(this IStreamEncryptor encryptor, string plainTextBase64, Stream cipherTextStream)
        {
            _ = encryptor.WhenNotNull();
            _ = plainTextBase64.WhenNotNull();
            _ = cipherTextStream.WhenNotNull();

            var plainTextBytes = Convert.FromBase64String(plainTextBase64);

            EncryptBytesToStream(encryptor, plainTextBytes, cipherTextStream);
        }

        #endregion

        #region Decrypt from stream to ...

        /// <summary>Decrypts a source stream containing the 'cipher text' data to a byte array.</summary>
        /// <param name="encryptor">The encryptor instance.</param>
        /// <param name="cipherTextStream">The source stream containing the 'cipher text' data to decrypt.</param>
        /// <returns>A byte array containing the 'plain text' content.</returns>
        public static byte[] DecryptStreamToBytes(this IStreamEncryptor encryptor, Stream cipherTextStream)
        {
            _ = encryptor.WhenNotNull();
            _ = cipherTextStream.WhenNotNull();

            using var plainTextStream = new MemoryStream();

            encryptor.Decrypt(cipherTextStream, plainTextStream);

            return plainTextStream.ToArray();
        }

        /// <summary>Decrypts a source stream containing the 'cipher text' data to a base64 encoded string.</summary>
        /// <param name="encryptor">The encryptor instance.</param>
        /// <param name="cipherTextStream">The source stream containing the 'cipher text' data to decrypt.</param>
        /// <returns>A base64 encoded string containing the 'plain text' content.</returns>
        public static string DecryptStreamToBase64(this IStreamEncryptor encryptor, Stream cipherTextStream)
        {
            _ = encryptor.WhenNotNull();
            _ = cipherTextStream.WhenNotNull();

            var plainTextBytes = DecryptStreamToBytes(encryptor, cipherTextStream);

            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>Decrypts a source stream containing the 'cipher text' data to a string.</summary>
        /// <param name="encryptor">The encryptor instance.</param>
        /// <param name="cipherTextStream">The destination stream containing the cipher text content.</param>
        /// <returns>A string containing the 'plain text' content.</returns>
        public static string DecryptStreamToPlainText(this IStreamEncryptor encryptor, Stream cipherTextStream)
        {
            _ = encryptor.WhenNotNull();
            _ = cipherTextStream.WhenNotNull();

            var plainTextBytes = DecryptStreamToBytes(encryptor, cipherTextStream);

            return Encoding.UTF8.GetString(plainTextBytes);
        }

        #endregion

        #region Decrypt from bytes to ...

        /// <summary>Decrypts a byte array containing the 'cipher text' data to a base64 encoded string.</summary>
        /// <param name="encryptor">The encryptor instance.</param>
        /// <param name="cipherTextBytes">The byte array containing the 'cipher text' data to decrypt.</param>
        /// <returns>A base64 encoded string containing the 'plain text' content.</returns>
        public static string DecryptBytesToBase64(this IEncryptor encryptor, byte[] cipherTextBytes)
        {
            _ = encryptor.WhenNotNull();
            _ = cipherTextBytes.WhenNotNull();

            var plainTextBytes = encryptor.Decrypt(cipherTextBytes);

            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>Decrypts a byte array containing the 'cipher text' data to a string.</summary>
        /// <param name="encryptor">The encryptor instance.</param>
        /// <param name="cipherTextBytes">The byte array containing the 'cipher text' data to decrypt.</param>
        /// <returns>A string containing the 'plain text' content.</returns>
        public static string DecryptBytesToPlainText(this IEncryptor encryptor, byte[] cipherTextBytes)
        {
            _ = encryptor.WhenNotNull();
            _ = cipherTextBytes.WhenNotNull();

            var plainTextBytes = encryptor.Decrypt(cipherTextBytes);

            return Encoding.UTF8.GetString(plainTextBytes);
        }

        /// <summary>Decrypts a byte array containing the 'cipher text' data to a destination stream.</summary>
        /// <param name="encryptor">The encryptor instance.</param>
        /// <param name="cipherTextBytes">The byte array containing the 'cipher text' data to decrypt.</param>
        /// <param name="plainTextStream">The destination stream containing the 'plain text' content.</param>
        public static void DecryptBytesToStream(this IStreamEncryptor encryptor, byte[] cipherTextBytes, Stream plainTextStream)
        {
            _ = encryptor.WhenNotNull();
            _ = cipherTextBytes.WhenNotNull();
            _ = plainTextStream.WhenNotNull();

            using var cipherTextStream = new MemoryStream(cipherTextBytes);

            encryptor.Decrypt(cipherTextStream, plainTextStream);
        }

        #endregion

        #region Decrypt from base64 to ...

        /// <summary>Decrypts a base64 encoded string containing the 'cipher text' data to a byte array.</summary>
        /// <param name="encryptor">The encryptor instance.</param>
        /// <param name="cipherTextBase64">The base64 encoded string containing the 'cipher text' data to decrypt.</param>
        /// <returns>A byte array containing the 'plain text' content.</returns>
        public static byte[] DecryptBase64ToBytes(this IEncryptor encryptor, string cipherTextBase64)
        {
            _ = encryptor.WhenNotNull();
            _ = cipherTextBase64.WhenNotNull();

            var cipherTextBytes = Convert.FromBase64String(cipherTextBase64);

            return encryptor.Decrypt(cipherTextBytes);
        }

        /// <summary>Decrypts a base64 encoded string containing the 'cipher text' data to a string.</summary>
        /// <param name="encryptor">The encryptor instance.</param>
        /// <param name="cipherTextBase64">The base64 encoded string containing the 'cipher text' data to decrypt.</param>
        /// <returns>A string containing the 'plain text' content.</returns>
        public static string DecryptBase64ToPlainText(this IEncryptor encryptor, string cipherTextBase64)
        {
            _ = encryptor.WhenNotNull();
            _ = cipherTextBase64.WhenNotNull();

            var cipherTextBytes = Convert.FromBase64String(cipherTextBase64);

            return DecryptBytesToPlainText(encryptor, cipherTextBytes);
        }

        /// <summary>Decrypts a base64 encoded string containing the 'cipher text' data to a destination stream.</summary>
        /// <param name="encryptor">The encryptor instance.</param>
        /// <param name="cipherTextBase64">The base64 encoded string containing the 'cipher text' data to decrypt.</param>
        /// <param name="plainTextStream">The destination stream containing the 'plain text' content.</param>
        public static void DecryptBase64ToStream(this IStreamEncryptor encryptor, string cipherTextBase64, Stream plainTextStream)
        {
            _ = encryptor.WhenNotNull();
            _ = cipherTextBase64.WhenNotNull();
            _ = plainTextStream.WhenNotNull();

            var cipherTextBytes = Convert.FromBase64String(cipherTextBase64);

            DecryptBytesToStream(encryptor, cipherTextBytes, plainTextStream);
        }

        #endregion
    }
}
