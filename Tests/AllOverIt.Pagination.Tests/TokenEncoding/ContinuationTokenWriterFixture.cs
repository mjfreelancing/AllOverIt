using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Pagination.TokenEncoding;
using AllOverIt.Serialization.Binary.Writers;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AllOverIt.Pagination.Tests.TokenEncoding
{
    public class ContinuationTokenWriterFixture : FixtureBase
    {
        public class ReadValue : ContinuationTokenWriterFixture
        {
            private readonly ContinuationTokenWriter _tokenWriter = new();
            private Fake<IEnrichedBinaryWriter> _binaryWriter;

            public ReadValue()
            {
                this.UseFakeItEasy();

                _binaryWriter = this.CreateFake<IEnrichedBinaryWriter>();
            }

            [Fact]
            public void Should_Throw_When_BinaryWriter_Null()
            {
                Invoking(() =>
                {
                    _tokenWriter.WriteValue(null, Create<object>());
                })
                    .Should()
                    .Throw<ArgumentNullException>()
                    .WithNamedMessageWhenNull("writer");
            }

            [Fact]
            public void Should_Write_Value()
            {
                var token = Create<ContinuationToken>();

                _tokenWriter.WriteValue(_binaryWriter.FakedObject, token);

                _binaryWriter
                    .CallsTo(call => call.Write((byte) token.Direction))
                    .MustHaveHappened();

                _binaryWriter
                    .CallsTo(call => call.WriteObject(token.Values, typeof(IReadOnlyCollection<object>)))
                    .MustHaveHappened();
            }
        }
    }
}