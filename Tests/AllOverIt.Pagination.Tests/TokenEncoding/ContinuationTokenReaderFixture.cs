using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Pagination.TokenEncoding;
using AllOverIt.Serialization.Binary.Readers;
using FakeItEasy;
using FluentAssertions;

namespace AllOverIt.Pagination.Tests.TokenEncoding
{
    public class ContinuationTokenReaderFixture : FixtureBase
    {
        public class ReadValue : ContinuationTokenReaderFixture
        {
            private readonly ContinuationTokenReader _tokenReader = new();
            private Fake<IEnrichedBinaryReader> _binaryReader;

            public ReadValue()
            {
                this.UseFakeItEasy();

                _binaryReader = this.CreateFake<IEnrichedBinaryReader>();
            }

            [Fact]
            public void Should_Throw_When_BinaryReader_Null()
            {
                Invoking(() =>
                {
                    _ = _tokenReader.ReadValue(null);
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("reader");
            }

            [Fact]
            public void Should_Read_Value()
            {
                var direction = Create<PaginationDirection>();

                _binaryReader
                    .CallsTo(call => call.ReadByte())
                    .Returns((byte) direction);

                var values = CreateMany<int>()
                    .Select(item => (object) item)
                    .ToList();

                _binaryReader
                    .CallsTo(call => call.ReadObject())
                    .Returns(values);

                var expected = new
                {
                    Direction = direction,
                    Values = values
                };

                var actual = _tokenReader.ReadValue(_binaryReader.FakedObject);

                actual.Should().BeEquivalentTo(expected);
            }
        }
    }
}