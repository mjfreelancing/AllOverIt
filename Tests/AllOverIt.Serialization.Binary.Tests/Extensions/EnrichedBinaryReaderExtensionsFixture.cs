using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Serialization.Binary.Exceptions;
using AllOverIt.Serialization.Binary.Readers;
using AllOverIt.Serialization.Binary.Readers.Extensions;
using FakeItEasy;
using Shouldly;

namespace AllOverIt.Serialization.Binary.Tests.Extensions
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

                _readerFake.FakedObject.ReadSafeString();

                _readerFake.CallsTo(fake => fake.ReadBoolean()).MustHaveHappened();
                _readerFake.CallsTo(fake => fake.ReadString()).MustNotHaveHappened();
            }

            [Fact]
            public void Should_Call_ReadString_When_Has_Value()
            {
                _readerFake
                    .CallsTo(fake => fake.ReadBoolean())
                    .Returns(true);

                _readerFake.FakedObject.ReadSafeString();

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

                var actual = _readerFake.FakedObject.ReadSafeString();

                actual.ShouldBe(default);
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

                var actual = _readerFake.FakedObject.ReadSafeString();

                actual.ShouldBe(value);
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

                var actual = _readerFake.FakedObject.ReadGuid();

                actual.ShouldBe(value);
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

                Should.Throw<BinaryReaderException>(() =>
                {
                    _ = _readerFake.FakedObject.ReadEnum();
                })
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

                var actual = _readerFake.FakedObject.ReadEnum();

                actual.ShouldBe(value);
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

                Should.Throw<BinaryReaderException>(() =>
                {
                    _ = _readerFake.FakedObject.ReadEnum<DummyEnum>();
                })
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

                var actual = _readerFake.FakedObject.ReadEnum<DummyEnum>();

                actual.ShouldBe(value);
            }
        }

        public class ReadDateTime : EnrichedBinaryReaderExtensionsFixture
        {
            [Fact]
            public void Should_Read_DateTime()
            {
                var value = Create<DateTime>();

                _readerFake
                    .CallsTo(fake => fake.ReadInt64())
                    .Returns(value.ToBinary());

                var actual = _readerFake.FakedObject.ReadDateTime();

                actual.ShouldBe(value);
            }
        }

        public class ReadDateOnly : EnrichedBinaryReaderExtensionsFixture
        {
            [Fact]
            public void Should_Read_DateOnly()
            {
                var value = Create<DateOnly>();

                _readerFake
                    .CallsTo(fake => fake.ReadInt32())
                    .Returns(value.DayNumber);

                var actual = _readerFake.FakedObject.ReadDateOnly();

                actual.ShouldBe(value);
            }
        }

        public class ReadTimeOnly : EnrichedBinaryReaderExtensionsFixture
        {
            [Fact]
            public void Should_Read_TimeOnly()
            {
                var value = Create<TimeOnly>();

                _readerFake
                    .CallsTo(fake => fake.ReadInt64())
                    .Returns(value.Ticks);

                var actual = _readerFake.FakedObject.ReadTimeOnly();

                actual.ShouldBe(value);
            }
        }

        public class ReadTimeSpan : EnrichedBinaryReaderExtensionsFixture
        {
            [Fact]
            public void Should_Read_TimeSpan()
            {
                var value = Create<TimeSpan>();

                _readerFake
                    .CallsTo(fake => fake.ReadInt64())
                    .Returns(value.Ticks);

                var actual = _readerFake.FakedObject.ReadTimeSpan();

                actual.ShouldBe(value);
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

                var actual = _readerFake.FakedObject.ReadObject<DummyType>();

                actual.ShouldBe(value);
            }
        }

        public class ReadObjectAsDictionary : EnrichedBinaryReaderExtensionsFixture
        {
            [Fact]
            public void Should_Read_Object()
            {
                var value = CreateMany<int>()
                    .Select(item => new KeyValuePair<object, object>(item, Create<DummyType>()))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                _readerFake
                    .CallsTo(fake => fake.ReadObject())
                    .Returns(value);

                var actual = _readerFake.FakedObject.ReadObjectAsDictionary<int, DummyType>();

                actual.Keys.ShouldBe(value.Keys.Cast<int>());
                actual.Values.ShouldBe(value.Values.Cast<DummyType>());
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

                var actual = _readerFake.FakedObject.ReadNullable<int>();

                actual.ShouldBe(value);
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
                    .CallsTo(fake => fake.ReadString())
                    .Returns(typeof(int).AssemblyQualifiedName);

                _readerFake
                    .CallsTo(fake => fake.ReadObject())
                    .ReturnsNextFromSequence(1, 0, 1, -1, 2);

                var actual = (List<int>)_readerFake.FakedObject.ReadEnumerable();

                actual.SequenceEqual(values).ShouldBeTrue();
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
                    .CallsTo(fake => fake.ReadString())
                    .Returns(typeof(int).AssemblyQualifiedName);

                _readerFake
                    .CallsTo(fake => fake.ReadObject())
                    .ReturnsNextFromSequence(1, 0, 1, -1, 2);

                var actual = _readerFake.FakedObject.ReadEnumerable<int>();

                actual.SequenceEqual(values).ShouldBeTrue();
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

                var actual = _readerFake.FakedObject.ReadDictionary();

                actual.ShouldBe(dictionary);
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

                var actual = _readerFake.FakedObject.ReadDictionary<object, object>();

                actual.ShouldBe(dictionary);
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

                var actual = _readerFake.FakedObject.ReadDictionary<int, string>();

                actual.ShouldBe(dictionary);
            }
        }
    }
}


