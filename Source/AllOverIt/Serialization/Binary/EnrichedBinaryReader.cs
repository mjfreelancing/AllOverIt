using AllOverIt.Serialization.Binary.Exceptions;
using AllOverIt.Serialization.Binary.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AllOverIt.Serialization.Binary
{
    public interface IEnrichedBinaryValueReader
    {
        Type Type { get; }
        object ReadValue(EnrichedBinaryReader reader);
        TValue ReadValue<TValue>(EnrichedBinaryReader reader);
    }

    public abstract class EnrichedBinaryValueReader<TType> : IEnrichedBinaryValueReader
    {
        public Type Type => typeof(TType);

        public abstract object ReadValue(EnrichedBinaryReader reader);

        public TValue ReadValue<TValue>(EnrichedBinaryReader reader)
        {
            return (TValue)ReadValue(reader);
        }
    }


    public interface IEnrichedBinaryReader
    {
        IList<IEnrichedBinaryValueReader> Readers { get; }

        //
        // Summary:
        //     Closes the current reader and the underlying stream.
        void Close();

        //
        // Summary:
        //     Returns the next available character and does not advance the byte or character
        //     position.
        //
        // Returns:
        //     The next available character, or -1 if no more characters are available or the
        //     stream does not support seeking.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ArgumentException:
        //     The current character cannot be decoded into the internal character buffer by
        //     using the System.Text.Encoding selected for the stream.
        int PeekChar();

        //
        // Summary:
        //     Reads characters from the underlying stream and advances the current position
        //     of the stream in accordance with the Encoding used and the specific character
        //     being read from the stream.
        //
        // Returns:
        //     The next character from the input stream, or -1 if no characters are currently
        //     available.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        int Read();

        //
        // Summary:
        //     Reads the specified number of bytes from the stream, starting from a specified
        //     point in the byte array.
        //
        // Parameters:
        //   buffer:
        //     The buffer to read data into.
        //
        //   index:
        //     The starting point in the buffer at which to begin reading into the buffer.
        //
        //   count:
        //     The number of bytes to read.
        //
        // Returns:
        //     The number of bytes read into buffer. This might be less than the number of bytes
        //     requested if that many bytes are not available, or it might be zero if the end
        //     of the stream is reached.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The buffer length minus index is less than count. -or- The number of decoded
        //     characters to read is greater than count. This can happen if a Unicode decoder
        //     returns fallback characters or a surrogate pair.
        //
        //   T:System.ArgumentNullException:
        //     buffer is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     index or count is negative.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        int Read(byte[] buffer, int index, int count);

        //
        // Summary:
        //     Reads the specified number of characters from the stream, starting from a specified
        //     point in the character array.
        //
        // Parameters:
        //   buffer:
        //     The buffer to read data into.
        //
        //   index:
        //     The starting point in the buffer at which to begin reading into the buffer.
        //
        //   count:
        //     The number of characters to read.
        //
        // Returns:
        //     The total number of characters read into the buffer. This might be less than
        //     the number of characters requested if that many characters are not currently
        //     available, or it might be zero if the end of the stream is reached.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The buffer length minus index is less than count. -or- The number of decoded
        //     characters to read is greater than count. This can happen if a Unicode decoder
        //     returns fallback characters or a surrogate pair.
        //
        //   T:System.ArgumentNullException:
        //     buffer is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     index or count is negative.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        int Read(char[] buffer, int index, int count);

#if NETSTANDARD2_1 || NETCOREAPP3_1_OR_GREATER
        //
        // Summary:
        //     Reads a sequence of bytes from the current stream and advances the position within
        //     the stream by the number of bytes read.
        //
        // Parameters:
        //   buffer:
        //     A region of memory. When this method returns, the contents of this region are
        //     replaced by the bytes read from the current source.
        //
        // Returns:
        //     The total number of bytes read into the buffer. This can be less than the number
        //     of bytes allocated in the buffer if that many bytes are not currently available,
        //     or zero (0) if the end of the stream has been reached.
        //
        // Exceptions:
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        int Read(Span<byte> buffer);

        //
        // Summary:
        //     Reads, from the current stream, the same number of characters as the length of
        //     the provided buffer, writes them in the provided buffer, and advances the current
        //     position in accordance with the Encoding used and the specific character being
        //     read from the stream.
        //
        // Parameters:
        //   buffer:
        //     A span of characters. When this method returns, the contents of this region are
        //     replaced by the characters read from the current source.
        //
        // Returns:
        //     The total number of characters read into the buffer. This might be less than
        //     the number of characters requested if that many characters are not currently
        //     available, or it might be zero if the end of the stream is reached.
        //
        // Exceptions:
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        int Read(Span<char> buffer);
#endif

        //
        // Summary:
        //     Reads a Boolean value from the current stream and advances the current position
        //     of the stream by one byte.
        //
        // Returns:
        //     true if the byte is nonzero; otherwise, false.
        //
        // Exceptions:
        //   T:System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        bool ReadBoolean();

        //
        // Summary:
        //     Reads the next byte from the current stream and advances the current position
        //     of the stream by one byte.
        //
        // Returns:
        //     The next byte read from the current stream.
        //
        // Exceptions:
        //   T:System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        byte ReadByte();

        //
        // Summary:
        //     Reads the specified number of bytes from the current stream into a byte array
        //     and advances the current position by that number of bytes.
        //
        // Parameters:
        //   count:
        //     The number of bytes to read. This value must be 0 or a non-negative number or
        //     an exception will occur.
        //
        // Returns:
        //     A byte array containing data read from the underlying stream. This might be less
        //     than the number of bytes requested if the end of the stream is reached.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The number of decoded characters to read is greater than count. This can happen
        //     if a Unicode decoder returns fallback characters or a surrogate pair.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     count is negative.
        byte[] ReadBytes(int count);

        //
        // Summary:
        //     Reads the next character from the current stream and advances the current position
        //     of the stream in accordance with the Encoding used and the specific character
        //     being read from the stream.
        //
        // Returns:
        //     A character read from the current stream.
        //
        // Exceptions:
        //   T:System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ArgumentException:
        //     A surrogate character was read.
        char ReadChar();

        //
        // Summary:
        //     Reads the specified number of characters from the current stream, returns the
        //     data in a character array, and advances the current position in accordance with
        //     the Encoding used and the specific character being read from the stream.
        //
        // Parameters:
        //   count:
        //     The number of characters to read.
        //
        // Returns:
        //     A character array containing data read from the underlying stream. This might
        //     be less than the number of characters requested if the end of the stream is reached.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     The number of decoded characters to read is greater than count. This can happen
        //     if a Unicode decoder returns fallback characters or a surrogate pair.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     count is negative.
        char[] ReadChars(int count);

        //
        // Summary:
        //     Reads a decimal value from the current stream and advances the current position
        //     of the stream by sixteen bytes.
        //
        // Returns:
        //     A decimal value read from the current stream.
        //
        // Exceptions:
        //   T:System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        decimal ReadDecimal();

        //
        // Summary:
        //     Reads an 8-byte floating point value from the current stream and advances the
        //     current position of the stream by eight bytes.
        //
        // Returns:
        //     An 8-byte floating point value read from the current stream.
        //
        // Exceptions:
        //   T:System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        double ReadDouble();

        //
        // Summary:
        //     Reads a 2-byte signed integer from the current stream and advances the current
        //     position of the stream by two bytes.
        //
        // Returns:
        //     A 2-byte signed integer read from the current stream.
        //
        // Exceptions:
        //   T:System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        short ReadInt16();

        //
        // Summary:
        //     Reads a 4-byte signed integer from the current stream and advances the current
        //     position of the stream by four bytes.
        //
        // Returns:
        //     A 4-byte signed integer read from the current stream.
        //
        // Exceptions:
        //   T:System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        int ReadInt32();

        //
        // Summary:
        //     Reads an 8-byte signed integer from the current stream and advances the current
        //     position of the stream by eight bytes.
        //
        // Returns:
        //     An 8-byte signed integer read from the current stream.
        //
        // Exceptions:
        //   T:System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        long ReadInt64();

        //
        // Summary:
        //     Reads a signed byte from this stream and advances the current position of the
        //     stream by one byte.
        //
        // Returns:
        //     A signed byte read from the current stream.
        //
        // Exceptions:
        //   T:System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        sbyte ReadSByte();

        //
        // Summary:
        //     Reads a 4-byte floating point value from the current stream and advances the
        //     current position of the stream by four bytes.
        //
        // Returns:
        //     A 4-byte floating point value read from the current stream.
        //
        // Exceptions:
        //   T:System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        float ReadSingle();

        //
        // Summary:
        //     Reads a string from the current stream. The string is prefixed with the length,
        //     encoded as an integer seven bits at a time.
        //
        // Returns:
        //     The string being read.
        //
        // Exceptions:
        //   T:System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        string ReadString();

        //
        // Summary:
        //     Reads a 2-byte unsigned integer from the current stream using little-endian encoding
        //     and advances the position of the stream by two bytes.
        //
        // Returns:
        //     A 2-byte unsigned integer read from this stream.
        //
        // Exceptions:
        //   T:System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        ushort ReadUInt16();

        //
        // Summary:
        //     Reads a 4-byte unsigned integer from the current stream and advances the position
        //     of the stream by four bytes.
        //
        // Returns:
        //     A 4-byte unsigned integer read from this stream.
        //
        // Exceptions:
        //   T:System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        uint ReadUInt32();

        //
        // Summary:
        //     Reads an 8-byte unsigned integer from the current stream and advances the position
        //     of the stream by eight bytes.
        //
        // Returns:
        //     An 8-byte unsigned integer read from this stream.
        //
        // Exceptions:
        //   T:System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ObjectDisposedException:
        //     The stream is closed.
        ulong ReadUInt64();


        // reads the type and value, including support for enumerable and dictionary
        object ReadObject();
    }

    public sealed class EnrichedBinaryReader : BinaryReader, IEnrichedBinaryReader
    {
        //private readonly Type ObjectType = typeof(object);

        private static readonly IDictionary<TypeMapping.TypeId, Func<EnrichedBinaryReader, object>> TypeIdReader = new Dictionary<TypeMapping.TypeId, Func<EnrichedBinaryReader, object>>
        {
            { TypeMapping.TypeId.Bool, reader => reader.ReadBoolean() },
            { TypeMapping.TypeId.Byte, reader => reader.ReadByte() },
            { TypeMapping.TypeId.SByte, reader => reader.ReadSByte() },
            { TypeMapping.TypeId.UShort, reader => reader.ReadUInt16() },
            { TypeMapping.TypeId.Short, reader => reader.ReadInt16() },
            { TypeMapping.TypeId.UInt, reader => reader.ReadUInt32() },
            { TypeMapping.TypeId.Int, reader => reader.ReadInt32() },
            { TypeMapping.TypeId.ULong, reader => reader.ReadUInt64() },
            { TypeMapping.TypeId.Long, reader => reader.ReadInt64() },
            { TypeMapping.TypeId.Float, reader => reader.ReadSingle() },
            { TypeMapping.TypeId.Double, reader => reader.ReadDouble() },
            { TypeMapping.TypeId.Decimal, reader => reader.ReadDecimal() },
            { TypeMapping.TypeId.String, reader => reader.ReadSafeString() },
            { TypeMapping.TypeId.Char, reader => reader.ReadChar() },
            { TypeMapping.TypeId.Enum, reader => reader.ReadEnum() },
            { TypeMapping.TypeId.Guid, reader => reader.ReadGuid() },
            { TypeMapping.TypeId.DateTime, reader => DateTime.FromBinary(reader.ReadInt64()) },
            { TypeMapping.TypeId.TimeSpan, reader => new TimeSpan(reader.ReadInt64()) },
            { TypeMapping.TypeId.Dictionary, reader => reader.ReadDictionary() },
            { TypeMapping.TypeId.Enumerable, reader => reader.ReadEnumerable() },
            {
                TypeMapping.TypeId.Cached, reader =>
                {
                    var cacheIndex = reader.ReadInt32();
                    var assemblyTypeName = reader._userDefinedTypeCache[cacheIndex];

                    var valueType = Type.GetType(assemblyTypeName);
                    var converter = reader.Readers.SingleOrDefault(converter => converter.Type == valueType);

                    return converter.ReadValue(reader);
                }
            },
            {
                TypeMapping.TypeId.UserDefined, reader =>
                {
                    var assemblyTypeName = reader.ReadString();
                    var valueType = Type.GetType(assemblyTypeName);                    // TODO: Check for null

                    if (valueType == null)
                    {
                        throw new BinaryReaderException($"Unknown type '{assemblyTypeName}'.");
                    }

                    // cache for later, to read the value as a cached user defined type
                    var cacheIndex = reader._userDefinedTypeCache.Keys.Count + 1;
                    reader._userDefinedTypeCache.Add(cacheIndex, assemblyTypeName);

                    var converter = reader.Readers.SingleOrDefault(converter => converter.Type == valueType);

                    return converter.ReadValue(reader);
                }
            }
        };

        private readonly IDictionary<int, string> _userDefinedTypeCache = new Dictionary<int, string>();

        public IList<IEnrichedBinaryValueReader> Readers { get; } = new List<IEnrichedBinaryValueReader>();

        /// <inheritdoc cref="BinaryReader(Stream)"/>
        public EnrichedBinaryReader(Stream output)
            : base(output)
        {
        }

        /// <inheritdoc cref="BinaryReader(Stream, Encoding)"/>
        public EnrichedBinaryReader(Stream output, Encoding encoding)
            : base(output, encoding)
        {
        }

        /// <inheritdoc cref="BinaryReader(Stream, Encoding, bool)"/>
        public EnrichedBinaryReader(Stream output, Encoding encoding, bool leaveOpen)
            : base(output, encoding, leaveOpen)
        {
        }

        public object ReadObject()
        {
            var typeId = ReadByte();

            var rawTypeId = (TypeMapping.TypeId) (typeId & ~0x80);       // Exclude the default bit flag

            object rawValue = default;

            // Applicable to strings and nullable types
            var haveValue = (typeId & (byte) TypeMapping.TypeId.DefaultValue) == 0;

            if (haveValue)
            {
                // Read the value
                rawValue = TypeIdReader[rawTypeId].Invoke(this);
            }

            return rawValue;
        }
    }
}
