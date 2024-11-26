using AllOverIt.Fixture;
using AllOverIt.Serialization.Binary.Exceptions;
using AllOverIt.Serialization.Binary.Readers;
using AllOverIt.Serialization.Binary.Readers.Extensions;
using AllOverIt.Serialization.Binary.Writers;
using AllOverIt.Serialization.Binary.Writers.Extensions;
using FluentAssertions;
using System.Collections;
using System.Text;

namespace AllOverIt.Serialization.Binary.Tests
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
            public string EmptyString { get; set; }
            public char Char { get; set; }
            public DummyEnum Enum { get; set; }
            public Guid Guid { get; set; }
            public DateTime DateTime { get; set; }
            public TimeSpan TimeSpan { get; set; }
            public IEnumerable<string> Strings { get; set; }
            public IEnumerable<double> Doubles { get; set; }
            public IEnumerable<int?> NullableInts { get; set; }
            public IEnumerable<double> EmptyDoubles { get; set; }
            public IEnumerable<double> NullDoubles { get; set; }        // (property is null) Must use WriteObject() / ReadObject()
            public int[] IntArray { get; set; }
            public double[] EmptyDoubleArray { get; set; }
            public IDictionary<int, string> Dictionary { get; set; }

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
                writer.WriteSafeString(EmptyString);
                writer.Write(Char);

                // Items below require these extension methods or WriteObject()
                writer.WriteEnum(Enum);
                writer.WriteGuid(Guid);
                writer.WriteDateTime(DateTime);
                writer.WriteTimeSpan(TimeSpan);

                // Testing IEnumerable
                writer.WriteEnumerable((IEnumerable) Strings, typeof(string));  // Testing explicit type info, and first item is null
                writer.WriteEnumerable((IEnumerable) Doubles, null);
                writer.WriteEnumerable((IEnumerable) NullableInts);
                writer.WriteEnumerable((IEnumerable) EmptyDoubles);

                // WriteObject handles null as long as it knows the type
                writer.WriteObject(NullDoubles, typeof(double[]));

                writer.WriteEnumerable((IEnumerable) IntArray);
                writer.WriteEnumerable((IEnumerable) EmptyDoubleArray);

                writer.WriteObject(Dictionary);
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
                EmptyString = reader.ReadSafeString();
                Char = reader.ReadChar();

                // Using extension methods to match the write calls
                Enum = reader.ReadEnum<DummyEnum>();
                Guid = reader.ReadGuid();
                DateTime = reader.ReadDateTime();
                TimeSpan = reader.ReadTimeSpan();

                Strings = (List<string>) reader.ReadEnumerable();
                Doubles = (List<double>) reader.ReadEnumerable();
                NullableInts = (List<int?>) reader.ReadEnumerable();
                EmptyDoubles = (List<double>) reader.ReadEnumerable();

                NullDoubles = (IEnumerable<double>) reader.ReadObject();

                IntArray = ((IEnumerable<int>) reader.ReadEnumerable()).ToArray();
                EmptyDoubleArray = ((IEnumerable<double>) reader.ReadEnumerable()).ToArray();

                // Must use this syntax when written using WriteObject()
                // Same as reader.ReadObject<Dictionary<object, object>>().ToDictionary(kvp => (int) kvp.Key, kvp => (string) kvp.Value);
                Dictionary = reader.ReadObjectAsDictionary<int, string>();
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
                writer.WriteSafeString(EmptyString);

                writer.WriteChar(Char);

                // Method 1 used the extension methods, this method is using WriteObject
                writer.WriteObject(Enum);
                writer.WriteObject(Guid);
                writer.WriteObject(DateTime);
                writer.WriteObject(TimeSpan);

                writer.WriteEnumerable(Strings, typeof(string));  // Testing explicit type info, and first item is null
                writer.WriteEnumerable(Doubles);
                writer.WriteEnumerable(NullableInts);
                writer.WriteEnumerable(EmptyDoubles);

                // WriteObject handles null as long as it knows the type
                writer.WriteObject(NullDoubles, typeof(double[]));

                writer.WriteEnumerable(IntArray);
                writer.WriteEnumerable(EmptyDoubleArray);

                writer.WriteDictionary(Dictionary);                 // is generic overload <int, string>
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
                EmptyString = reader.ReadSafeString();

                Char = reader.ReadChar();

                // These were written using WriteObject()
                Enum = reader.ReadObject<DummyEnum>();
                Guid = reader.ReadObject<Guid>();
                DateTime = reader.ReadObject<DateTime>();
                TimeSpan = reader.ReadObject<TimeSpan>();

                // Using ReadEnumerable<T>() to compliment WriteEnumerable<T>()
                Strings = reader.ReadEnumerable<string>();
                Doubles = reader.ReadEnumerable<double>();
                NullableInts = reader.ReadEnumerable<int?>();
                EmptyDoubles = reader.ReadEnumerable<double>();

                NullDoubles = (IEnumerable<double>) reader.ReadObject();

                // ReadEnumerable() returns as a list so we need to ToArray()
                // See method 3 below which uses WriteArray() and ReadArray().
                // Note, WriteEnumerable() + ReadArray() will also work - but best to match the write / read methods used
                IntArray = reader.ReadEnumerable<int>().ToArray();
                EmptyDoubleArray = reader.ReadEnumerable<double>().ToArray();

                // Must use this syntax when using WriteDictionary()
                Dictionary = reader.ReadDictionary<int, string>();
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
                writer.WriteSafeString(EmptyString);
                writer.WriteObject(Char);
                writer.WriteObject(Enum);
                writer.WriteObject(Guid);
                writer.WriteObject(DateTime);
                writer.WriteObject(TimeSpan);
                writer.WriteObject(Strings);
                writer.WriteObject(Doubles);
                writer.WriteObject(NullableInts);
                writer.WriteObject(EmptyDoubles);
                writer.WriteObject(NullDoubles, typeof(IEnumerable<double>));

                // Using WriteArray as WriteObject() was used in another method
                writer.WriteArray(IntArray);
                writer.WriteArray(EmptyDoubleArray);

                writer.WriteObject(Dictionary);
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
                NullString = reader.ReadSafeString();                   // Can't use reader.ReadObject<string>()
                EmptyString = reader.ReadSafeString();

                Char = reader.ReadObject<char>();
                Enum = reader.ReadObject<DummyEnum>();
                Guid = reader.ReadObject<Guid>();
                DateTime = reader.ReadObject<DateTime>();
                TimeSpan = reader.ReadObject<TimeSpan>();

                // Using ReadObject() to compliment WriteObject()
                Strings = reader.ReadObject<IList<string>>();
                Doubles = reader.ReadObject<IList<double>>();
                NullableInts = reader.ReadObject<IList<int?>>();
                EmptyDoubles = reader.ReadObject<IList<double>>();

                NullDoubles = reader.ReadObject<IList<double>>();

                IntArray = reader.ReadArray<int>();
                EmptyDoubleArray = reader.ReadArray<double>();

                // Must use this syntax when written using WriteObject()
                // Same as reader.ReadObject<Dictionary<object, object>>().ToDictionary(kvp => (int) kvp.Key, kvp => (string) kvp.Value);
                Dictionary = reader.ReadObjectAsDictionary<int, string>();
            }

            // =================================================================================

        }

        [Fact]
        public void Should_Throw_When_Value_Null_Type_Null()
        {
            Invoking(() =>
            {
                using (var stream = new MemoryStream())
                {
                    var writer = new EnrichedBinaryWriter(stream);

                    writer.WriteObject(null, null);
                }
            })
            .Should()
            .Throw<BinaryWriterException>()
            .WithMessage("All binary serialized values must be typed or have a non-null value.");
        }

        [Fact]
        public void Should_Throw_When_Value_Null_Type_Object()
        {
            Invoking(() =>
            {
                using (var stream = new MemoryStream())
                {
                    var writer = new EnrichedBinaryWriter(stream);

                    writer.WriteObject(null, typeof(object));
                }
            })
            .Should()
            .Throw<BinaryWriterException>()
            .WithMessage("All binary serialized values must be typed or have a non-null value.");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Should_Write_Defaults_All_Constructors(int constructor)
        {
            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                var writer = constructor switch
                {
                    1 => new EnrichedBinaryWriter(stream),
                    2 => new EnrichedBinaryWriter(stream, Encoding.UTF8),
                    _ => new EnrichedBinaryWriter(stream, Encoding.UTF8, true),
                };

                using (writer)
                {
                    writer.WriteObject(null, Reflection.CommonTypes.StringType);
                }

                bytes = stream.ToArray();
            }

            var actual = string.Empty;

            using (var stream = new MemoryStream(bytes))
            {
                var reader = constructor switch
                {
                    1 => new EnrichedBinaryReader(stream),
                    2 => new EnrichedBinaryReader(stream, Encoding.UTF8),
                    _ => new EnrichedBinaryReader(stream, Encoding.UTF8, true),
                };

                using (reader)
                {
                    // Should be read back as null
                    actual = (string) reader.ReadObject();
                }
            }

            actual.Should().BeNull();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Should_Write_Using_Write_Method1_Using_All_Constructors(int constructor)
        {
            var expected = CreateKnownTypes();

            KnownTypes actual = new();

            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                var writer = constructor switch
                {
                    1 => new EnrichedBinaryWriter(stream),
                    2 => new EnrichedBinaryWriter(stream, Encoding.UTF8),
                    _ => new EnrichedBinaryWriter(stream, Encoding.UTF8, true),
                };

                using (writer)
                {
                    expected.WriteUsingWrite_Method1(writer);
                }

                bytes = stream.ToArray();
            }

            using (var stream = new MemoryStream(bytes))
            {
                var reader = constructor switch
                {
                    1 => new EnrichedBinaryReader(stream),
                    2 => new EnrichedBinaryReader(stream, Encoding.UTF8),
                    _ => new EnrichedBinaryReader(stream, Encoding.UTF8, true),
                };

                using (reader)
                {
                    actual.Read_Method1(reader);
                }
            }

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Should_Write_Using_Write_Extensions_Method2_Using_All_Constructors(int constructor)
        {
            var expected = CreateKnownTypes();

            KnownTypes actual = new();

            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                var writer = constructor switch
                {
                    1 => new EnrichedBinaryWriter(stream),
                    2 => new EnrichedBinaryWriter(stream, Encoding.UTF8),
                    _ => new EnrichedBinaryWriter(stream, Encoding.UTF8, true),
                };

                using (writer)
                {
                    expected.WriteUsingExtensions_Method2(writer);
                }

                bytes = stream.ToArray();
            }

            using (var stream = new MemoryStream(bytes))
            {
                var reader = constructor switch
                {
                    1 => new EnrichedBinaryReader(stream),
                    2 => new EnrichedBinaryReader(stream, Encoding.UTF8),
                    _ => new EnrichedBinaryReader(stream, Encoding.UTF8, true),
                };

                using (reader)
                {
                    actual.Read_Method2(reader);
                }
            }

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Should_Write_All_As_Objects()
        {
            var expected = CreateKnownTypes();

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

        [Fact]
        public void Should_Write_RangeIterator_Using_WriteEnumerable()
        {
            var expected = Enumerable.Range(1, 10);     // returns a RangeIterator - no generic arguments

            IEnumerable<int> actual = null;

            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
                {
                    writer.WriteEnumerable(expected);
                }

                bytes = stream.ToArray();
            }

            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
                {
                    actual = reader.ReadEnumerable<int>();
                }
            }

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Should_Write_RangeIterator_Using_WriteObject()
        {
            var expected = Enumerable.Range(1, 10);     // returns a RangeIterator - no generic arguments

            IEnumerable<int> actual = null;

            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
                {
                    writer.WriteObject(expected);
                }

                bytes = stream.ToArray();
            }

            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
                {
                    // Must use ReadObject() to compliment WriteObject()
                    actual = ((IEnumerable<int>) reader.ReadObject());
                }
            }

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Should_Write_SelectIterator_Using_WriteEnumerable()
        {
            var expected = CreateMany<int>().Select(item => item);     // returns SelectListIterator<int?, object> - two generic arguments

            IEnumerable<int> actual = null;

            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
                {
                    writer.WriteEnumerable(expected);                   // interpreted as WriteEnumerable<int> so the type info will be used
                }

                bytes = stream.ToArray();
            }

            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
                {
                    actual = reader.ReadEnumerable<int>();
                }
            }

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Should_Write_SelectIterator_Using_WriteObject()
        {
            var expected = CreateMany<int>().Select(item => item);     // returns SelectListIterator<int?, object> - two generic arguments

            IEnumerable<int> actual = null;

            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
                {
                    writer.WriteObject(expected);
                }

                bytes = stream.ToArray();
            }

            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
                {
                    // Must use ReadObject() to compliment WriteObject()
                    actual = ((IEnumerable<int>) reader.ReadObject());
                }
            }

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Should_Write_Value_When_Object_Type()
        {
            var expected = Create<int>();

            var actual = 0;

            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
                {
                    writer.WriteObject(expected, typeof(object));
                }

                bytes = stream.ToArray();
            }

            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
                {
                    actual = reader.ReadObject<int>();
                }
            }

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_Write_Non_Generic_Dictionary()
        {
            var expected = Environment.GetEnvironmentVariables();       // IDictionary
            IDictionary<string, string> actual = null;                  // Expected to be read back as

            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
                {
                    writer.WriteDictionary(expected);
                }

                bytes = stream.ToArray();
            }

            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
                {
                    actual = reader.ReadDictionary<string, string>();
                }
            }

            actual.Should().BeEquivalentTo(expected);
        }

        private KnownTypes CreateKnownTypes()
        {
            var knownTypes = Create<KnownTypes>();

            knownTypes.Strings = new List<string> { null, "a", "", null, "b" };
            knownTypes.Doubles = knownTypes.Doubles.ToList();               // replace Autofixture type
            knownTypes.NullableInts = new int?[3] { 1, null, 3 };
            knownTypes.EmptyDoubleArray = Array.Empty<double>();
            knownTypes.EmptyDoubles = new List<double>();
            knownTypes.NullDoubles = null;
            knownTypes.NullString = null;
            knownTypes.EmptyString = string.Empty;

            return knownTypes;
        }
    }
}