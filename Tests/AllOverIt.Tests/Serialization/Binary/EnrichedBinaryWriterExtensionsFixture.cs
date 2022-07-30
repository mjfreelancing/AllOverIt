using AllOverIt.Fixture;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Extensions;
using FakeItEasy;
using System;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Serialization.Binary
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
            public void Should_Write_Value()
            {
                var value = Create<ulong>();

                EnrichedBinaryWriterExtensions.WriteUInt64(_writerFake.FakedObject, value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteUInt32 : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<uint>();

                EnrichedBinaryWriterExtensions.WriteUInt32(_writerFake.FakedObject, value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteUInt16 : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<ushort>();

                EnrichedBinaryWriterExtensions.WriteUInt16(_writerFake.FakedObject, value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteSafeString : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Not_Write_Value()
            {
                string value = default;

                EnrichedBinaryWriterExtensions.WriteSafeString(_writerFake.FakedObject, value);

                _writerFake.CallsTo(fake => fake.Write(false)).MustHaveHappened();
                _writerFake.CallsTo(fake => fake.Write(value)).MustNotHaveHappened();
            }

            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<string>();

                EnrichedBinaryWriterExtensions.WriteSafeString(_writerFake.FakedObject, value);

                _writerFake
                    .CallsTo(fake => fake.Write(true)).MustHaveHappened()
                    .Then(_writerFake.CallsTo(fake => fake.Write(value)).MustHaveHappened());
            }
        }

        public class WriteSingle : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<float>();

                EnrichedBinaryWriterExtensions.WriteSingle(_writerFake.FakedObject, value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteSByte : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<sbyte>();

                EnrichedBinaryWriterExtensions.WriteSByte(_writerFake.FakedObject, value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteChars_Span : EnrichedBinaryWriterExtensionsFixture
        {
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
            // CS8175 - As above for WriteChars
        }

        public class WriteInt64 : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<long>();

                EnrichedBinaryWriterExtensions.WriteInt64(_writerFake.FakedObject, value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteInt32 : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<int>();

                EnrichedBinaryWriterExtensions.WriteInt32(_writerFake.FakedObject, value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteDouble : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<double>();

                EnrichedBinaryWriterExtensions.WriteDouble(_writerFake.FakedObject, value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteDecimal : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<decimal>();

                EnrichedBinaryWriterExtensions.WriteDecimal(_writerFake.FakedObject, value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteChars_Index_Count : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Write_Value()
            {
                var value = CreateMany<char>().ToArray();
                var index = Create<int>();
                var count = Create<int>();

                EnrichedBinaryWriterExtensions.WriteChars(_writerFake.FakedObject, value, index, count);

                _writerFake
                    .CallsTo(fake => fake.Write(value, index, count))
                    .MustHaveHappened();
            }
        }

        public class WriteChars : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Write_Value()
            {
                var value = CreateMany<char>().ToArray();

                EnrichedBinaryWriterExtensions.WriteChars(_writerFake.FakedObject, value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteBytes_Index_Count : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Write_Value()
            {
                var value = CreateMany<byte>().ToArray();
                var index = Create<int>();
                var count = Create<int>();

                EnrichedBinaryWriterExtensions.WriteBytes(_writerFake.FakedObject, value, index, count);

                _writerFake
                    .CallsTo(fake => fake.Write(value, index, count))
                    .MustHaveHappened();
            }
        }

        public class WriteBytes : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Write_Value()
            {
                var value = CreateMany<byte>().ToArray();

                EnrichedBinaryWriterExtensions.WriteBytes(_writerFake.FakedObject, value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteByte : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<byte>();

                EnrichedBinaryWriterExtensions.WriteByte(_writerFake.FakedObject, value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteBoolean : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<bool>();

                EnrichedBinaryWriterExtensions.WriteBoolean(_writerFake.FakedObject, value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteInt16 : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<short>();

                EnrichedBinaryWriterExtensions.WriteInt16(_writerFake.FakedObject, value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteChar : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<char>();

                EnrichedBinaryWriterExtensions.WriteChar(_writerFake.FakedObject, value);

                _writerFake
                    .CallsTo(fake => fake.Write(value))
                    .MustHaveHappened();
            }
        }

        public class WriteGuid : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<Guid>();

                EnrichedBinaryWriterExtensions.WriteGuid(_writerFake.FakedObject, value);

                _writerFake
                    .CallsTo(fake => fake.Write(A<byte[]>.That.Matches(array => array.SequenceEqual(value.ToByteArray()))))
                    .MustHaveHappened();
            }
        }

        public class WriteEnum : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<DummyEnum>();

                EnrichedBinaryWriterExtensions.WriteEnum(_writerFake.FakedObject, value);

                _writerFake
                    .CallsTo(fake => fake.Write(value.GetType().AssemblyQualifiedName)).MustHaveHappened()
                    .Then(_writerFake.CallsTo(fake => fake.Write($"{value}")).MustHaveHappened());
            }
        }

        public class WriteNullable : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Write_Value()
            {
                int? value = Create<int>();

                EnrichedBinaryWriterExtensions.WriteNullable(_writerFake.FakedObject, value);

                _writerFake
                   .CallsTo(fake => fake.WriteObject(value, typeof(int?)))
                   .MustHaveHappened();
            }
        }

        public class WriteObject_Typed : EnrichedBinaryWriterExtensionsFixture
        {
            [Fact]
            public void Should_Write_Value()
            {
                var value = Create<DummyType>();

                EnrichedBinaryWriterExtensions.WriteObject<DummyType>(_writerFake.FakedObject, value);

                _writerFake
                   .CallsTo(fake => fake.WriteObject(value, typeof(DummyType)))
                   .MustHaveHappened();
            }
        }

        // The WriteEnumerable() and WriteDictionary() methods are best covered by the functional tests
    }
}