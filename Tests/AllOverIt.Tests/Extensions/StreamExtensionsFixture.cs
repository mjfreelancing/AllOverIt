using AllOverIt.Extensions;
using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using FluentAssertions;
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

        public class ReadFullBlock : StreamExtensionsFixture
        {
            // Helper class to simulate streams that return data in chunks
            private class SlowReadStream : MemoryStream
            {
                private readonly int _maxChunkSize;

                public SlowReadStream(byte[] buffer, int maxChunkSize) : base(buffer)
                {
                    _maxChunkSize = maxChunkSize;
                }

                public override int Read(byte[] buffer, int offset, int count)
                {
                    var chunkSize = Math.Min(count, _maxChunkSize);
                    return base.Read(buffer, offset, chunkSize);
                }
            }

            [Fact]
            public void Should_Throw_When_Stream_Null()
            {
                Invoking(() =>
                {
                    Stream stream = null;
                    var buffer = new byte[10];

                    _ = StreamExtensions.ReadFullBlock(stream, buffer, 10);
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
                    var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                    using var stream = new MemoryStream(data);

                    byte[] buffer = null;

                    _ = StreamExtensions.ReadFullBlock(stream, buffer, 10);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("bytes");
            }

            [Fact]
            public void Should_Read_Full_Block()
            {
                var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                using var stream = new MemoryStream(data);
                var buffer = new byte[10];

                var bytesRead = stream.ReadFullBlock(buffer, 10);

                bytesRead.Should().Be(10);
                buffer.Should().BeEquivalentTo(data);
            }

            [Fact]
            public void Should_Read_Partial_Block_When_Stream_Ends()
            {
                var data = new byte[] { 1, 2, 3, 4, 5 };
                using var stream = new MemoryStream(data);
                var buffer = new byte[10];

                var bytesRead = stream.ReadFullBlock(buffer, 10);

                bytesRead.Should().Be(5);
                buffer.Take(5).Should().BeEquivalentTo(data);
            }

            [Fact]
            public void Should_Return_Zero_When_Stream_Empty()
            {
                using var stream = new MemoryStream();
                var buffer = new byte[10];

                var bytesRead = stream.ReadFullBlock(buffer, 10);

                bytesRead.Should().Be(0);
            }

            [Fact]
            public void Should_Handle_Multiple_Partial_Reads()
            {
                var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                using var stream = new SlowReadStream(data, 3); // Read 3 bytes at a time
                var buffer = new byte[10];

                var bytesRead = stream.ReadFullBlock(buffer, 10);

                bytesRead.Should().Be(10);
                buffer.Should().BeEquivalentTo(data);
            }

            [Fact]
            public void Should_Read_From_Current_Position()
            {
                var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                using var stream = new MemoryStream(data);
                stream.Position = 5;
                var buffer = new byte[5];

                var bytesRead = stream.ReadFullBlock(buffer, 5);

                bytesRead.Should().Be(5);
                buffer.Should().BeEquivalentTo(new byte[] { 6, 7, 8, 9, 10 });
            }

            [Fact]
            public void Should_Read_Less_Than_Block_Size_Available()
            {
                var data = new byte[] { 1, 2, 3, 4, 5, 6, 7 };
                using var stream = new MemoryStream(data);
                var buffer = new byte[10];

                var bytesRead = stream.ReadFullBlock(buffer, 5);

                bytesRead.Should().Be(5);
                buffer.Take(5).Should().BeEquivalentTo(new byte[] { 1, 2, 3, 4, 5 });
            }
        }
    }
}
