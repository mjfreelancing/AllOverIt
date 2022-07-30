using AllOverIt.Fixture;
using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Extensions;
using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace AllOverIt.Tests.Serialization
{
    public class EnrichedBinaryReaderWriterFixture : FixtureBase
    {
        private enum DummyEnum
        {
            One,
            Two
        }

        private sealed class KnownTypes
        {
            public bool Bool { get; set; }
            public byte Byte { get; set; }
            public sbyte SByte { get; set; }
            public ushort UShort { get; set; }
            public short Short { get; set; }
            public uint UInt { get; set; }
            public int Int { get; set; }
            public ulong ULong { get; set; }
            public long Long { get; set; }
            public float Float { get; set; }
            public double Double { get; set; }
            public decimal Decimal { get; set; }
            public string String { get; set; }
            public string NullString { get; set; }
            public char Char { get; set; }

            // Items below require WriteObject
            public DummyEnum Enum { get; set; }
            public Guid Guid { get; set; }
            public DateTime DateTime { get; set; }
            public TimeSpan TimeSpan { get; set; }

            // =================================================================================

            public void WriteUsingWrite_Method1(IEnrichedBinaryWriter writer)
            {
                writer.Write(Bool);
                writer.Write(Byte);
                writer.Write(SByte);
                writer.Write(UShort);
                writer.Write(Short);
                writer.Write(UInt);
                writer.Write(Int);
                writer.Write(ULong);
                writer.Write(Long);
                writer.Write(Float);
                writer.Write(Double);
                writer.Write(Decimal);
                writer.Write(String);
                writer.WriteSafeString(NullString);             // Can't use Write()
                writer.Write(Char);

                // Items below require WriteObject
                writer.WriteObject(Enum);
                writer.WriteObject(Guid);
                writer.WriteObject(DateTime);
                writer.WriteObject(TimeSpan);
            }

            public void Read_Method1(IEnrichedBinaryReader reader)
            {
                Bool = reader.ReadBoolean();
                Byte = reader.ReadByte();
                SByte = reader.ReadSByte();
                UShort = reader.ReadUInt16();
                Short = reader.ReadInt16();
                UInt = reader.ReadUInt32();
                Int = reader.ReadInt32();
                ULong = reader.ReadUInt64();
                Long = reader.ReadInt64();
                Float = reader.ReadSingle();
                Double = reader.ReadDouble();
                Decimal = reader.ReadDecimal();
                String = reader.ReadString();
                NullString = reader.ReadSafeString();           // Can't use reader.ReadString()
                Char = reader.ReadChar();

                // These were written using WriteObject()
                Enum = reader.ReadObject<DummyEnum>();
                Guid = reader.ReadObject<Guid>();
                DateTime = reader.ReadObject<DateTime>();
                TimeSpan = reader.ReadObject<TimeSpan>();
            }

            // =================================================================================

            public void WriteUsingExtensions_Method2(IEnrichedBinaryWriter writer)
            {
                writer.WriteBoolean(Bool);
                writer.WriteByte(Byte);
                writer.WriteSByte(SByte);
                writer.WriteUInt16(UShort);
                writer.WriteInt16(Short);
                writer.WriteUInt32(UInt);
                writer.WriteInt32(Int);
                writer.WriteUInt64(ULong);
                writer.WriteInt64(Long);
                writer.WriteSingle(Float);
                writer.WriteDouble(Double);
                writer.WriteDecimal(Decimal);

                writer.WriteSafeString(String);
                writer.WriteSafeString(NullString);

                writer.WriteChar(Char);

                // Items below require WriteObject
                writer.WriteObject(Enum);
                writer.WriteObject(Guid);
                writer.WriteObject(DateTime);
                writer.WriteObject(TimeSpan);
            }

            public void Read_Method2(IEnrichedBinaryReader reader)
            {
                Bool = reader.ReadBoolean();
                Byte = reader.ReadByte();
                SByte = reader.ReadSByte();
                UShort = reader.ReadUInt16();
                Short = reader.ReadInt16();
                UInt = reader.ReadUInt32();
                Int = reader.ReadInt32();
                ULong = reader.ReadUInt64();
                Long = reader.ReadInt64();
                Float = reader.ReadSingle();
                Double = reader.ReadDouble();
                Decimal = reader.ReadDecimal();
                
                String = reader.ReadSafeString();
                NullString = reader.ReadSafeString();

                Char = reader.ReadChar();

                // These were written using WriteObject()
                Enum = reader.ReadObject<DummyEnum>();
                Guid = reader.ReadObject<Guid>();
                DateTime = reader.ReadObject<DateTime>();
                TimeSpan = reader.ReadObject<TimeSpan>();
            }

            // =================================================================================

            public void WriteAsObjects_Method3(IEnrichedBinaryWriter writer)
            {
                writer.WriteObject(Bool);
                writer.WriteObject(Byte);
                writer.WriteObject(SByte);
                writer.WriteObject(UShort);
                writer.WriteObject(Short);
                writer.WriteObject(UInt);
                writer.WriteObject(Int);
                writer.WriteObject(ULong);
                writer.WriteObject(Long);
                writer.WriteObject(Float);
                writer.WriteObject(Double);
                writer.WriteObject(Decimal);
                writer.WriteObject(String);
                writer.WriteSafeString(NullString);             // Can't use WriteObject()
                writer.WriteObject(Char);
                writer.WriteObject(Enum);
                writer.WriteObject(Guid);
                writer.WriteObject(DateTime);
                writer.WriteObject(TimeSpan);
            }

            public void Read_Method3(IEnrichedBinaryReader reader)
            {
                Bool = reader.ReadObject<bool>();
                Byte = reader.ReadObject<byte>();
                SByte = reader.ReadObject<sbyte>();
                UShort = reader.ReadObject<ushort>();
                Short = reader.ReadObject<short>();
                UInt = reader.ReadObject<uint>();
                Int = reader.ReadObject<int>();
                ULong = reader.ReadObject<ulong>();
                Long = reader.ReadObject<long>();
                Float = reader.ReadObject<float>();
                Double = reader.ReadObject<double>();
                Decimal = reader.ReadObject<decimal>();

                String = reader.ReadObject<string>();
                NullString = reader.ReadSafeString();           // Can't use reader.ReadObject<string>()

                Char = reader.ReadObject<char>();
                Enum = reader.ReadObject<DummyEnum>();
                Guid = reader.ReadObject<Guid>();
                DateTime = reader.ReadObject<DateTime>();
                TimeSpan = reader.ReadObject<TimeSpan>();
            }

            // =================================================================================


            /*
            { TypeIdentifier.Bool, reader => reader.ReadBoolean() },
            { TypeIdentifier.Byte, reader => reader.ReadByte() },
            { TypeIdentifier.SByte, reader => reader.ReadSByte() },
            { TypeIdentifier.UShort, reader => reader.ReadUInt16() },
            { TypeIdentifier.Short, reader => reader.ReadInt16() },
            { TypeIdentifier.UInt, reader => reader.ReadUInt32() },
            { TypeIdentifier.Int, reader => reader.ReadInt32() },
            { TypeIdentifier.ULong, reader => reader.ReadUInt64() },
            { TypeIdentifier.Long, reader => reader.ReadInt64() },
            { TypeIdentifier.Float, reader => reader.ReadSingle() },
            { TypeIdentifier.Double, reader => reader.ReadDouble() },
            { TypeIdentifier.Decimal, reader => reader.ReadDecimal() },
            { TypeIdentifier.String, reader => reader.ReadSafeString() },
            { TypeIdentifier.Char, reader => reader.ReadChar() },
            { TypeIdentifier.Enum, reader => reader.ReadEnum() },
            { TypeIdentifier.Guid, reader => reader.ReadGuid() },
            { TypeIdentifier.DateTime, reader => DateTime.FromBinary(reader.ReadInt64()) },
            { TypeIdentifier.TimeSpan, reader => new TimeSpan(reader.ReadInt64()) },
            { TypeIdentifier.Dictionary, reader => reader.ReadDictionary() },
            { TypeIdentifier.Enumerable, reader => reader.ReadEnumerable() },             
             */

        }

        [Fact]
        public void Should_Write_Using_Write_Method1()
        {
            var expected = Create<KnownTypes>();
            expected.NullString = default;

            KnownTypes actual = new();

            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
                {
                    expected.WriteUsingWrite_Method1(writer);
                }

                bytes = stream.ToArray();
            }

            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
                {
                    actual.Read_Method1(reader);
                }
            }

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Should_Write_Using_Write_Extensions_Method2()
        {
            var expected = Create<KnownTypes>();
            expected.NullString = default;

            KnownTypes actual = new();

            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
                {
                    expected.WriteUsingExtensions_Method2(writer);
                }

                bytes = stream.ToArray();
            }

            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
                {
                    actual.Read_Method2(reader);
                }
            }

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Should_Write_All_As_Objects()
        {
            var expected = Create<KnownTypes>();
            expected.NullString = default;

            KnownTypes actual = new();

            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
                {
                    expected.WriteAsObjects_Method3(writer);
                }

                bytes = stream.ToArray();
            }

            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
                {
                    actual.Read_Method3(reader);
                }
            }

            actual.Should().BeEquivalentTo(expected);
        }
    }
}