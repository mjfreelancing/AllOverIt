using System.IO;
using System.Text;
using AllOverIt.Fixture;
using AllOverIt.Io;
using FluentAssertions;
using Xunit;

namespace AllOverIt.Tests.Io
{
    public class TextStreamerFixture : FixtureBase
    {
        public class Constructor : TextStreamerFixture
        {
            private readonly TextStreamer _streamer = new();

            [Fact]
            public void Should_Set_Default_Encoding()
            {
                // Defined by StreamWriter
                var defaultEncoding = new UTF8Encoding(false, true);

                _streamer.GetWriter().Encoding.Should().BeEquivalentTo(defaultEncoding);
            }

            [Fact]
            public void Should_Set_Default_Capacity()
            {
                var memoryStream = (MemoryStream) _streamer.GetWriter().BaseStream;

                memoryStream.Capacity.Should().Be(0);
            }

            // No way to test this
            //[Fact]
            //public void Should_Set_Default_BufferSize()
            //{
            //}

            [Fact]
            public void Should_Set_Initial_Text()
            {
                var expected = Create<string>();

                var stream = new TextStreamer(expected);

                var actual = stream.ToString();

                expected.Should().Be(actual);
            }
        }

        public class GetWriter : TextStreamerFixture
        {
            [Fact]
            public void Should_Return_Writer()
            {
                var stream = new TextStreamer();
                var writer = stream.GetWriter();

                writer.Should().BeOfType<StreamWriter>();
            }

            [Fact]
            public void Should_Have_Expected_BaseStream()
            {
                var stream = new TextStreamer();
                var writer = stream.GetWriter();

                writer.BaseStream.Should().BeSameAs(stream);
            }

            [Fact]
            public void Should_Write_Text()
            {
                var sb = new StringBuilder();

                var value1 = Create<int>();
                var value2 = Create<string>();

                sb.Append(value1);
                sb.AppendLine();
                sb.Append(value2);

                var expected = sb.ToString();

                var stream = new TextStreamer();
                var writer = stream.GetWriter();

                writer.Write(value1);
                writer.WriteLine();
                writer.Write(value2);

                var actual = stream.ToString();

                expected.Should().Be(actual);
            }
        }

        public class ToStringMethod : TextStreamerFixture
        {
            [Fact]
            public void Should_Return_Text()
            {
                var sb = new StringBuilder();

                var value1 = Create<int>();
                var value2 = Create<string>();

                sb.Append(value1);
                sb.AppendLine();
                sb.Append(value2);

                var expected = sb.ToString();

                var stream = new TextStreamer();
                var writer = stream.GetWriter();

                writer.Write(value1);
                writer.WriteLine();
                writer.Write(value2);

                var actual = stream.ToString();

                expected.Should().Be(actual);
            }
        }
    }
}