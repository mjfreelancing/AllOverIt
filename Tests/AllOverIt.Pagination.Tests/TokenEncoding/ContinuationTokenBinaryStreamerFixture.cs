using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pagination.TokenEncoding;
using FakeItEasy;
using FluentAssertions;
using System;
using System.IO;
using Xunit;

namespace AllOverIt.Pagination.Tests.TokenEncoding
{
    public class ContinuationTokenBinaryStreamerFixture : FixtureBase
    {
        private readonly ContinuationTokenBinaryStreamer _continuationTokenBinaryStreamer = new();

        public class SerializeToStream : ContinuationTokenBinaryStreamerFixture
        {
            [Fact]
            public void Should_Throw_When_Token_Null()
            {
                Invoking(() =>
                {
                    _continuationTokenBinaryStreamer.SerializeToStream(null, Stream.Null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("continuationToken");
            }

            [Fact]
            public void Should_Throw_When_Stream_Null()
            {
                Invoking(() =>
                {
                    _continuationTokenBinaryStreamer.SerializeToStream(A.Fake<IContinuationToken>(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("stream");
            }

            [Fact]
            public void Should_Serialize_To_Stream()
            {
                var continuationToken = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[] { Create<bool>(), Create<int>() }
                };

                using (var stream = new MemoryStream())
                {
                    _continuationTokenBinaryStreamer.SerializeToStream(continuationToken, stream);

                    stream.ToArray().Should().NotBeEmpty();
                }
            }
        }

        public class DeserializeFromStream : ContinuationTokenBinaryStreamerFixture
        {
            [Fact]
            public void Should_Throw_When_Stream_Null()
            {
                Invoking(() =>
                {
                    _ = _continuationTokenBinaryStreamer.DeserializeFromStream(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("stream");
            }

            [Fact]
            public void Should_Deserialize_From_Stream()
            {
                var continuationToken = new ContinuationToken
                {
                    Direction = Create<PaginationDirection>(),
                    Values = new object[] { Create<bool>(), Create<int>() }
                };

                using (var stream = new MemoryStream())
                {
                    _continuationTokenBinaryStreamer.SerializeToStream(continuationToken, stream);

                    stream.Position = 0;

                    var actual = _continuationTokenBinaryStreamer.DeserializeFromStream(stream);

                    continuationToken.Should().BeEquivalentTo(actual);
                }
            }
        }
    }
}