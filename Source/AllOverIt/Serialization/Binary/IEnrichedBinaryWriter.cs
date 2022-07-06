using System;
using System.Collections.Generic;
using System.IO;

namespace AllOverIt.Serialization.Binary
{
    public interface IEnrichedBinaryWriter
    {
        IList<IEnrichedBinaryValueWriter> Writers { get; }

        #region BinaryWriter

        /// <summary>Closes the current writer and the underlying stream.</summary>
        void Close();

        /// <summary>Clears all buffers for the current writer and causes any buffered data to be written to the underlying device.</summary>
        void Flush();
        
        /// <summary>Sets the position within the current stream.</summary>
        /// <param name="offset">A byte offset relative to origin.</param>
        /// <param name="origin">Indicates the reference point from which the new position is to be obtained.</param>
        /// <returns>The position with the current stream.</returns>
        long Seek(int offset, SeekOrigin origin);
        
        //
        // Summary:
        //     Writes an eight-byte unsigned integer to the current stream and advances the
        //     stream position by eight bytes.
        //
        // Parameters:
        //   value:
        //     The eight-byte unsigned integer to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(ulong value);

        //
        // Summary:
        //     Writes a four-byte unsigned integer to the current stream and advances the stream
        //     position by four bytes.
        //
        // Parameters:
        //   value:
        //     The four-byte unsigned integer to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(uint value);

        //
        // Summary:
        //     Writes a two-byte unsigned integer to the current stream and advances the stream
        //     position by two bytes.
        //
        // Parameters:
        //   value:
        //     The two-byte unsigned integer to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(ushort value);
        
        //
        // Summary:
        //     Writes a length-prefixed string to this stream in the current encoding of the
        //     System.IO.BinaryWriter, and advances the current position of the stream in accordance
        //     with the encoding used and the specific characters being written to the stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ArgumentNullException:
        //     value is null.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(string value);

        //
        // Summary:
        //     Writes a four-byte floating-point value to the current stream and advances the
        //     stream position by four bytes.
        //
        // Parameters:
        //   value:
        //     The four-byte floating-point value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(float value);

        //
        // Summary:
        //     Writes a signed byte to the current stream and advances the stream position by
        //     one byte.
        //
        // Parameters:
        //   value:
        //     The signed byte to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(sbyte value);

#if NETSTANDARD2_1 || NETCOREAPP3_1_OR_GREATER
        //
        // Summary:
        //     Writes a span of characters to the current stream, and advances the current position
        //     of the stream in accordance with the Encoding used and perhaps the specific characters
        //     being written to the stream.
        //
        // Parameters:
        //   chars:
        //     A span of chars to write.
        void Write(ReadOnlySpan<char> chars);

        //
        // Summary:
        //     Writes a span of bytes to the current stream.
        //
        // Parameters:
        //   buffer:
        //     The span of bytes to write.
        void Write(ReadOnlySpan<byte> buffer);
#endif

        //
        // Summary:
        //     Writes an eight-byte signed integer to the current stream and advances the stream
        //     position by eight bytes.
        //
        // Parameters:
        //   value:
        //     The eight-byte signed integer to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(long value);

        //
        // Summary:
        //     Writes a four-byte signed integer to the current stream and advances the stream
        //     position by four bytes.
        //
        // Parameters:
        //   value:
        //     The four-byte signed integer to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(int value);

        //
        // Summary:
        //     Writes an eight-byte floating-point value to the current stream and advances
        //     the stream position by eight bytes.
        //
        // Parameters:
        //   value:
        //     The eight-byte floating-point value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(double value);

        //
        // Summary:
        //     Writes a decimal value to the current stream and advances the stream position
        //     by sixteen bytes.
        //
        // Parameters:
        //   value:
        //     The decimal value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(decimal value);

        //
        // Summary:
        //     Writes a section of a character array to the current stream, and advances the
        //     current position of the stream in accordance with the Encoding used and perhaps
        //     the specific characters being written to the stream.
        //
        // Parameters:
        //   chars:
        //     A character array containing the data to write.
        //
        //   index:
        //     The index of the first character to read from chars and to write to the stream.
        //
        //   count:
        //     The number of characters to read from chars and to write to the stream.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The buffer length minus index is less than count.
        //
        //   T:System.ArgumentNullException:
        //     chars is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     index or count is negative.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(char[] chars, int index, int count);

        //
        // Summary:
        //     Writes a character array to the current stream and advances the current position
        //     of the stream in accordance with the Encoding used and the specific characters
        //     being written to the stream.
        //
        // Parameters:
        //   chars:
        //     A character array containing the data to write.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     chars is null.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        void Write(char[] chars);

        //
        // Summary:
        //     Writes a region of a byte array to the current stream.
        //
        // Parameters:
        //   buffer:
        //     A byte array containing the data to write.
        //
        //   index:
        //     The index of the first byte to read from buffer and to write to the stream.
        //
        //   count:
        //     The number of bytes to read from buffer and to write to the stream.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The buffer length minus index is less than count.
        //
        //   T:System.ArgumentNullException:
        //     buffer is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     index or count is negative.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(byte[] buffer, int index, int count);

        //
        // Summary:
        //     Writes a byte array to the underlying stream.
        //
        // Parameters:
        //   buffer:
        //     A byte array containing the data to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.ArgumentNullException:
        //     buffer is null.
        void Write(byte[] buffer);

        //
        // Summary:
        //     Writes an unsigned byte to the current stream and advances the stream position
        //     by one byte.
        //
        // Parameters:
        //   value:
        //     The unsigned byte to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(byte value);

        //
        // Summary:
        //     Writes a one-byte Boolean value to the current stream, with 0 representing false
        //     and 1 representing true.
        //
        // Parameters:
        //   value:
        //     The Boolean value to write (0 or 1).
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(bool value);

        //
        // Summary:
        //     Writes a two-byte signed integer to the current stream and advances the stream
        //     position by two bytes.
        //
        // Parameters:
        //   value:
        //     The two-byte signed integer to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        void Write(short value);

        //
        // Summary:
        //     Writes a Unicode character to the current stream and advances the current position
        //     of the stream in accordance with the Encoding used and the specific characters
        //     being written to the stream.
        //
        // Parameters:
        //   ch:
        //     The non-surrogate, Unicode character to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurs.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.ArgumentException:
        //     ch is a single surrogate character.
        void Write(char ch);

        #endregion


        // Includes support for enumerable and dictionary. Methods also exist for these (as extensions) purely for readability reasons.
        void WriteObject(object value);
        void WriteObject(object value, Type type);
    }
}
