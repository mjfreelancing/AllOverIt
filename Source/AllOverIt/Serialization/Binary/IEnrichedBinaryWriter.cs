using System;
using System.Collections.Generic;
using System.IO;

namespace AllOverIt.Serialization.Binary
{
    /// <summary>Represents a binary writer that will serialize objects and values to a stream.</summary>
    public interface IEnrichedBinaryWriter
    {
        /// <summary>Provides a list of custom value writer that can serialize custom types to the underyling stream.</summary>
        IList<IEnrichedBinaryValueWriter> Writers { get; }

        #region BinaryWriter methods

        /// <summary>Closes the current writer and the underlying stream.</summary>
        void Close();

        /// <summary>Clears all buffers for the current writer and causes any buffered data to be written to the underlying stream.</summary>
        void Flush();

        /// <inheritdoc cref="BinaryWriter.Seek(int, SeekOrigin)"/>
        long Seek(int offset, SeekOrigin origin);

        /// <inheritdoc cref="BinaryWriter.Write(char[], int, int)"/>
        void Write(char[] chars);

        /// <inheritdoc cref="BinaryWriter.Write(byte[], int, int)"/>
        void Write(byte[] buffer, int index, int count);

        /// <inheritdoc cref="BinaryWriter.Write(ReadOnlySpan{byte})"/>
        void Write(ReadOnlySpan<byte> buffer);

        /// <inheritdoc cref="BinaryWriter.Write(ReadOnlySpan{char})"/>
        void Write(ReadOnlySpan<char> chars);

        /// <inheritdoc cref="BinaryWriter.Write(bool)"/>
        void Write(bool value);

        /// <inheritdoc cref="BinaryWriter.Write(byte)"/>
        void Write(byte value);

        /// <inheritdoc cref="BinaryWriter.Write(byte[])"/>
        void Write(byte[] buffer);

        /// <inheritdoc cref="BinaryWriter.Write(char)"/>
        void Write(char ch);

        /// <inheritdoc cref="BinaryWriter.Write(char[], int, int)"/>
        void Write(char[] chars, int index, int count);

        /// <inheritdoc cref="BinaryWriter.Write(decimal)"/>
        void Write(decimal value);

        /// <inheritdoc cref="BinaryWriter.Write(double)"/>
        void Write(double value);

        /// <inheritdoc cref="BinaryWriter.Write(short)"/>
        void Write(short value);

        /// <inheritdoc cref="BinaryWriter.Write(int)"/>
        void Write(int value);

        /// <inheritdoc cref="BinaryWriter.Write(long)"/>
        void Write(long value);

        /// <inheritdoc cref="BinaryWriter.Write(sbyte)"/>
        void Write(sbyte value);

        /// <inheritdoc cref="BinaryWriter.Write(float)"/>
        void Write(float value);

        /// <inheritdoc cref="BinaryWriter.Write(string)"/>
        void Write(string value);

        /// <inheritdoc cref="BinaryWriter.Write(ushort)"/>
        void Write(ushort value);

        /// <inheritdoc cref="BinaryWriter.Write(uint)"/>
        void Write(uint value);

        /// <inheritdoc cref="BinaryWriter.Write(ulong)"/>
        void Write(ulong value);

        #endregion

        /// <summary>Writes a value to the underlying stream, prefixed with a type identifier that will enable it to be read back again.
        /// The value cannot be null and it must have a type that is supported by the writer. IEnumerable and IDictionary types are supported
        /// so long as their underlying value types can be determined.</summary>
        /// <param name="value">The value to be written.</param>
        void WriteObject(object value);

        /// <summary>
        /// Writes a value to the underlying stream, prefixed with a type identifier that will enable it to be read back again.
        /// Null values are supported so long as the type information is available; it cannot be typeof(object).
        /// </summary>
        /// <param name="value">The value to be written.</param>
        /// <param name="type">The value type. A suitable type identifier will be written to the stream prior to the value. If
        /// this is typeof(object) then the value's runtime type information will be used; this cannot be typeof(object).</param>
        void WriteObject(object value, Type type);
    }
}
