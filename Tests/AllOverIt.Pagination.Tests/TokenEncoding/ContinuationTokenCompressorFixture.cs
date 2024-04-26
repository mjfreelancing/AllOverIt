using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Fixture.FakeItEasy;
using AllOverIt.Pagination.TokenEncoding;
using FakeItEasy;
using FluentAssertions;
using System.IO.Compression;

namespace AllOverIt.Pagination.Tests.TokenEncoding
{
    public class ContinuationTokenCompressorFixture : FixtureBase
    {
        // Need this as A.Fake<IContinuationTokenStreamer>() isn't working for an internal interface
        private class ContinuationTokenStreamerDummy : IContinuationTokenStreamer
        {
            public IContinuationToken ContinuationToken { get; private set; }
            public Stream Stream { get; private set; }

            public IContinuationToken DeserializeFromStream(Stream stream)
            {
                Stream = stream;

                return A.Fake<IContinuationToken>();
            }

            public void SerializeToStream(IContinuationToken continuationToken, Stream stream)
            {
                ContinuationToken = continuationToken;
                Stream = stream;
            }
        }

        private readonly ContinuationTokenStreamerDummy _continuationTokenStreamer;
        private readonly ContinuationTokenCompressor _continuationTokenCompressor;

        public ContinuationTokenCompressorFixture()
        {
            _continuationTokenStreamer = new ContinuationTokenStreamerDummy();
            _continuationTokenCompressor = new ContinuationTokenCompressor(_continuationTokenStreamer);
        }

        public class Constructor : ContinuationTokenCompressorFixture
        {
            [Fact]
            public void Should_Throw_When_Token_Streamer_Null()
            {
                Invoking(() =>
                {
                    _ = new ContinuationTokenCompressor(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("tokenStreamer");
            }
        }

        public class SerializeToStream : ContinuationTokenCompressorFixture
        {
            [Fact]
            public void Should_Throw_When_Token_Null()
            {
                Invoking(() =>
                {
                    _continuationTokenCompressor.SerializeToStream(null, Stream.Null);
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
                    _continuationTokenCompressor.SerializeToStream(this.CreateStub<IContinuationToken>(), null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("stream");
            }

            [Fact]
            public void Should_Serialize_To_Stream()
            {
                var continuationToken = this.CreateStub<IContinuationToken>();

                _continuationTokenCompressor.SerializeToStream(continuationToken, Stream.Null);

                _continuationTokenStreamer.ContinuationToken.Should().BeSameAs(continuationToken);
                _continuationTokenStreamer.Stream.Should().BeOfType<DeflateStream>();
            }
        }

        public class DeserializeFromStream : ContinuationTokenCompressorFixture
        {
            [Fact]
            public void Should_Throw_When_Stream_Null()
            {
                Invoking(() =>
                {
                    _ = _continuationTokenCompressor.DeserializeFromStream(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("stream");
            }

            [Fact]
            public void Should_Deserialize_From_Stream()
            {
                var continuationToken = _continuationTokenCompressor.DeserializeFromStream(Stream.Null);

                continuationToken.Should().NotBeNull();

                _continuationTokenStreamer.Stream.Should().BeOfType<DeflateStream>();
            }
        }
    }
}