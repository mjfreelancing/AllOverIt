using AllOverIt.Extensions;
using AllOverIt.Fixture;
using FluentAssertions;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Xunit;

namespace AllOverIt.Tests.Extensions
{
    public partial class EndianExtensionsFixture : FixtureBase
    {
        public class SwapBytes_UShort : EndianExtensionsFixture
        {
            [Fact]
            public void Should_Swap_Bytes()
            {
                var value = Create<ushort>();

                var actual = EndianExtensions.SwapBytes(value);

                var expected = BitConverter.ToUInt16(BitConverter.GetBytes(value).Reverse().ToArray(), 0);

                actual.Should().Be(expected);
            }
        }

        public class SwapBytes_Short : EndianExtensionsFixture
        {
            [Fact]
            public void Should_Swap_Bytes()
            {
                var value = Create<short>();

                var actual = EndianExtensions.SwapBytes(value);

                var expected = BitConverter.ToInt16(BitConverter.GetBytes(value).Reverse().ToArray(), 0);

                actual.Should().Be(expected);
            }
        }

        public class SwapBytes_UInt : EndianExtensionsFixture
        {
            [Fact]
            public void Should_Swap_Bytes()
            {
                var value = Create<uint>();

                var actual = EndianExtensions.SwapBytes(value);

                var expected = BitConverter.ToUInt32(BitConverter.GetBytes(value).Reverse().ToArray(), 0);

                actual.Should().Be(expected);
            }
        }

        public class SwapBytes_Int : EndianExtensionsFixture
        {
            [Fact]
            public void Should_Swap_Bytes()
            {
                var value = Create<int>();

                var actual = EndianExtensions.SwapBytes(value);

                var expected = BitConverter.ToInt32(BitConverter.GetBytes(value).Reverse().ToArray(), 0);

                actual.Should().Be(expected);
            }
        }

        public class SwapBytes_ULong : EndianExtensionsFixture
        {
            [Fact]
            public void Should_Swap_Bytes()
            {
                var value = Create<ulong>();

                var actual = EndianExtensions.SwapBytes(value);

                var expected = BitConverter.ToUInt64(BitConverter.GetBytes(value).Reverse().ToArray(), 0);

                actual.Should().Be(expected);
            }
        }

        public class SwapBytes_Long : EndianExtensionsFixture
        {
            [Fact]
            public void Should_Swap_Bytes()
            {
                var value = Create<long>();

                var actual = EndianExtensions.SwapBytes(value);

                var expected = BitConverter.ToInt64(BitConverter.GetBytes(value).Reverse().ToArray(), 0);

                actual.Should().Be(expected);
            }
        }

        public class SwapBytes_Float : EndianExtensionsFixture
        {
            [Fact]
            public void Should_Swap_Bytes()
            {
                var value = Create<float>();

                var actual = EndianExtensions.SwapBytes(value);

                var expected = BitConverter.ToSingle(BitConverter.GetBytes(value).Reverse().ToArray(), 0);

                actual.Should().Be(expected);
            }
        }

        public class SwapBytes_Double : EndianExtensionsFixture
        {
            [Fact]
            public void Should_Swap_Bytes()
            {
                var value = Create<double>();

                var actual = EndianExtensions.SwapBytes(value);

                var expected = BitConverter.ToDouble(BitConverter.GetBytes(value).Reverse().ToArray(), 0);

                actual.Should().Be(expected);
            }
        }

        public class SwapBytes_Decimal : EndianExtensionsFixture
        {
            [Fact]
            public void Should_Swap_Bytes()
            {
                var value = Create<decimal>();

                // From decimal.GetBits() source:
                // Returns a binary representation of a Decimal. The return value is an
                // integer array with four elements. Elements 0, 1, and 2 contain the low,
                // middle, and high 32 bits of the 96-bit integer part of the Decimal.
                // Element 3 contains the scale factor and sign of the Decimal: bits 0-15
                // (the lower word) are unused; bits 16-23 contain a value between 0 and
                // 28, indicating the power of 10 to divide the 96-bit integer part by to
                // produce the Decimal value; bits 24-30 are unused; and finally bit 31
                // indicates the sign of the Decimal value, 0 meaning positive and 1
                // meaning negative.

                // Get the four int elements that represent the decimal
                var valueInts = decimal.GetBits(value); 

                var actual = EndianExtensions.SwapBytes(value);
                var actualInts = decimal.GetBits(actual);

                // Can't create a decimal using 'new decimal(valueBytesReversed)' without getting an error
                // due to expected format so need to check the individual int elements. For reference.
                //
                // lo = bits[0];
                // mid = bits[1];
                // hi = bits[2];
                // flags = bits[3];     // this is validated

                // Byte order would be preserved, but each element will have its bytes reversed
                actualInts[0].Should().Be(valueInts[0].SwapBytes());
                actualInts[1].Should().Be(valueInts[1].SwapBytes());
                actualInts[2].Should().Be(valueInts[2].SwapBytes());
                actualInts[3].Should().Be(valueInts[3].SwapBytes());

                // also sanity check the reversal
                var sanityCheck = BitConverter.IsLittleEndian
                    ? actual.SwapBytes()
                    : actual;

                sanityCheck.Should().Be(value);
                $"{value}".Should().NotBe($"{actual}");
            }
        }
    }
}