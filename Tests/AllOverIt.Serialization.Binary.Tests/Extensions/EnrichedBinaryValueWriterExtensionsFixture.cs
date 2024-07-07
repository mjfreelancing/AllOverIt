using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Serialization.Binary.Writers;
using AllOverIt.Serialization.Binary.Writers.Extensions;
using FakeItEasy;
using FluentAssertions;

namespace AllOverIt.Serialization.Binary.Tests.Extensions
{
    public class EnrichedBinaryValueWriterExtensionsFixture : FixtureBase
    {
        public class WriteValue : EnrichedBinaryValueWriterExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_ValueWriter_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryValueWriterExtensions.WriteValue(null, this.CreateStub<IEnrichedBinaryWriter>(), Create<string>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("valueWriter");
            }

            [Fact]
            public void Should_Throw_When_Writer_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryValueWriterExtensions.WriteValue(this.CreateStub<IEnrichedBinaryValueWriter>(), null, Create<string>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Throw_When_Value_Null()
            {
                Invoking(() =>
                {
                    EnrichedBinaryValueWriterExtensions.WriteValue(this.CreateStub<IEnrichedBinaryValueWriter>(), this.CreateStub<IEnrichedBinaryWriter>(), (string) null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("value");
            }

            [Fact]
            public void Should_Not_Throw_When_Value_Not_Reference_Default()
            {
                Invoking(() =>
                {
                    EnrichedBinaryValueWriterExtensions.WriteValue(this.CreateStub<IEnrichedBinaryValueWriter>(), this.CreateStub<IEnrichedBinaryWriter>(), (int) default);
                })
                .Should()
                .NotThrow();
            }

            [Fact]
            public void Should_Write_Value()
            {
                var valueWriterFake = new Fake<IEnrichedBinaryValueWriter>();
                var writerFake = this.CreateStub<IEnrichedBinaryWriter>();

                var expected = Create<string>();
                string actual = default;

                valueWriterFake
                    .CallsTo(fake => fake.WriteValue(writerFake, expected))
                    .Invokes(call => actual = call.Arguments.Get<string>(1));

                EnrichedBinaryValueWriterExtensions.WriteValue(valueWriterFake.FakedObject, writerFake, expected);

                actual.Should().Be(expected);
            }
        }
    }
}