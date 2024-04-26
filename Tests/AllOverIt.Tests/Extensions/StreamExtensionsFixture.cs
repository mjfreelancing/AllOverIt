using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
using System;
using System.IO;
using System.Text;

namespace AllOverIt.Tests.Extensions
{
    public class StreamExtensionsFixture : FixtureBase
    {
        public class ToByteArray : StreamExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Stream_Null()
            {
                Invoking(() =>
                {
                    Stream stream = null;

                    _ = StreamExtensions.ToByteArray(stream);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("stream");
            }

            [Fact]
            public void Should_Get_Bytes()
            {
                var expected = Encoding.UTF8.GetBytes(Create<string>());

                using (var stream = new MemoryStream(expected))
                {
                    var actual = StreamExtensions.ToByteArray(stream);

                    actual.Should().BeEquivalentTo(expected);
                }
            }
        }

        public class FromByteArray : StreamExtensionsFixture
        {
            [Fact]
            public void Should_Throw_When_Stream_Null()
            {
                Invoking(() =>
                {
                    Stream stream = null;

                    StreamExtensions.FromByteArray(stream, Array.Empty<byte>());
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("stream");
            }

            [Fact]
            public void Should_Throw_When_Bytes_Null()
            {
                Invoking(() =>
                {
                    using (var stream = new MemoryStream())
                    {
                        StreamExtensions.FromByteArray(stream, null);
                    }
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("bytes");
            }

            [Fact]
            public void Should_Get_Bytes()
            {
                var expected = Encoding.UTF8.GetBytes(Create<string>());

                using (var stream = new MemoryStream())
                {
                    StreamExtensions.FromByteArray(stream, expected);

                    var actual = stream.ToArray();

                    actual.Should().BeEquivalentTo(expected);
                }
            }

            [Fact]
            public void Should_Leave_Stream_Open()
            {
                var expected = Encoding.UTF8.GetBytes(Create<string>());

                using (var stream = new MemoryStream())
                {
                    StreamExtensions.FromByteArray(stream, expected);

                    stream.Position = 0;                // Will throw of the stream has been closed

                    var actual = stream.ToArray();

                    actual.Should().BeEquivalentTo(expected);
                }
            }
        }
    }
}
