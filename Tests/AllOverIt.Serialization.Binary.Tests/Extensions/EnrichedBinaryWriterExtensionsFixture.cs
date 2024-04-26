using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Serialization.Binary.Writers;
using AllOverIt.Serialization.Binary.Writers.Extensions;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Linq;

namespace AllOverIt.Serialization.Binary.Tests.Extensions
{
    // ALL TESTS ARE BEHAVIOURAL AND MAY NOT REFLECT HOW THE ACTUAL IMPLEMENTATION WORKS.
    // FUNCTIONAL TESTS COVER THE REAL IMPLEMENTATION.
    public class EnrichedBinaryWriterExtensionsFixture : FixtureBase
    {
        private enum DummyEnum
        {
            One,
            Two
        }

        private class DummyType
        {
            public int Prop1 { get; set; }
        }

        private readonly Fake<IEnrichedBinaryWriter> _writerFake;

        public EnrichedBinaryWriterExtensionsFixture()
        {
            this.UseFakeItEasy();

            _writerFake = this.CreateFake<IEnrichedBinaryWriter>();
        }

        public class WriteUInt64 : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteUInt64(null, Create<ulong>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<ulong>();

                _writerFake.FakedObject.WriteUInt64(value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteUInt32 : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteUInt32(null, Create<uint>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<uint>();

                _writerFake.FakedObject.WriteUInt32(value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteUInt16 : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteUInt16(null, Create<ushort>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<ushort>();

                _writerFake.FakedObject.WriteUInt16(value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteSafeString : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteSafeString(null, Create<string>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Not_Write_Value()
            {
                string value = default;

                _writerFake.FakedObject.WriteSafeString(value);

                _writerFake.CallsTo(fake => fake.Write(false)).MustHaveHappened();
                _writerFake.CallsTo(fake => fake.Write(value)).MustNotHaveHappened();
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<string>();

                _writerFake.FakedObject.WriteSafeString(value);

                _writerFake
                    .CallsTo(fake => fake.Write(true)).MustHaveHappened()
                    .Then(_writerFake.CallsTo(fake => fake.Write(value)).MustHaveHappened());
            }
        }

        public class WriteSingle : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteSingle(null, Create<float>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<float>();

                _writerFake.FakedObject.WriteSingle(value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteSByte : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteSByte(null, Create<sbyte>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<sbyte>();

                _writerFake.FakedObject.WriteSByte(value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteChars_Span : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteChars(null, Create<string>().AsSpan());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            // CS8175 - Cannot use ref local 'span' in anonymous method, lambda expression, or query expression
            //
            //[Fact]
            //public void Should_Write_Value()
            //{
            //    var value = Create<string>();
            //    var span = value.AsSpan();

            //    EnrichedBinaryWriterExtensions.WriteChars(_writerFake.FakedObject, span);

            //    _writerFake
            //        .CallsTo(fake => fake.Write(span))
            //        .MustHaveHappened();
            //}
        }

        public class WriteBytes_Span : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteBytes(null, (Span<byte>) Array.Empty<byte>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            // CS8175 - As above for WriteChars
        }

        public class WriteInt64 : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteInt64(null, Create<long>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<long>();

                _writerFake.FakedObject.WriteInt64(value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteInt32 : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteInt32(null, Create<int>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<int>();

                _writerFake.FakedObject.WriteInt32(value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteDouble : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteDouble(null, Create<double>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<double>();

                _writerFake.FakedObject.WriteDouble(value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteDecimal : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteDecimal(null, Create<decimal>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<decimal>();

                _writerFake.FakedObject.WriteDecimal(value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteChars_Index_Count : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteChars(null, Array.Empty<char>(), Create<int>(), Create<int>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = CreateMany<char>().ToArray();
                var index = Create<int>();
                var count = Create<int>();

                _writerFake.FakedObject.WriteChars(value, index, count);

                _writerFake
                    .CallsTo(fake => fake.Write(value, index, count))
                    .MustHaveHappened();
            }
        }

        public class WriteChars : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteChars(null, Array.Empty<char>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = CreateMany<char>().ToArray();

                _writerFake.FakedObject.WriteChars(value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteBytes_Index_Count : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteBytes(null, Array.Empty<byte>(), Create<int>(), Create<int>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = CreateMany<byte>().ToArray();
                var index = Create<int>();
                var count = Create<int>();

                _writerFake.FakedObject.WriteBytes(value, index, count);

                _writerFake
                    .CallsTo(fake => fake.Write(value, index, count))
                    .MustHaveHappened();
            }
        }

        public class WriteBytes : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteBytes(null, Array.Empty<byte>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = CreateMany<byte>().ToArray();

                _writerFake.FakedObject.WriteBytes(value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteByte : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteByte(null, Create<byte>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<byte>();

                _writerFake.FakedObject.WriteByte(value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteBoolean : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteBoolean(null, Create<bool>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<bool>();

                _writerFake.FakedObject.WriteBoolean(value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteInt16 : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteInt16(null, Create<short>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<short>();

                _writerFake.FakedObject.WriteInt16(value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteChar : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteChar(null, Create<char>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<char>();

                _writerFake.FakedObject.WriteChar(value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteGuid : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteGuid(null, Create<Guid>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<Guid>();

                _writerFake.FakedObject.WriteGuid(value);

                _writerFake
                    .CallsTo(fake => fake.Write(A<byte[]>.That.Matches(array => array.SequenceEqual(value.ToByteArray()))))
                    .MustHaveHappened();
            }
        }

        public class WriteEnum : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteEnum(null, Create<DummyEnum>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<DummyEnum>();

                _writerFake.FakedObject.WriteEnum(value);

                _writerFake
                    .CallsTo(fake => fake.Write(value.GetType().AssemblyQualifiedName)).MustHaveHappened()
                    .Then(_writerFake.CallsTo(fake => fake.Write($"{value}")).MustHaveHappened());
            }
        }

        public class WriteNullable : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteNullable<int>(null, Create<int>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                int? value = Create<int>();

                _writerFake.FakedObject.WriteNullable(value);

                _writerFake
                   .CallsTo(fake => fake.WriteObject(value, typeof(int?)))
                   .MustHaveHappened();
            }
        }

        public class WriteDateTime : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteDateTime(null, Create<DateTime>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<DateTime>();

                _writerFake.FakedObject.WriteDateTime(value);

                _writerFake
                    .CallsTo(fake => fake.Write(value.ToBinary()))
                    .MustHaveHappened();
            }
        }

        public class WriteTimeSpan : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteTimeSpan(null, Create<TimeSpan>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<TimeSpan>();

                _writerFake.FakedObject.WriteTimeSpan(value);

                _writerFake
                     .CallsTo(fake => fake.Write(value.Ticks))
                     .MustHaveHappened();
            }
        }

        public class WriteObject_Typed : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryWriterExtensions.WriteObject(null, Create<DummyType>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<DummyType>();

                EnrichedBinaryWriterExtensions.WriteObject(_writerFake.FakedObject, value);

                _writerFake
                   .CallsTo(fake => fake.WriteObject(value, typeof(DummyType)))
                   .MustHaveHappened();
            }
        }

        // The WriteEnumerable() and WriteDictionary() methods are best covered by the functional tests
    }
}