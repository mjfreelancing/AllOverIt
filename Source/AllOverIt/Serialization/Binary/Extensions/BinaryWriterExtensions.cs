using AllOverIt.Extensions;
using System;
using System.IO;

namespace AllOverIt.Serialization.Binary.Extensions
{
    public static class BinaryWriterExtensions
    {
        // TODO: To be moved as these are system extensions
        
        /// <inheritdoc cref="BinaryWriter.Write(ulong)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteUInt64(this BinaryWriter writer, ulong value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(uint)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteUInt32(this BinaryWriter writer, uint value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(ushort)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteUInt16(this BinaryWriter writer, ushort value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(string)"/>
        /// <param name="writer">The binary writer.</param>
        /// <remarks>The value can null.</remarks>
        public static void WriteSafeString(this BinaryWriter writer, string value)
        {
            var hasValue = value.IsNotNullOrEmpty();
            writer.WriteBoolean(hasValue);

            if (hasValue)
            {
                writer.Write(value);
            }
        }

        /// <inheritdoc cref="BinaryWriter.Write(float)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteSingle(this BinaryWriter writer, float value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(sbyte)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteSByte(this BinaryWriter writer, sbyte value) => writer.Write(value);

#if NETSTANDARD2_1 || NETCOREAPP3_1_OR_GREATER
        /// <inheritdoc cref="BinaryWriter.Write(ReadOnlySpan{char})"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteChars(this BinaryWriter writer, ReadOnlySpan<char> chars) => writer.Write(chars);

        /// <inheritdoc cref="BinaryWriter.Write(ReadOnlySpan{byte})"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteBytes(this BinaryWriter writer, ReadOnlySpan<byte> buffer) => writer.Write(buffer);
#endif

        /// <inheritdoc cref="BinaryWriter.Write(long)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteInt64(this BinaryWriter writer, long value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(int)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteInt32(this BinaryWriter writer, int value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(double)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteDouble(this BinaryWriter writer, double value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(decimal)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteDecimal(this BinaryWriter writer, decimal value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(char[], int, int)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteChars(this BinaryWriter writer, char[] chars, int index, int count) => writer.Write(chars, index, count);

        /// <inheritdoc cref="BinaryWriter.Write(char[])"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteChars(this BinaryWriter writer, char[] chars) => writer.Write(chars);

        /// <inheritdoc cref="BinaryWriter.Write(byte[], int, int)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteBytes(this BinaryWriter writer, byte[] buffer, int index, int count) => writer.Write(buffer, index, count);

        /// <inheritdoc cref="BinaryWriter.Write(byte[])"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteBytes(this BinaryWriter writer, byte[] buffer) => writer.Write(buffer);

        /// <inheritdoc cref="BinaryWriter.Write(byte)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteByte(this BinaryWriter writer, byte value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(bool)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteBoolean(this BinaryWriter writer, bool value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(short)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteInt16(this BinaryWriter writer, short value) => writer.Write(value);

        /// <inheritdoc cref="BinaryWriter.Write(char)"/>
        /// <param name="writer">The binary writer.</param>
        public static void WriteChar(this BinaryWriter writer, char value) => writer.Write(value);

        /// <summary>Writes a GUID value to the current stream.</summary>
        /// <param name="writer">The binary writer.</param>
        public static void WriteGuid(this BinaryWriter writer, Guid value) => writer.WriteBytes(value.ToByteArray());


        /// <summary>Writes an Enum value to the current stream.</summary>
        /// <param name="writer">The binary writer.</param>
        /// <remarks>The enum's assembly qualified type name and it's string value are written.</remarks>
        public static void WriteEnum(this BinaryWriter writer, object value)
        {
            // Need the string representation of the value in order to convert it back to the original Enum type.
            // Convert.ChangeType() cannot convert an integral type to an Enum type.
            writer.Write(value.GetType().AssemblyQualifiedName);
            writer.Write($"{value}");   
        }



      
    }

}
