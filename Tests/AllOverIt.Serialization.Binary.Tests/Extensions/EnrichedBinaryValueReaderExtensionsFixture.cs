using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Serialization.Binary.Readers;
using AllOverIt.Serialization.Binary.Readers.Extensions;
using FakeItEasy;
using FluentAssertions;

namespace AllOverIt.Serialization.Binary.Tests.Extensions
{
    public class EnrichedBinaryValueReaderExtensionsFixture : FixtureBase
    {
        public class ReadValue : EnrichedBinaryValueReaderExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_ValueReader_Null()
            {
                Invoking(() =>
                {
                    _ = EnrichedBinaryValueReaderExtensions.ReadValue<string>(null, this.CreateStub<IEnrichedBinaryReader>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("valueReader");
            }

            [Fact]
            public void Should_Throw_When_Reader_Null()
            {
                Invoking(() =>
                {
                    _ = this.CreateStub<IEnrichedBinaryValueReader>().ReadValue<string>(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("reader");
            }

            [Fact]
            public void Should_Read_Value()
            {
                var valueReaderFake = new Fake<IEnrichedBinaryValueReader>();
                var readerFake = this.CreateStub<IEnrichedBinaryReader>();

                var expected = Create<string>();

                valueReaderFake
                   .CallsTo(fake => fake.ReadValue(readerFake))
                   .Returns(expected);

                var actual = valueReaderFake.FakedObject.ReadValue<string>(readerFake);

                actual.Should().Be(expected);
            }
        }
    }
}