using System;
using System.Collections.Generic;
using System.IO;

namespace AllOverIt.Serialization.Binary
{
    /// <summary>Represents a binary reader that will deserialize objects and values from a stream.</summary>
    public interface IEnrichedBinaryReader
    {
        /// <summary>Provides a list of custom value readers that can deserialize custom types from the underyling stream.</summary>
        IList<IEnrichedBinaryValueReader> Readers { get; }

        #region Implemented by BinaryReader

        /// <inheritdoc cref="BinaryReader.Close"/>
        void Close();

        /// <inheritdoc cref="BinaryReader.PeekChar"/>
        int PeekChar();

        /// <inheritdoc cref="BinaryReader.Read()"/>
        int Read();

        /// <inheritdoc cref="BinaryReader.Read(byte[], int, int)"/>
        int Read(byte[] buffer, int index, int count);

        /// <inheritdoc cref="BinaryReader.Read(char[], int, int)"/>
        int Read(char[] buffer, int index, int count);

        /// <inheritdoc cref="BinaryReader.Read(Span{byte})"/>
        int Read(Span<byte> buffer);

        /// <inheritdoc cref="BinaryReader.Read(Span{char})"/>
        int Read(Span<char> buffer);

        /// <inheritdoc cref="BinaryReader.ReadBoolean"/>
        bool ReadBoolean();

        /// <inheritdoc cref="BinaryReader.ReadByte"/>
        byte ReadByte();

        /// <inheritdoc cref="BinaryReader.ReadBytes"/>
        byte[] ReadBytes(int count);

        /// <inheritdoc cref="BinaryReader.ReadChar"/>
        char ReadChar();

        /// <inheritdoc cref="BinaryReader.ReadChars"/>
        char[] ReadChars(int count);

        /// <inheritdoc cref="BinaryReader.ReadDecimal"/>
        decimal ReadDecimal();

        /// <inheritdoc cref="BinaryReader.ReadDouble"/>
        double ReadDouble();

        /// <inheritdoc cref="BinaryReader.ReadInt16"/>
        short ReadInt16();

        /// <inheritdoc cref="BinaryReader.ReadInt32"/>
        int ReadInt32();

        /// <inheritdoc cref="BinaryReader.ReadInt64"/>
        long ReadInt64();

        /// <inheritdoc cref="BinaryReader.ReadSByte"/>
        sbyte ReadSByte();

        /// <inheritdoc cref="BinaryReader.ReadSingle"/>
        float ReadSingle();

        /// <inheritdoc cref="BinaryReader.ReadString"/>
        string ReadString();

        /// <inheritdoc cref="BinaryReader.ReadUInt16"/>
        ushort ReadUInt16();

        /// <inheritdoc cref="BinaryReader.ReadUInt32"/>
        uint ReadUInt32();

        /// <inheritdoc cref="BinaryReader.ReadUInt64"/>
        ulong ReadUInt64();

        #endregion

        /// <summary>Reads type information and its associated value from the stream. This method can only read values previously written by
        /// <see cref="IEnrichedBinaryWriter.WriteObject(object)"/> or <see cref="IEnrichedBinaryWriter.WriteObject(object, Type)"/>.</summary>
        /// <returns>The appropriately typed value, as an object.</returns>
        object ReadObject();
    }
}
