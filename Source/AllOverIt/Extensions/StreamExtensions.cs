using AllOverIt.Assertion;
using System.Text;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="Stream"/> types.</summary>
    public static class StreamExtensions
    {
        /// <summary>Writes the content of a stream, from its' current position, to a byte array.</summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The content of a stream as a byte array.</returns>
        public static byte[] ToByteArray(this Stream stream)
        {
            _ = stream.WhenNotNull(nameof(stream));

            using (var reader = new BinaryReader(stream))
            {
                // Will read as much as the stream's length unless the end of the stream is reached
                return reader.ReadBytes((int) stream.Length);
            }
        }

        /// <summary>Reads the content of a byte array and writes it to a stream at its' current position.</summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="bytes">The byte array to write to the stream.</param>
        public static void FromByteArray(this Stream stream, byte[] bytes)
        {
            _ = stream.WhenNotNull(nameof(stream));
            _ = bytes.WhenNotNull(nameof(bytes));

            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.Write(bytes);
            }
        }
    }
}
