using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Exceptions;
using AllOverIt.Serialization.Binary.Extensions;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AllOverIt.Tests.Serialization.Binary
{
    // ALL TESTS ARE BEHAVIOURAL AND MAY NOT REFLECT HOW THE ACTUAL IMPLEMENTATION WORKS.
    // FUNCTIONAL TESTS COVER THE REAL IMPLEMENTATION.
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
            public void Should_Throw_When_Unknown_Enum()
            {
                var enumTypeName = Create<string>();

                _readerFake
                    .CallsTo(fake => fake.ReadString())
                    .Returns(enumTypeName);

                Invoking(() =>
                {
                    _ = EnrichedBinaryReaderExtensions.ReadEnum<DummyEnum>(_readerFake.FakedObject);
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

                var actual = EnrichedBinaryReaderExtensions.ReadEnum<DummyEnum>(_readerFake.FakedObject);

                actual.Should().Be(value);
            }
        }

        public class ReadObject_Typed : EnrichedBinaryReaderExtensionsFixture
        {
            [Fact]
            public void Should_Read_Object()
            {
                var value = Create<DummyType>();

                _readerFake
                    .CallsTo(fake => fake.ReadObject())
                    .Returns(value);

                var actual = EnrichedBinaryReaderExtensions.ReadObject<DummyType>(_readerFake.FakedObject);

                actual.Should().Be(value);
            }
        }

        public class ReadNullable : EnrichedBinaryReaderExtensionsFixture
        {
            [Fact]
            public void Should_Read_Nullablet()
            {
                int? value = Create<int>();

                _readerFake
                    .CallsTo(fake => fake.ReadObject())
                    .Returns(value);

                var actual = EnrichedBinaryReaderExtensions.ReadNullable<int>(_readerFake.FakedObject);

                actual.Should().Be(value);
            }
        }

        public class ReadEnumerable : EnrichedBinaryReaderExtensionsFixture
        {
            [Fact]
            public void Should_Read_Enumerable()
            {
                var values = new[] { 1, 0, 1, -1, 2 };

                _readerFake
                    .CallsTo(fake => fake.ReadInt32())
                    .Returns(5);

                _readerFake
                    .CallsTo(fake => fake.ReadObject())
                    .ReturnsNextFromSequence(1, 0, 1, -1, 2);

                var actual = EnrichedBinaryReaderExtensions
                    .ReadEnumerable(_readerFake.FakedObject)
                    .Select(item => (int) item);

                actual.SequenceEqual(values).Should().BeTrue();
            }
        }

        public class ReadEnumumerable_Typed : EnrichedBinaryReaderExtensionsFixture
        {
            [Fact]
            public void Should_Read_Enumerable()
            {
                var values = new[] { 1, 0, 1, -1, 2 };

                _readerFake
                    .CallsTo(fake => fake.ReadInt32())
                    .Returns(5);

                _readerFake
                    .CallsTo(fake => fake.ReadObject())
                    .ReturnsNextFromSequence(1, 0, 1, -1, 2);

                var actual = EnrichedBinaryReaderExtensions.ReadEnumerable<int>(_readerFake.FakedObject);

                actual.SequenceEqual(values).Should().BeTrue();
            }
        }

        public class ReadDictionary : EnrichedBinaryReaderExtensionsFixture
        {
            [Fact]
            public void Should_Read_Dictionary()
            {
                var dictionary = new Dictionary<object, object>
                {
                    { Create<int>(), Create<string>() },
                    { Create<double>(), Create<long>() },
                    { Create<string>(), Create<double>() },
                };

                var keys = dictionary.Keys.ToList();
                var values = dictionary.Values.ToList();

                _readerFake
                    .CallsTo(fake => fake.ReadInt32())
                    .Returns(3);

                _readerFake
                    .CallsTo(fake => fake.ReadObject())
                    .ReturnsNextFromSequence(keys[0], values[0], keys[1], values[1], keys[2], values[2]);

                var actual = EnrichedBinaryReaderExtensions.ReadDictionary(_readerFake.FakedObject);

                actual.Should().BeEquivalentTo(dictionary);
            }
        }

        public class ReadDictionary_Typed : EnrichedBinaryReaderExtensionsFixture
        {
            [Fact]
            public void Should_Read_Dictionary_When_Object_Object()
            {
                var dictionary = new Dictionary<object, object>
                {
                    { Create<int>(), Create<string>() },
                    { Create<double>(), Create<long>() },
                    { Create<string>(), Create<double>() },
                };

                var keys = dictionary.Keys.ToList();
                var values = dictionary.Values.ToList();

                _readerFake
                    .CallsTo(fake => fake.ReadInt32())
                    .Returns(3);

                _readerFake
                    .CallsTo(fake => fake.ReadObject())
                    .ReturnsNextFromSequence(keys[0], values[0], keys[1], values[1], keys[2], values[2]);

                var actual = EnrichedBinaryReaderExtensions.ReadDictionary<object, object>(_readerFake.FakedObject);

                actual.Should().BeEquivalentTo(dictionary);
            }

            [Fact]
            public void Should_Read_Dictionary_When_Typed()
            {
                var dictionary = new Dictionary<int, string>
                {
                    { Create<int>(), Create<string>() },
                    { Create<int>(), Create<string>() },
                    { Create<int>(), Create<string>() },
                };

                var keys = dictionary.Keys.ToList();
                var values = dictionary.Values.ToList();

                _readerFake
                    .CallsTo(fake => fake.ReadInt32())
                    .Returns(3);

                _readerFake
                    .CallsTo(fake => fake.ReadObject())
                    .ReturnsNextFromSequence(keys[0], values[0], keys[1], values[1], keys[2], values[2]);

                var actual = EnrichedBinaryReaderExtensions.ReadDictionary<int, string>(_readerFake.FakedObject);

                actual.Should().BeEquivalentTo(dictionary);
            }
        }
    }
}