using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;

namespace AllOverIt.Tests.Extensions
{
    public partial class EndianExtensionsFixture : FixtureBase
    {
        // WhenBigEndian and WhenLittleEndian fixture will not be run in parallel

        [Collection("EndianExtensions")]
        public class WhenBigEndian : EndianExtensionsFixture
        {
            public WhenBigEndian()
            {
                EndianExtensions.InternalIsLittleEndian = false;
            }

            [Fact]
            public void Should_Convert_UShort_AsBigEndian()
            {
                var value = Create<ushort>();

                var actual = value.AsBigEndian();

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Convert_UShort_AsLittleEndian()
            {
                var value = Create<ushort>();

                var actual = value.AsLittleEndian();

                actual.Should().Be(value.SwapBytes());
            }

            [Fact]
            public void Should_Convert_Short_AsBigEndian()
            {
                var value = Create<short>();

                var actual = value.AsBigEndian();

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Convert_Short_AsLittleEndian()
            {
                var value = Create<short>();

                var actual = value.AsLittleEndian();

                actual.Should().Be(value.SwapBytes());
            }

            [Fact]
            public void Should_Convert_UInt_AsBigEndian()
            {
                var value = Create<uint>();

                var actual = value.AsBigEndian();

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Convert_UInt_AsLittleEndian()
            {
                var value = Create<uint>();

                var actual = value.AsLittleEndian();

                actual.Should().Be(value.SwapBytes());
            }

            [Fact]
            public void Should_Convert_Int_AsBigEndian()
            {
                var value = Create<int>();

                var actual = value.AsBigEndian();

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Convert_Int_AsLittleEndian()
            {
                var value = Create<int>();

                var actual = value.AsLittleEndian();

                actual.Should().Be(value.SwapBytes());
            }

            [Fact]
            public void Should_Convert_ULong_AsBigEndian()
            {
                var value = Create<ulong>();

                var actual = value.AsBigEndian();

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Convert_ULong_AsLittleEndian()
            {
                var value = Create<ulong>();

                var actual = value.AsLittleEndian();

                actual.Should().Be(value.SwapBytes());
            }

            [Fact]
            public void Should_Convert_Long_AsBigEndian()
            {
                var value = Create<long>();

                var actual = value.AsBigEndian();

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Convert_Long_AsLittleEndian()
            {
                var value = Create<long>();

                var actual = value.AsLittleEndian();

                actual.Should().Be(value.SwapBytes());
            }

            [Fact]
            public void Should_Convert_Float_AsBigEndian()
            {
                var value = Create<float>();

                var actual = value.AsBigEndian();

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Convert_Float_AsLittleEndian()
            {
                var value = Create<float>();

                var actual = value.AsLittleEndian();

                actual.Should().Be(value.SwapBytes());
            }

            [Fact]
            public void Should_Convert_Double_AsBigEndian()
            {
                var value = Create<double>();

                var actual = value.AsBigEndian();

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Convert_Double_AsLittleEndian()
            {
                var value = Create<double>();

                var actual = value.AsLittleEndian();

                actual.Should().Be(value.SwapBytes());
            }

            [Fact]
            public void Should_Convert_Decimal_AsBigEndian()
            {
                var value = Create<decimal>();

                var actual = value.AsBigEndian();

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Convert_Decimal_AsLittleEndian()
            {
                var value = Create<decimal>();

                var actual = value.AsLittleEndian();

                actual.Should().Be(value.SwapBytes());
            }
        }

        [Collection("EndianExtensions")]
        public class WhenLittleEndian : EndianExtensionsFixture
        {
            public WhenLittleEndian()
            {
                EndianExtensions.InternalIsLittleEndian = true;
            }

            [Fact]
            public void Should_Convert_UShort_AsBigEndian()
            {
                var value = Create<ushort>();

                var actual = value.AsBigEndian();

                actual.Should().Be(value.SwapBytes());
            }

            [Fact]
            public void Should_Convert_UShort_AsLittleEndian()
            {
                var value = Create<ushort>();

                var actual = value.AsLittleEndian();

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Convert_Short_AsBigEndian()
            {
                var value = Create<short>();

                var actual = value.AsBigEndian();

                actual.Should().Be(value.SwapBytes());
            }

            [Fact]
            public void Should_Convert_Short_AsLittleEndian()
            {
                var value = Create<short>();

                var actual = value.AsLittleEndian();

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Convert_UInt_AsBigEndian()
            {
                var value = Create<uint>();

                var actual = value.AsBigEndian();

                actual.Should().Be(value.SwapBytes());
            }

            [Fact]
            public void Should_Convert_UInt_AsLittleEndian()
            {
                var value = Create<uint>();

                var actual = value.AsLittleEndian();

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Convert_Int_AsBigEndian()
            {
                var value = Create<int>();

                var actual = value.AsBigEndian();

                actual.Should().Be(value.SwapBytes());
            }

            [Fact]
            public void Should_Convert_Int_AsLittleEndian()
            {
                var value = Create<int>();

                var actual = value.AsLittleEndian();

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Convert_ULong_AsBigEndian()
            {
                var value = Create<ulong>();

                var actual = value.AsBigEndian();

                actual.Should().Be(value.SwapBytes());
            }

            [Fact]
            public void Should_Convert_ULong_AsLittleEndian()
            {
                var value = Create<ulong>();

                var actual = value.AsLittleEndian();

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Convert_Long_AsBigEndian()
            {
                var value = Create<long>();

                var actual = value.AsBigEndian();

                actual.Should().Be(value.SwapBytes());
            }

            [Fact]
            public void Should_Convert_Long_AsLittleEndian()
            {
                var value = Create<long>();

                var actual = value.AsLittleEndian();

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Convert_Float_AsBigEndian()
            {
                var value = Create<float>();

                var actual = value.AsBigEndian();

                actual.Should().Be(value.SwapBytes());
            }

            [Fact]
            public void Should_Convert_Float_AsLittleEndian()
            {
                var value = Create<float>();

                var actual = value.AsLittleEndian();

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Convert_Double_AsBigEndian()
            {
                var value = Create<double>();

                var actual = value.AsBigEndian();

                actual.Should().Be(value.SwapBytes());
            }

            [Fact]
            public void Should_Convert_Double_AsLittleEndian()
            {
                var value = Create<double>();

                var actual = value.AsLittleEndian();

                actual.Should().Be(value);
            }

            [Fact]
            public void Should_Convert_Decimal_AsBigEndian()
            {
                var value = Create<decimal>();

                var actual = value.AsBigEndian();

                actual.Should().Be(value.SwapBytes());
            }

            [Fact]
            public void Should_Convert_Decimal_AsLittleEndian()
            {
                var value = Create<decimal>();

                var actual = value.AsLittleEndian();

                actual.Should().Be(value);
            }
        }
    }
}