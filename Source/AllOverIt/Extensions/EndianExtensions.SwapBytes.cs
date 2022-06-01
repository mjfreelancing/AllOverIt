using System.Runtime.InteropServices;

namespace AllOverIt.Extensions
{
    public static partial class EndianExtensions
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct FloatToUInt32
        {
            [FieldOffset(0)] public float Float;
            [FieldOffset(0)] public uint UInt32;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct DoubleToUInt64
        {
            [FieldOffset(0)] public double Double;
            [FieldOffset(0)] public ulong UInt64;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct DecimalToFourInts
        {
            [FieldOffset(0)] public decimal Decimal;
            [FieldOffset(0)] public uint UInt32_1;
            [FieldOffset(4)] public uint UInt32_2;
            [FieldOffset(8)] public uint UInt32_3;
            [FieldOffset(12)] public uint UInt32_4;
        }

        /// <summary>Returns an unsigned short after swapping the bytes.</summary>
        /// <param name="value">The original value to have its bytes swapped.</param>
        /// <returns>The new value after the bytes of the original value have been swapped.</returns>
        public static ushort SwapBytes(this ushort value)
        {
            return unchecked((ushort) (((value & 0xFF00U) >> 8) | ((value & 0x00FFU) << 8)));
        }

        /// <summary>Returns a short after swapping the bytes.</summary>
        /// <param name="value">The original value to have its bytes swapped.</param>
        /// <returns>The new value after the bytes of the original value have been swapped.</returns>
        public static short SwapBytes(this short value)
        {
            return unchecked((short) SwapBytes(unchecked((ushort) value)));
        }

        /// <summary>Returns an unsigned int after swapping the bytes.</summary>
        /// <param name="value">The original value to have its bytes swapped.</param>
        /// <returns>The new value after the bytes of the original value have been swapped.</returns>
        public static uint SwapBytes(this uint value)
        {
            // Swap adjacent 16-bit blocks
            value = (value >> 16) | (value << 16);

            // Swap adjacent 8-bit blocks
            return ((value & 0xFF00FF00U) >> 8) | ((value & 0x00FF00FFU) << 8);
        }

        /// <summary>Returns an int after swapping the bytes.</summary>
        /// <param name="value">The original value to have its bytes swapped.</param>
        /// <returns>The new value after the bytes of the original value have been swapped.</returns>
        public static int SwapBytes(this int value)
        {
            return unchecked((int) SwapBytes(unchecked((uint) value)));
        }

        /// <summary>Returns an unsigned long after swapping the bytes.</summary>
        /// <param name="value">The original value to have its bytes swapped.</param>
        /// <returns>The new value after the bytes of the original value have been swapped.</returns>
        public static ulong SwapBytes(this ulong value)
        {
            // Swap adjacent 32-bit blocks
            value = (value >> 32) | (value << 32);

            // Swap adjacent 16-bit blocks
            value = ((value & 0xFFFF0000FFFF0000U) >> 16) | ((value & 0x0000FFFF0000FFFFU) << 16);

            // Swap adjacent 8-bit blocks
            return ((value & 0xFF00FF00FF00FF00U) >> 8) | ((value & 0x00FF00FF00FF00FFU) << 8);
        }

        /// <summary>Returns a long after swapping the bytes.</summary>
        /// <param name="value">The original value to have its bytes swapped.</param>
        /// <returns>The new value after the bytes of the original value have been swapped.</returns>
        public static long SwapBytes(this long value)
        {
            return unchecked((long) SwapBytes(unchecked((ulong) value)));
        }

        /// <summary>Returns a float after swapping the bytes.</summary>
        /// <param name="value">The original value to have its bytes swapped.</param>
        /// <returns>The new value after the bytes of the original value have been swapped.</returns>
        public static float SwapBytes(this float value)
        {
            var union = new FloatToUInt32()
            {
                Float = value
            };

            union.UInt32 = SwapBytes(union.UInt32);
            return union.Float;
        }

        /// <summary>Returns a double after swapping the bytes.</summary>
        /// <param name="value">The original value to have its bytes swapped.</param>
        /// <returns>The new value after the bytes of the original value have been swapped.</returns>
        public static double SwapBytes(this double value)
        {
            var union = new DoubleToUInt64()
            {
                Double = value
            };

            union.UInt64 = SwapBytes(union.UInt64);
            return union.Double;
        }

        /// <summary>Returns a decimal after swapping the bytes.</summary>
        /// <param name="value">The original value to have its bytes swapped.</param>
        /// <returns>The new value after the bytes of the original value have been swapped.</returns>
        public static decimal SwapBytes(this decimal value)
        {
            var union = new DecimalToFourInts()
            {
                Decimal = value
            };

            union.UInt32_1 = SwapBytes(union.UInt32_1);
            union.UInt32_2 = SwapBytes(union.UInt32_2);
            union.UInt32_3 = SwapBytes(union.UInt32_3);
            union.UInt32_4 = SwapBytes(union.UInt32_4);

            return union.Decimal;
        }
    }
}
