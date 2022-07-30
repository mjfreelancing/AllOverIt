using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Exceptions;
using AllOverIt.Serialization.Binary.Extensions;
using FakeItEasy;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Tests.Serialization.Binary
{
    public class EnrichedBinaryReaderExtensionsFixture : FixtureBase
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

        private readonly Fake<IEnrichedBinaryReader> _readerFake;

        public EnrichedBinaryReaderExtensionsFixture()
        {
            this.UseFakeItEasy();

            _readerFake = this.CreateFake<IEnrichedBinaryReader>();
        }

        public class ReadSafeString : EnrichedBinaryReaderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Reader_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryReaderExtensions.ReadSafeString(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("reader");
            }

            [Fact]
            public void Should_Not_Call_ReadString_When_Has_Value()
            {
                _readerFake
                    .CallsTo(fake => fake.ReadBoolean())
                    .Returns(false);

                EnrichedBinaryReaderExtensions.ReadSafeString(_readerFake.FakedObject);

                _readerFake.CallsTo(fake => fake.ReadBoolean()).MustHaveHappened();
                _readerFake.CallsTo(fake => fake.ReadString()).MustNotHaveHappened();
            }

            [Fact]
            public void Should_Call_ReadString_When_Has_Value()
            {
                _readerFake
                    .CallsTo(fake => fake.ReadBoolean())
                    .Returns(true);

                EnrichedBinaryReaderExtensions.ReadSafeString(_readerFake.FakedObject);

                _readerFake
                    .CallsTo(fake => fake.ReadBoolean()).MustHaveHappened()
                    .Then(_readerFake.CallsTo(fake => fake.ReadString()).MustHaveHappened());
            }

            [Fact]
            public void Should_Return_Default()
            {
                _readerFake
                    .CallsTo(fake => fake.ReadBoolean())
                    .Returns(false);

                var actual = EnrichedBinaryReaderExtensions.ReadSafeString(_readerFake.FakedObject);

                actual.Should().Be(default);
            }

            [Fact]
            public void Should_Return_String()
            {
                var value = Create<string>();

                _readerFake
                    .CallsTo(fake => fake.ReadBoolean())
                    .Returns(true);

                _readerFake
                    .CallsTo(fake => fake.ReadString())
                    .Returns(value);

                var actual = EnrichedBinaryReaderExtensions.ReadSafeString(_readerFake.FakedObject);

                actual.Should().Be(value);
            }
        }

        public class ReadGuid : EnrichedBinaryReaderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Reader_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryReaderExtensions.ReadGuid(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("reader");
            }

            [Fact]
            public void Should_Read_Guid()
            {
                var value = Create<Guid>();

                _readerFake
                    .CallsTo(fake => fake.ReadBytes(16))
                    .Returns(value.ToByteArray());

                var actual = EnrichedBinaryReaderExtensions.ReadGuid(_readerFake.FakedObject);

                actual.Should().Be(value);
            }
        }

        public class ReadEnum : EnrichedBinaryReaderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Reader_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryReaderExtensions.ReadEnum(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("reader");
            }

            [Fact]
            public void Should_Throw_When_Unknown_Enum()
            {
                var enumTypeName = Create<string>();

                _readerFake
                    .CallsTo(fake => fake.ReadString())
                    .Returns(enumTypeName);

                Invoking(() =>
                {
                    _ = EnrichedBinaryReaderExtensions.ReadEnum(_readerFake.FakedObject);
                })
                   .Should()
                   .Throw<BinaryReaderException>()
                   .WithMessage($"Unknown enum type '{enumTypeName}'.");                
            }

            [Fact]
            public void Should_Read_Enum()
            {
                var value = Create<DummyEnum>();
                var enumTypeName = typeof(DummyEnum).AssemblyQualifiedName;

                _readerFake
                    .CallsTo(fake => fake.ReadString())
                    .ReturnsNextFromSequence(new[] { enumTypeName, $"{value}" });

                var actual = EnrichedBinaryReaderExtensions.ReadEnum(_readerFake.FakedObject);

                actual.Should().Be(value);
            }
        }

        public class ReadEnum_Typed : EnrichedBinaryReaderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Reader_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryReaderExtensions.ReadEnum<DummyEnum>(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("reader");
            }

        }

        public class ReadObject_Typed : EnrichedBinaryReaderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Reader_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryReaderExtensions.ReadObject<DummyType>(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("reader");
            }

        }

        public class ReadNullable : EnrichedBinaryReaderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Reader_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryReaderExtensions.ReadNullable<int>(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("reader");
            }

        }

        public class ReadEnumerable : EnrichedBinaryReaderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Reader_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryReaderExtensions.ReadEnumerable(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("reader");
            }

        }

        public class ReadEnumumerable_Typed : EnrichedBinaryReaderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Reader_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryReaderExtensions.ReadEnumerable<int>(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("reader");
            }

        }

        public class ReadDictionary : EnrichedBinaryReaderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Reader_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryReaderExtensions.ReadDictionary(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("reader");
            }

        }

        public class ReadDictionary_Typed : EnrichedBinaryReaderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Reader_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryReaderExtensions.ReadDictionary<int, string>(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("reader");
            }

        }
    }
}